using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Android;

public class GeoPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public AbstractMap abstractMap;

    public float lastLatitude = 0f;
    public float lastLongitude = 0f;
    private float timeInterval = 0f;
    Vector2d userLocation;
    private static float lat, lon;
    private void Start()
    {
        // Check if device supports location services

        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // Request location permission
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location services not enabled on device");
            lat = lastLatitude;
            lon = lastLongitude;
            return;
        }

        // Start location services
        Input.location.Start();

        // Set the desired accuracy for GPS data
        Input.location.Start(1f, 1f);

        // Store the current time to calculate the time interval later
        timeInterval = Time.time;
    }

    private void Update()
    {
        // Check if GPS data is available
        if (Input.location.status == LocationServiceStatus.Running)
        {
            // Calculate the time interval between the current frame and the previous frame
            float elapsedTime = Time.time - timeInterval;
            timeInterval = Time.time;

            // Calculate the distance between the current position and the previous position
            float distance = Vector2.Distance(new Vector2(lastLatitude, lastLongitude),
                                              new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude));

            // Calculate the speed using the distance and time interval

            // Store the current latitude and longitude for the next frame
            lastLatitude = Input.location.lastData.latitude;
            lastLongitude = Input.location.lastData.longitude;
            abstractMap._options.locationOptions.latitudeLongitude = lastLatitude.ToString() + "," + lastLongitude.ToString();
            userLocation = Conversions.StringToLatLon(lastLatitude.ToString() + "," + lastLongitude.ToString());
            transform.localPosition = abstractMap.GeoToWorldPosition(userLocation, true);
            lat = lastLatitude;
            lon = lastLongitude;
        }
    }

    public static string GetLoc()
    {
        return lat.ToString() + "," + lon.ToString();
    }
    private void OnDestroy()
    {
        Input.location.Stop();
    }
}
