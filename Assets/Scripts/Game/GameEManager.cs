using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using TMPro;
using UnityEngine;

public class GameEManager : MonoBehaviour
{
    public GameObject HomePanel, DropMapView,DropCameraView,SeeAllPanel, OnBoardingPanel,InWorlChatUI;
    public GameObject indicator3D, indicatorInteractive, Object3DPanel, InteractiveObjectPanel;
    public TMP_Text indicatorText3D, indicatorTextInteractive, mediafilecount;

    public GameObject LoadingPanel;
    public AbstractMap _map;
    Vector2d initUserLocation;
    public GameObject ChatButton;

    public void ActivePanel(GameObject panel)
    {
        CloseAllPanel();
        panel.SetActive(true);
        if(FindAnyObjectByType<TapToPlaceobject>().enabled)
        {
            FindAnyObjectByType<TapToPlaceobject>().DestroyObject();
            FindAnyObjectByType<TapToPlaceobject>().enabled = false;
            InWorlChatUI.SetActive(false);
        }
    }

    public void CloseAllPanel()
    {
        mediafilecount.text = "10 Media file";
        HomePanel.SetActive(false);
        DropMapView.SetActive(false);
        DropCameraView.SetActive(false);
        SeeAllPanel.SetActive(false);
        OnBoardingPanel.SetActive(false);
        ChatButton.SetActive(false);
       

    }

    public void Open3DObjectPanel()
    {
        mediafilecount.text = "10 Media file";
        Object3DPanel.SetActive(true);
        InteractiveObjectPanel.SetActive(false);
        indicatorText3D.color = Color.white;
        indicatorTextInteractive.color = new Color(161, 161, 161, 255);
        indicator3D.SetActive(true);
        indicatorInteractive.SetActive(false);
        //messageController.selectedImoji = null;
        //messageController.__AI = null;
       

    }

    public void OpenInterativeObjectPanel()
    {
        mediafilecount.text = "8 Media file";
        Object3DPanel.SetActive(false);
        InteractiveObjectPanel.SetActive(true);
        indicatorText3D.color = new Color(161, 161, 161, 255);
        indicatorTextInteractive.color = Color.white;
        indicator3D.SetActive(false);
        indicatorInteractive.SetActive(true);
        //messageController.__AI = null;
        //messageController.selectedImoji = null;
       

    }

    public void StartGame()
    {
        initUserLocation = Conversions.StringToLatLon(GeoPlayer.GetLoc());
        _map.Initialize(initUserLocation, 15);
    }

    public void OpenSeeAll()
    {
        CloseAllPanel();
        ActivePanel(SeeAllPanel);
        //messageController.selectedImoji = null;
        //messageController.__AI = null;
    }

    public void OpenCameraView()
    {
        CloseAllPanel();
    }
}
