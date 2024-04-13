/// <author>
/// Jakub Komarek
/// </author>
/// <date>
/// 05.04.2024
/// </date>
/// <summary>
/// ovlada� k minimap� - nastavuje domovskou pozici, zomm a umo�nuje pohyb po map�
/// </summary>
using UnityEngine;
using Mapbox.Unity.Map;
using Microsoft.MixedReality.Toolkit.UI;
using Unity.Mathematics;


public class MapControler : MonoBehaviour
{
    void Start()
    {
        setCurentCenter();

        abstractMap.OnMapRedrawn += onMapRedrawn;
        abstractMap.OnUpdated += onMapLoaded;
    }

    private void onMapRedrawn()
    {
        isLoaded = false;
    }
    private void onMapLoaded()
    {
        isLoaded = true;
    }


    public void setCurentCenter()
    {
        initCenter = abstractMap.CenterLatitudeLongitude;
    }

    bool isLoaded = true;
    void Update()
    {
        if (isLoaded) // dopozicov�n� minimapy do �rovn� stolu
        {
            Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude; // referen�n� bod je st�ed mapy
            Vector3 wordPosOfCenter = abstractMap.GeoToWorldPosition(actualCenter, true);
            Vector3 tablePos = tableTransform.position;
            // rozd�l polohy stolu a mapy je ode�ten on pozice
            mapTransform.localPosition = new Vector3(0, mapTransform.localPosition.y + (tablePos.y - wordPosOfCenter.y), 0);
        }
    }

    private Mapbox.Utils.Vector2d initCenter;
    public AbstractMap abstractMap = null;
    public Transform mapTransform = null;
    public Transform tableTransform = null;

    private float sliderVal = 0.5f;
    private float zoom = 19f;

    private float moveStepSize = 0.0005f;

    public void setZoom(SliderEventData sliderEventData)
    {
        sliderVal = sliderEventData.NewValue;
        zoom = math.round(19f + sliderVal * 4f - 2f);

        try
        {
            abstractMap.UpdateMap(zoom);
        }
        catch { }


    }

    private float getStepSize()
    {
        return moveStepSize / mapTransform.localScale.x;
    }

    public void moveUp()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.y = actualCenter.y - getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }

    public void moveDown()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.y = actualCenter.y + getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }
    public void moveLeft()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.x = actualCenter.x + getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }
    public void moveRight()
    {
        Mapbox.Utils.Vector2d actualCenter = abstractMap.CenterLatitudeLongitude;
        actualCenter.x = actualCenter.x - getStepSize();

        abstractMap.UpdateMap(actualCenter, zoom);
    }
    public void center()
    {
        abstractMap.UpdateMap(initCenter, zoom);
        reloadMap();
    }

    public void reloadMap()
    {
        abstractMap.UpdateMap(14);
        abstractMap.UpdateMap(zoom);
    }

    public void changeMapImage(int index)
    {
        VectorSubLayerProperties vc = null;
        switch (index)
        {
            default:
            case 0:
                abstractMap.ImageLayer.SetLayerSource(ImagerySourceType.MapboxSatelliteStreet);

                vc = abstractMap.VectorData.FindFeatureSubLayerWithName("Buildings-side");
                vc.Texturing.SetStyleType(StyleTypes.Realistic);

                vc = abstractMap.VectorData.FindFeatureSubLayerWithName("Buildings-roof");
                vc.Texturing.SetStyleType(StyleTypes.Satellite);
                break;
            case 1:
                abstractMap.ImageLayer.SetLayerSource(ImagerySourceType.MapboxStreets);

                vc = abstractMap.VectorData.FindFeatureSubLayerWithName("Buildings-side");
                vc.Texturing.SetStyleType(StyleTypes.Light);

                vc = abstractMap.VectorData.FindFeatureSubLayerWithName("Buildings-roof");
                vc.Texturing.SetStyleType(StyleTypes.Light);

                break;
            case 2:
                abstractMap.ImageLayer.SetLayerSource(ImagerySourceType.MapboxDark);

                vc = abstractMap.VectorData.FindFeatureSubLayerWithName("Buildings-side");
                vc.Texturing.SetStyleType(StyleTypes.Dark);

                vc = abstractMap.VectorData.FindFeatureSubLayerWithName("Buildings-roof");
                vc.Texturing.SetStyleType(StyleTypes.Dark);
                break;
        }
    }
}
