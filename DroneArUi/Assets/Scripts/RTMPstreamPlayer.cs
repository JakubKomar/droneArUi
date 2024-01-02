/*
 * modified example for low latency rtmp stream script
 * author: jakub komárek
 * 
 * 
*/

using UnityEngine;
using System;
using LibVLCSharp;
using UnityEngine.UI;
using System.Collections.Generic;

///This is a basic implementation of a media player using VLC for Unity using LibVLCSharp
///It exposes some basic playback controls, you may wish to add more of these
///It outputs audio directly to speakers and video to a RenderTexture and a Renderer or RawImage screen
///This example also shows how to deal with several common problems including vertically flipped videos
///
/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
///libvlcsharp usage documentation: https://code.videolan.org/videolan/LibVLCSharp/-/blob/master/docs/home.md
///LibVLC parameters: https://wiki.videolan.org/VLC_command-line_help/
///Report a bug: https://code.videolan.org/videolan/vlc-unity/-/issues

public class RTMPstreamPlayer : Singleton<RTMPstreamPlayer>
{

    public static LibVLC libVLC; //The LibVLC class is mainly used for making MediaPlayer and Media objects. You should only have one LibVLC instance.
	public MediaPlayer mediaPlayer; //MediaPlayer is the main class we use to interact with VLC

	//Screens
	public Renderer screen; //Assign a mesh to render on a 3d object
	public RawImage canvasScreen; //Assign a Canvas RawImage to render on a GUI object

	Texture2D _vlcTexture = null; //This is the texture libVLC writes to directly. It's private.
	public RenderTexture texture = null; //We copy it into this texture which we actually use in unity.

	public string ip= "127.0.0.1";
    public string port= "1935";
	public string dronId = "6ab6be76";
	public bool manualId = false;

	private string fullPath;
    // when copying native Texture2D textures to Unity RenderTextures, the orientation mapping is incorrect on Android, so we flip it over.
    public bool flipTextureX = true;
	public bool flipTextureY = true;

	public bool logToConsole = false; //Log function calls and LibVLC logs to Unity console

	//Unity Awake, OnDestroy, and Update functions
	#region unity
	void Awake()
	{
		//Setup LibVLC
		if (libVLC == null)
			CreateLibVLC();

		//Setup Screen
		if (screen == null)
			screen = GetComponent<Renderer>();
		if (canvasScreen == null)
			canvasScreen = GetComponent<RawImage>();
		this.fullPath = "rtmp://" + ip + ":" + port + "/live/";

        CreateMediaPlayer();
		if (manualId) {
			Open(fullPath + dronId);
		}
			
	}

	void OnDestroy()
	{
		//Dispose of mediaPlayer, or it will stay in nemory and keep playing audio
		DestroyMediaPlayer();
        libVLC.Dispose();

    }

	void Update()
	{
		//Get size every frame
		uint height = 0;
		uint width = 0;
		mediaPlayer.Size(0, ref width, ref height);

		//Automatically resize output textures if size changes
		if (_vlcTexture == null || _vlcTexture.width != width || _vlcTexture.height != height)
		{
			ResizeOutputTextures(width, height);
		}

		if (_vlcTexture != null)
		{
			//Update the vlc texture (tex)
			var texptr = mediaPlayer.GetTexture(width, height, out bool updated);
			if (updated)
			{
				_vlcTexture.UpdateExternalTexture(texptr);

				//Copy the vlc texture into the output texture, automatically flipped over
				var flip = new Vector2(flipTextureX ? -1 : 1, flipTextureY ? -1 : 1);
				Graphics.Blit(_vlcTexture, texture, flip, Vector2.zero); //If you wanted to do post processing outside of VLC you could use a shader here.
			}
		}
	}
	#endregion


	//Public functions that expose VLC MediaPlayer functions in a Unity-friendly way. You may want to add more of these.
	#region vlc
	public void Open(string path)
	{
        Log("VLCPlayerExample Open " + path);
        if (mediaPlayer.Media != null)
            mediaPlayer.Media.Dispose();

        var trimmedPath = path.Trim(new char[] { '"' });//Windows likes to copy paths with quotes but Uri does not like to open them
        mediaPlayer.Media = new Media(new Uri(trimmedPath));
        Play();
    }

	public void Play()
	{
		Log("VLCPlayerExample Play");

		mediaPlayer.Play();
	}

	public void OnDroneConnected(string conectedDroneId){
		if (manualId)
			return;
        Open(fullPath + conectedDroneId);
    }
	

	//This returns the video orientation for the currently playing video, if there is one
	public VideoOrientation? GetVideoOrientation()
	{
		var tracks = mediaPlayer?.Tracks(TrackType.Video);

		if (tracks == null || tracks.Count == 0)
			return null;

		var orientation = tracks[0]?.Data.Video.Orientation; //At the moment we're assuming the track we're playing is the first track

		return orientation;
	}

	#endregion

	//Private functions create and destroy VLC objects and textures
	#region internal
	//Create a new static LibVLC instance and dispose of the old one. You should only ever have one LibVLC instance.
	void CreateLibVLC()
	{
		Log("VLCPlayerExample CreateLibVLC");
		//Dispose of the old libVLC if necessary
		if (libVLC != null)
		{
			libVLC.Dispose();
			libVLC = null;
		}

		Core.Initialize(Application.dataPath); //Load VLC dlls
		libVLC = new LibVLC(enableDebugLogs: true, "--clock-synchro=1", "--low-delay", "--no-osd", "--network-caching=0", "--no-audio"); //You can customize LibVLC with advanced CLI options here https://wiki.videolan.org/VLC_command-line_help/
															
		//Setup Error Logging
		Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
		libVLC.Log += (s, e) =>
		{
			//Always use try/catch in LibVLC events.
			//LibVLC can freeze Unity if an exception goes unhandled inside an event handler.
			try
			{
				if (logToConsole)
				{
					Log(e.FormattedLog);
				}
			}
			catch (Exception ex)
			{
				Log("Exception caught in libVLC.Log: \n" + ex.ToString());
			}

		};
	}

	//Create a new MediaPlayer object and dispose of the old one.
	void CreateMediaPlayer()
	{
		Log("VLCPlayerExample CreateMediaPlayer");
		if (mediaPlayer != null)
		{
			DestroyMediaPlayer();
		}
		mediaPlayer = new MediaPlayer(libVLC);
	}

	//Dispose of the MediaPlayer object.
	void DestroyMediaPlayer()
	{
		Log("VLCPlayerExample DestroyMediaPlayer");
        mediaPlayer?.Stop();

        _vlcTexture = null;
        texture = null;

		mediaPlayer?.Dispose();
		mediaPlayer = null;
	}

	//Resize the output textures to the size of the video
	void ResizeOutputTextures(uint px, uint py)
	{
		var texptr = mediaPlayer.GetTexture(px, py, out bool updated);
		if (px != 0 && py != 0 && updated && texptr != IntPtr.Zero)
		{
			//If the currently playing video uses the Bottom Right orientation, we have to do this to avoid stretching it.
			if(GetVideoOrientation() == VideoOrientation.BottomRight)
			{
				uint swap = px;
				px = py;
				py = swap;
			}

			_vlcTexture = Texture2D.CreateExternalTexture((int)px, (int)py, TextureFormat.RGBA32, false, true, texptr); //Make a texture of the proper size for the video to output to
			texture = new RenderTexture(_vlcTexture.width, _vlcTexture.height, 0, RenderTextureFormat.ARGB32); //Make a renderTexture the same size as vlctex

			if (screen != null)
				screen.material.mainTexture = texture;
			if (canvasScreen != null)
				canvasScreen.texture = texture;
		}
	}

	void Log(object message)
	{
		if(logToConsole)
			Debug.Log(message);
	}
	#endregion
}
