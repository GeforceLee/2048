using UnityEngine;
using System.Collections;

public class LocationManager : MonoBehaviour {
	[HideInInspector] 
	public bool isInit = false;
	[HideInInspector]
	public float latitude = 0;
	[HideInInspector]
	public float longitude = 0;	
	private static LocationManager s_instance = null;
	
	int maxWait = 20;

	
	LocationInfo currentGPSPosition;
	// Use this for initialization
	
	void Init() {
		if(!Input.location.isEnabledByUser)
			return;
		Input.location.Start();
		StartCoroutine(GetGPS());
	}
	
	public static LocationManager instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(LocationManager)) as LocationManager;
				if(s_instance!=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("LocationManager");
				s_instance = obj.AddComponent(typeof(LocationManager)) as LocationManager;
				s_instance.Init();
				Debug.Log("Could not locate an LocationManager object. LocationManager was Generated Automaticly");
			}
			return s_instance;
		}
	}
	
	IEnumerator GetGPS()
	{
		while(Input.location.status == LocationServiceStatus.Initializing && maxWait>0){
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if(maxWait<1){
			Debug.Log("Time Out");
			yield return null;
		}else{
			currentGPSPosition = Input.location.lastData;
			latitude = currentGPSPosition.latitude;
			longitude = currentGPSPosition.longitude;
			  print ("Location: " + currentGPSPosition.latitude + " " +
                   currentGPSPosition.longitude + " " +
                   currentGPSPosition.altitude + " " +
                   currentGPSPosition.horizontalAccuracy + " " +
                   currentGPSPosition.timestamp);
			isInit = true;
		}
	}
	
}
