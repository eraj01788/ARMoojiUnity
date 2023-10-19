using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonEventDispatcher : MonoBehaviour
{
    public Camera mapCamera;

    public RenderTexture mapCameraRenderTexture;

    private void Update()
    {
        // Check for input or event from map camera
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider != null)
                {
                    GameObject clickedObject = hit.collider.gameObject;

                    // Check if the clicked object is the button
                    if (clickedObject.CompareTag("ProfileBtn"))
                    {
                        Debug.Log("Button Clicked from Map Camera");
                        // Handle button click logic here
                    }
                }
            }
        }
    }
}
