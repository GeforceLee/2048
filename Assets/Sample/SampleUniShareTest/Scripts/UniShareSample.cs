using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using System.IO;
using UniShare.Json;
public class UniShareSample : MonoBehaviour {
	public GUIText debugText;
	bool shouldStopTimer = false;
	string url = "http://unisharekit.com";
	string imgUrl = "http://ww4.sinaimg.cn/mw690/bd83d650gw1e1hlzfnjd1j.jpg";
	string imgPath = "testPicture.jpg";
	string template;
	// Use this for initialization
	void Start () { 
		imgPath = Application.persistentDataPath + "/" + imgPath; 
		if(!File.Exists(imgPath)){
			TextAsset bindata= Resources.Load("picture") as TextAsset;
			File.WriteAllBytes(imgPath, bindata.bytes);
		}
		template = "Post byUniShar(sharekit for unity)at{0}.https://www.assetstore.unity3d.com/#/search/unishare";
		Facebook.instance.OnCallBack += onUpdateReturn;
		Twitter.instance.OnCallBack += onUpdateReturn;
		SinaWeibo.instance.OnCallBack += onUpdateReturn;
		TencentWeibo.instance.OnCallBack += onUpdateReturn;
		Renren.instance.OnCallBack += onUpdateReturn;
		Kaixin.instance.OnCallBack += onUpdateReturn;
		Linkedin.instance.OnCallBack += onUpdateReturn;
		Foursquare.instance.OnCallBack += onUpdateReturn;
		GooglePlus.instance.OnCallBack += onUpdateReturn;
	}
	
	// Update is called once per frame
	void Update () {
		if(!shouldStopTimer)
			debugText.text = System.DateTime.Now.Millisecond.ToString();
	}
	
	void OnGUI(){
		//Facebook
		GUI.Label(new Rect(10, 50, 80, 60), "Facebook");
		if(GUI.Button(new Rect(100, 50, 100, 60), "Authorize")){
			Facebook.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 50, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Facebook.instance.Share(text);
//			Facebook.instance.GetUserInfo();
		}
		if(GUI.Button(new Rect(340, 50, 120, 60), "ShareWithImage")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Facebook.instance.ShareWithImage(text, "name", url, "caption", "description", imgUrl);
		}
		if(GUI.Button(new Rect(460, 50, 100, 60), "Logout")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Facebook.instance.Logout();
		}
		//SinaWeibo
		GUI.Label(new Rect(10, 120, 90, 60), "SinaWeibo");
		if(GUI.Button(new Rect(100, 120, 100, 60), "Authorize")){
			SinaWeibo.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 120, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			SinaWeibo.instance.Share(text);
		}
		if(GUI.Button(new Rect(340, 120, 120, 60), "ShareWithImage")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			SinaWeibo.instance.ShareWithImage(text, imgPath);
		}		
		//TencentWeibo
		GUI.Label(new Rect(10, 200, 100, 80), "TencentWeibo");
		if(GUI.Button(new Rect(100, 200, 100, 60), "Authorize")){
			TencentWeibo.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 200, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			TencentWeibo.instance.Share(text);
		}
		if(GUI.Button(new Rect(340, 200, 120, 60), "ShareWithImage")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			TencentWeibo.instance.ShareWithImage(text, imgPath);
		}			
		//Renren
		GUI.Label(new Rect(10, 280, 120, 60), "Renren");
		if(GUI.Button(new Rect(100, 280, 100, 60), "Authorize")){
			Renren.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 280, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Renren.instance.Share(text);
		}
		if(GUI.Button(new Rect(340, 280, 120, 60), "ShareWithImage")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Renren.instance.ShareWithImage("name", "description", url, text, imgUrl);
		}			
		//Kaixin
		GUI.Label(new Rect(10, 360, 80, 60), "Kaixin");
		if(GUI.Button(new Rect(100, 360, 100, 60), "Authorize")){
			Kaixin.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 360, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Kaixin.instance.Share(text);
		}
		if(GUI.Button(new Rect(340, 360, 120, 60), "ShareWithImage")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Kaixin.instance.ShareWithImage(text, imgPath);
		}			
		//Linkedin
		GUI.Label(new Rect(10, 440, 80, 60), "Linkedin");
		if(GUI.Button(new Rect(100, 440, 100, 60), "Authorize")){
			Linkedin.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 440, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Linkedin.instance.Share("comment", "title", "description", url, imgUrl);
		}
		//Twitter
		GUI.Label(new Rect(10, 520, 80, 60), "Twitter");
		if(GUI.Button(new Rect(100, 520, 100, 60), "Authorize")){
			Twitter.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 520, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			//add for test
			//string text = "(o iyfguu hvh)";
			Twitter.instance.Share(text);
		}
		if(GUI.Button(new Rect(340, 520, 100, 60), "ShareWithImage")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Twitter.instance.ShareWithImage(text, imgPath);
		}
		//Foursquare
		GUI.Label(new Rect(10, 600, 80, 60), "Foursquare");
		if(GUI.Button(new Rect(100, 600, 100, 60), "Authorize")){
			Foursquare.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 600, 100, 60), "Checkin")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Foursquare.instance.Checkin("4ce898b4e1eeb60c8784a4ae", "it works");
		}
		if(GUI.Button(new Rect(340, 600, 100, 60), "GetUserInfo")){
			Foursquare.instance.GetUserinfo();
		}		
		if(GUI.Button(new Rect(440, 600, 100, 60), "UploadPhoto")){
			//Foursquare.instance.AddPhoto("51690241e4b06b0b72f4f4ac", "this is a picture", imgPath);
			Foursquare.instance.ExploreVenues(40.7f, -74f);
			//51690241e4b06b0b72f4f4ac
		}	
		//GooglePlus
		GUI.Label(new Rect(10, 680, 80, 60), "Google+");
		if(GUI.Button(new Rect(100, 680, 100, 60), "Authorize")){
			GooglePlus.instance.Authorize();
		}
		if(GUI.Button(new Rect(220, 680, 100, 60), "Share")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			//GooglePlus.instance.Share("text");
			GooglePlus.instance.ShareWithUrl("http://unisharekit.com");
		}		
		if(GUI.Button(new Rect(340, 680, 100, 60), "GetUserInfo")){
			GooglePlus.instance.GetUserInfo();
		}
	}
	
	public void onUpdateReturn(ResponseResult res){
		shouldStopTimer = true;
		string logText = res.returnType.ToString();
		if(!string.IsNullOrEmpty(res.description)){
			if(res.description.Length>100){
				logText = logText + res.description.Substring(0, 100);
			}else{
				logText = logText + res.description;
			}
		}
		debugText.text = logText;
		Debug.Log(res.returnType.ToString());
		Debug.Log(res.description);
	}
	
	void OnDestroy()
	{
		Facebook.instance.OnCallBack -= onUpdateReturn;
		Twitter.instance.OnCallBack -= onUpdateReturn;
		SinaWeibo.instance.OnCallBack -= onUpdateReturn;
		TencentWeibo.instance.OnCallBack -= onUpdateReturn;
		Renren.instance.OnCallBack -= onUpdateReturn;
		Kaixin.instance.OnCallBack -= onUpdateReturn;
		Linkedin.instance.OnCallBack -= onUpdateReturn;
		Foursquare.instance.OnCallBack -= onUpdateReturn;
		GooglePlus.instance.OnCallBack -= onUpdateReturn;
	}	
//	public void onGetUserInfo(ResponseResult res){
//		shouldStopTimer = true;
//		debugText.text = res.returnType.ToString();
//		
//		Debug.Log(res.returnType.ToString());
//		Debug.Log(res.description);
//		if(res.returnType == ReturnType.RETURNTYPE_SUCCESS && res.commandType == Facebook.instance.METHOD_ME){
//			//parse the json
//			
//			var json = JsonReader.Deserialize<Dictionary<string, object>>(res.description);
//			//print user info
//			Debug.Log(json["name"]);
//			foreach (KeyValuePair<string ,object> item in json){
//				string keyvalue = string.Format("{0}:{1}", item.Key.ToString(), item.Value.ToString());
//				Debug.Log(keyvalue);
//			}
//		}
//	}
}
