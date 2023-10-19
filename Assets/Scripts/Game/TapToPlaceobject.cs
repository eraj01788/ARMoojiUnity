using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Inworld;

[RequireComponent(typeof(ARRaycastManager))]
public class TapToPlaceobject : MonoBehaviour
{
    public GameObject AI;
    public GameObject spawnedObject;
    private ARRaycastManager raycastManager;
    private ARPlaneManager ARPlaneManager;

    public GameObject PIND, ScanIns;
    private Vector2 touchPos;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPos)
    {
        if(Input.touchCount>0)
        {
            touchPos = Input.GetTouch(index: 0).position;
            return true;
        }
        touchPos = default;
        return false;
    }

    private void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPos))
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                var touchPosition = touch.position;



                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    var hitPose = hits[0].pose;
                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(AI, hitPose.position, hitPose.rotation);
                        spawnedObject.transform.GetChild(0).GetComponent<Animator>().enabled = true;
                        spawnedObject.transform.GetChild(0).GetComponent<InworldCharacter>().RegisterLiveSession();
                        PIND.SetActive(false);
                        ScanIns.SetActive(false);
                        FindAnyObjectByType<GameManager>().InWorlChatUI.SetActive(true);
                    }
                }
            }
        }
    }

    public void DestroyObject()
    {
        Destroy(spawnedObject);
    }
}
