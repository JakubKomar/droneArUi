using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LibVLCSharp;


///This script controls all the GUI for the VLC Unity Canvas Example
///It sets up event handlers and updates the GUI every frame
///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
public class VLCPlayerExampleGui : MonoBehaviour
{
	public VLCPlayerExample vlcPlayer;
	
	//GUI Elements
	public RawImage screen;
	public AspectRatioFitter screenAspectRatioFitter;
	public Slider seekBar;
	public Button playButton;
	public Button pauseButton;
	public Button stopButton;
	public Button pathButton;
	public Button tracksButton;
	public Button volumeButton;
	public GameObject pathGroup; //Group containing pathInputField and openButton
	public InputField pathInputField;
	public Button openButton;
	public GameObject tracksButtonsGroup; //Group containing buttons to switch video, audio, and subtitle tracks
	public Slider volumeBar;
	public GameObject trackButtonPrefab;
	public GameObject trackLabelPrefab;
	public Color unselectedButtonColor; //Used for unselected track text
	public Color selectedButtonColor; //Used for selected track text

	//Configurable Options
	public int maxVolume = 100; //The highest volume the slider can reach. 100 is usually good but you can go higher.

	//State variables
	bool _isPlaying = false; //We use VLC events to track whether we are playing, rather than relying on IsPlaying 
	bool _isDraggingSeekBar = false; //We advance the seek bar every frame, unless the user is dragging it

	///Unity wants to do everything on the main thread, but VLC events use their own thread.
	///These variables can be set to true in a VLC event handler indicate that a function should be called next Update.
	///This is not actually thread safe and should be gone soon!
	bool _shouldUpdateTracks = false; //Set this to true and the Tracks menu will regenerate next frame
	bool _shouldClearTracks = false; //Set this to true and the Tracks menu will clear next frame

	List<Button> _videoTracksButtons = new List<Button>();
	List<Button> _audioTracksButtons = new List<Button>();
	List<Button> _textTracksButtons = new List<Button>();


	void Start()
	{
		//VLC Event Handlers
		vlcPlayer.mediaPlayer.Playing += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				//Because many Unity functions can only be used on the main thread, they will fail in VLC event handlers
				//A simple way around this is to set flag variables which cause functions to be called on the next Update
				_isPlaying = true;//Switch to the Pause button next update
				_shouldUpdateTracks = true;//Regenerate tracks next update


			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Play: \n" + ex.ToString());
			}
		};

		vlcPlayer.mediaPlayer.Paused += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Paused: \n" + ex.ToString());
			}
		};

		vlcPlayer.mediaPlayer.Stopped += (object sender, EventArgs e) => {
			//Always use Try/Catch for VLC Events
			try
			{
				_isPlaying = false;//Switch to the Play button next update
				_shouldClearTracks = true;//Clear tracks next update
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception caught in mediaPlayer.Stopped: \n" + ex.ToString());
			}
		};



	}

	void Update()
	{
		//Update screen aspect ratio. Doing this every frame is probably more than is necessary.

		if(vlcPlayer.texture != null)
			screenAspectRatioFitter.aspectRatio = (float)vlcPlayer.texture.width / (float)vlcPlayer.texture.height;
	}



	//Enable a GameObject if it is disabled, or disable it if it is enabled
	bool ToggleElement(GameObject element)
	{
		bool toggled = !element.activeInHierarchy;
		element.SetActive(toggled);
		return toggled;
	}
}