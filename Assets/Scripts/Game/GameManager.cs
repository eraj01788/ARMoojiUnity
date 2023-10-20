using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Mapbox.Examples;
using Inworld;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEditor;
using System;
using GLTFast.Schema;

public class GameManager : MonoBehaviour
{
    public GameObject EmptyPanel,HomePanel, DropMapViewPanel, InWorlChatUI;
    public TMP_InputField messageTextMapDrop, messageTextCamDrop,messageTextSeeAll;
    string messageMapDrop, messageCamDrop,messageSeeAll;
    public GameObject selectedImoji,DropPrefab, ObjectToPlace,MultipleDropPrefab;
    SpawnOnMap onMap;
    public GameObject PlaceIndecator,ScanIns;
    public GameObject SelectedAI;
    public GameObject SelectedPrefab;
    GameObject selectedCheckMark;
    public AbstractMap _map;
    Vector2d initUserLocation;
    UserDataManager dataManager;
    string objectName = "";
    public List<GameObject> placable3dObject = new List<GameObject>();
    public List<MultipleDropPrefabManager> multipleDropPrefabManagers = new List<MultipleDropPrefabManager>();
    LoginManager loginManager;
    public List<GameObject> DropPrefabs = new List<GameObject>();
    public GameObject ARCamera,NonARCamera;

    private void Start()
    {
        onMap = FindObjectOfType<SpawnOnMap>();
        dataManager = FindObjectOfType<UserDataManager>();
        loginManager = FindObjectOfType<LoginManager>();
    }
    public void DropMessageFromFirebase(Placed3DObjectData newObject)
    {
        foreach (GameObject obj in placable3dObject)
        {
            if (obj.name == newObject.ObjectName)
            {
                selectedImoji = Instantiate(obj);
                ObjectToPlace = Instantiate(DropPrefab);
                selectedImoji.transform.parent = ObjectToPlace.transform;
                selectedImoji.transform.localPosition = Vector3.zero;
                ObjectToPlace.GetComponent<DropPrefabManager>().DropMessage.text = newObject.message;
                onMap.SpwanImoji(ObjectToPlace, newObject.ObjectLocation);
                newObject.RefObject = ObjectToPlace;
                CheckForNearbyObjects(newObject, ObjectToPlace);
                selectedImoji = null;
            }
        }
    }

    private void CheckForNearbyObjects(Placed3DObjectData newObject,GameObject objToPlace)
    {
        foreach (var existingObject in dataManager.placed3DObjects)
        {
            if (existingObject != newObject)
            {
                float distance = CalculateDistance(newObject.ObjectLocation, existingObject.ObjectLocation);

                if (distance <= dataManager.detectionDistance)
                {
                    if(!CheckForNearByMultipleDropPrefab(newObject))
                    {
                        CreateMultipleDropPrefab(existingObject, newObject);
                    }
                }
            }
        }
    }

    private bool CheckForNearByMultipleDropPrefab(Placed3DObjectData newObject)
    {
        bool hasMult = false;
        foreach (var existingMultDrop in multipleDropPrefabManagers)
        {
            float distance = CalculateDistance(newObject.ObjectLocation, existingMultDrop.location);

            if (distance <= dataManager.detectionDistance)
            {
                ExistingMultipleDropPrefab(existingMultDrop, newObject);
                hasMult = true;
                break;
            }
        }
        return hasMult;
    }

    void CreateMultipleDropPrefab(Placed3DObjectData existingObject, Placed3DObjectData newObject)
    {
        GameObject mdp = Instantiate(MultipleDropPrefab);
        MultipleDropPrefabManager mdpm = mdp.GetComponent<MultipleDropPrefabManager>();
        mdpm.location = newObject.ObjectLocation;
        GameObject up = Instantiate(mdpm.UserPanel, mdpm.content);
        UserPanelMultDropManager upmdm = up.GetComponent<UserPanelMultDropManager>();
        upmdm.UserNameText.text = newObject.userName;
        upmdm.UserMessageText.text = newObject.message;
        upmdm.ObjectToView = newObject.RefObject;
        upmdm.friendId = newObject.userId;
        upmdm.friendName = newObject.userName;
        upmdm.ObjectToView.SetActive(false);
        upmdm.mdpm = mdpm;
        multipleDropPrefabManagers.Add(mdpm);
        DropPrefabs.Add(upmdm.ObjectToView);
        onMap.SpwanImoji(mdp, newObject.ObjectLocation);
        if(!existingObject.isInsideMult)
        {
            GameObject upe = Instantiate(mdpm.UserPanel, mdpm.content);
            UserPanelMultDropManager upmdme = upe.GetComponent<UserPanelMultDropManager>();
            upmdme.UserNameText.text = existingObject.userName;
            upmdme.UserMessageText.text = existingObject.message;
            upmdme.ObjectToView = existingObject.RefObject;
            upmdme.friendId = existingObject.userId;
            upmdme.friendName = existingObject.userName;
            upmdme.ObjectToView.SetActive(false);
            upmdme.mdpm = mdpm;
            DropPrefabs.Add(upmdme.ObjectToView);
        }
    }

    void ExistingMultipleDropPrefab(MultipleDropPrefabManager mdpm,Placed3DObjectData newObject)
    {
        GameObject up = Instantiate(mdpm.UserPanel, mdpm.content);
        UserPanelMultDropManager upmdm = up.GetComponent<UserPanelMultDropManager>();
        upmdm.UserNameText.text = newObject.userName;
        upmdm.UserMessageText.text = newObject.message;
        upmdm.ObjectToView = newObject.RefObject;
        upmdm.friendId = newObject.userId;
        upmdm.friendName = newObject.userName;
        upmdm.ObjectToView.SetActive(false);
        upmdm.mdpm = mdpm;
        DropPrefabs.Add(upmdm.ObjectToView);
    }

    private float CalculateDistance(string location1, string location2)
    {
        // Example format for ObjectLocation: "latitude,longitude"

        // Split the location strings into latitude and longitude components
        string[] parts1 = location1.Split(',');
        string[] parts2 = location2.Split(',');

        if (parts1.Length != 2 || parts2.Length != 2)
        {
            // Invalid location format
            Debug.LogError("Invalid ObjectLocation format.");
            return float.MaxValue; // Return a large distance value to indicate an error.
        }

        // Parse latitude and longitude as doubles
        double lat1 = double.Parse(parts1[0]);
        double lon1 = double.Parse(parts1[1]);
        double lat2 = double.Parse(parts2[0]);
        double lon2 = double.Parse(parts2[1]);

        // Earth radius in meters (mean radius)
        double earthRadius = 6371000; // meters

        // Convert latitude and longitude from degrees to radians
        double dLat = Mathf.Deg2Rad * (float)(lat2 - lat1);
        double dLon = Mathf.Deg2Rad * (float)(lon2 - lon1);

        // Haversine formula
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(Mathf.Deg2Rad * (float)lat1) * Math.Cos(Mathf.Deg2Rad * (float)lat2) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = earthRadius * c;

        return (float)distance;
    }

    public void DropMessageMap()
    {
        messageMapDrop = messageTextMapDrop.text;
        if (selectedImoji!=null && messageMapDrop!=null)
        {
            Destroy(selectedCheckMark);
            messageTextMapDrop.text = "";
            Destroy(selectedImoji);
            dataManager.AddPlacedObject(objectName, GetLocation(), messageMapDrop);
            objectName = "";

        }
        else
        {
            messageTextMapDrop.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.15f, 1);
        }
    }

    public void DropMessageCam()
    {
        messageCamDrop = messageTextCamDrop.text;
        if (selectedImoji != null && messageCamDrop != null)
        {
            Destroy(selectedCheckMark);
            messageTextCamDrop.text = "";
            Destroy(selectedImoji);
            dataManager.AddPlacedObject(objectName, GetLocation(), messageCamDrop);
            objectName = "";
        }
        else
        {
            messageTextCamDrop.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.15f, 1);
        }
    }

    public void DropMessageSeeAll()
    {
        if(selectedImoji!=null)
        {
            messageSeeAll = messageTextSeeAll.text;
            if (selectedImoji != null && messageSeeAll != "")
            {
                Destroy(selectedImoji);
                dataManager.AddPlacedObject(objectName, GetLocation(), messageSeeAll);
                objectName = "";
                Destroy(selectedCheckMark);
                messageTextSeeAll.text = "";
                OpenARCamera();
                FindObjectOfType<PanelManager>().ShowPanel("EmptyPanel");
            }
            else
            {
                messageTextSeeAll.GetComponent<RectTransform>().DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.15f, 1);
            }
        }
    }

    public string GetLocation()
    {
        return GeoPlayer.GetLoc();
    }

    public void SelectAI(GameObject _AI)
    {
        OpenARCamera();
        SelectedAI = _AI;
        FindAnyObjectByType<TapToPlaceobject>().enabled = true;
        FindAnyObjectByType<TapToPlaceobject>().AI = SelectedAI;
        PlaceIndecator.SetActive(true);
        ScanIns.SetActive(true);
        FindAnyObjectByType<PanelManager>().ShowPanel("EmptyPanel");
    }

    public void SelectImoji(GameObject Imoji)
    {
        Destroy(selectedImoji);
        objectName = Imoji.name;
        selectedImoji = Instantiate(Imoji);
        onMap.PreviewEmoji(selectedImoji, GetLocation());
    }

    public void SelectedControl(RectTransform parent)
    {
        Destroy(selectedCheckMark);
        selectedCheckMark = Instantiate(SelectedPrefab, parent);
    }

    public void StartGame()
    {
        initUserLocation = Conversions.StringToLatLon(GeoPlayer.GetLoc());
        _map.Initialize(initUserLocation, 18);
        dataManager.StartGameAndPlaceObject();
    }
    public void ControllMapAndCamView(int zoomValue)
    {
        initUserLocation = Conversions.StringToLatLon(GeoPlayer.GetLoc());
        _map.Initialize(initUserLocation, zoomValue);
    }

    public void MakeSelectedNull()
    {
        if(SelectedAI != null)
        {
            SelectedAI = null;
        }
        if(selectedImoji!=null)
        {
            Destroy(selectedImoji);
            selectedImoji = null;
        }
        if (selectedCheckMark != null)
        {
            Destroy(selectedCheckMark);
            selectedCheckMark = null;
        }
    }

    public void BackToHome()
    {
        if (FindAnyObjectByType<TapToPlaceobject>().enabled)
        {
            FindAnyObjectByType<TapToPlaceobject>().DestroyObject();
            FindAnyObjectByType<TapToPlaceobject>().enabled = false;
            PlaceIndecator.SetActive(false);
            InWorlChatUI.SetActive(false);
            ScanIns.SetActive(false);
        }
    }

    public void OpenARCamera()
    {
        ARCamera.SetActive(true);
        NonARCamera.SetActive(false);
    }

    public void CloseARCamera()
    {
        ARCamera.SetActive(false);
        NonARCamera.SetActive(true);
    }

    public void OpenEmptyPanel()
    {
        FindAnyObjectByType<TapToPlaceobject>().DestroyObject();
        FindAnyObjectByType<TapToPlaceobject>().enabled = false;
        PlaceIndecator.SetActive(false);
        InWorlChatUI.SetActive(false);
        ScanIns.SetActive(false);
    }
}

