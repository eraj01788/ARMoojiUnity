using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARSelectedObject : MonoBehaviour
{
    GameObject selectedObject;
    Touch touch;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    selectedObject = hit.transform.gameObject;
                    selectedObject.GetComponent<DragAndRotate>().isActive = true;
                }
            }
        }

    }
}
