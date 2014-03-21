using UnityEngine;
using System.Collections;

public class QZoneTest : MonoBehaviour {
	string template;
	// Use this for initialization
	void Start () {
		template = "Post byUniShar(sharekit for unity) at {0}.https://www.assetstore.unity3d.com/#/search/unishare";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnGUI(){
		//Facebook
		if(GUI.Button(new Rect(100, 50, 100, 60), "Authorize")){
			QZone.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 50, 100, 60), "ShareWithImage")){
			string shareText =  string.Format(template, System.DateTime.Now.ToString());
			
			QZone.instance.Share("share", "http://www.baidu.com", shareText);
		}
	}
}
