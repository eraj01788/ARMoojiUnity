namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
    using System.Collections.Generic;
    using System.Linq;

    public class SpawnOnMap : MonoBehaviour
    {
        [SerializeField]
		AbstractMap _map;

		[SerializeField]
		[Geocode]
		string[] _locationStrings;
        public List<Vector2d> _locations = new List<Vector2d>();
        Vector2d _userLocation;

        [SerializeField]
		float _spawnScale = 100f;

		[SerializeField]
		GameObject _markerPrefab;

		public List<GameObject> _spawnedObjects = new List<GameObject>();
		public int objectCount = 0;

        /*		void Start()
                {
                    _locations = new Vector2d[_locationStrings.Length];
                    _spawnedObjects = new List<GameObject>();
                    for (int i = 0; i < _locationStrings.Length; i++)
                    {
                        var locationString = _locationStrings[i];
                        _locations[i] = Conversions.StringToLatLon(locationString);
                        var instance = Instantiate(_markerPrefab);
                        instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
                        instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
                        _spawnedObjects.Add(instance);
                    }
                }*/


        public void SpwanImoji(GameObject ObjectToPlace,string userLocation)
        {
            _map._options.locationOptions.latitudeLongitude = userLocation;
            _userLocation = Conversions.StringToLatLon(_map._options.locationOptions.latitudeLongitude);
			ObjectToPlace.transform.localPosition = _map.GeoToWorldPosition(_userLocation, true);
			ObjectToPlace.SetActive(true);
			_spawnedObjects.Add(ObjectToPlace);
			_locations.Add(_userLocation);
        }

		public void PreviewEmoji(GameObject ObjectToPlace, string userLocation)
		{
            _map._options.locationOptions.latitudeLongitude = userLocation;
            _userLocation = Conversions.StringToLatLon(_map._options.locationOptions.latitudeLongitude);
            ObjectToPlace.transform.localPosition = _map.GeoToWorldPosition(_userLocation, true);
        }

		private void Update()
		{
			int count = _spawnedObjects.Count;
			for (int i = 0; i < count; i++)
			{
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}
	}
}