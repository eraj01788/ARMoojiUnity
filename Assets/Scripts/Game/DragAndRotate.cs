using UnityEngine;

public class DragAndRotate : MonoBehaviour
{
    private float initialDistance;
    private Vector3 initialScale;
    public bool activeRotate=false;
    public bool isActive=false;
    GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }
    void Update()
    {
        if(gameManager.EmptyPanel.activeSelf)
        {
            activeRotate = true;
        }
        else
        {
            activeRotate = false;
        }

        if (Input.touchCount == 1 && activeRotate && isActive)
        {
            Touch touchToRotate = Input.GetTouch(0);

            if (touchToRotate.phase == TouchPhase.Moved)
            {
                Quaternion rotationx = Quaternion.Euler(0f, -touchToRotate.deltaPosition.x * 20f * Time.deltaTime, 0f);
                transform.rotation = rotationx * transform.rotation;
            }

            if (touchToRotate.phase == TouchPhase.Ended)
            {
                isActive = false;
            }
        }

        if (Input.touchCount == 2 && activeRotate && isActive)
        {
            var touchZero = Input.GetTouch(0);
            var touchOne = Input.GetTouch(1);
            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
            {
                isActive = false;
                return;
            }

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = transform.localScale;
            }
            else
            {
                var currentDistance = Vector2.Distance(touchZero.position, touchOne.position);
                if (Mathf.Approximately(initialDistance, 0))
                {
                    return;
                }
                var factor = currentDistance / initialDistance;
                transform.localScale = initialScale * factor;
            }
        }
    }
}
