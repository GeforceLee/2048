using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using System.IO;
using UniShare.Json;
public class SampleGooglePlus : MonoBehaviour {
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
		GooglePlus.instance.OnCallBack += onUpdateReturn;
	}
	
	// Update is called once per frame
	void Update () {
		if(!shouldStopTimer)
			debugText.text = System.DateTime.Now.Millisecond.ToString();
	}
	
	void OnGUI(){
		//GooglePlus
		GUI.Label(new Rect(10, 100, 80, 60), "Google+");
		if(GUI.Button(new Rect(100, 100, 100, 60), "Authorize")){
			GooglePlus.instance.Authorize();
		}
		if(GUI.Button(new Rect(100, 180, 100, 60), "GetUserInfo")){
			GooglePlus.instance.GetUserInfo();
		}
		if(GUI.Button(new Rect(100, 260, 100, 60), "AddActivity")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			GooglePlus.instance.Share(text);
		}		
		if(GUI.Button(new Rect(100, 340, 100, 60), "Share")){
			GooglePlus.instance.ShareWithUrl("http://unisharekit.com");
		}
	}
	
	public void onUpdateReturn(ResponseResult res){
		shouldStopTimer = true;
		string logText = res.returnType.ToString();
		// if success
		if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
			if(res.commandType == GooglePlus.instance.PEOPLE_ME){
				var json = JsonReader.Deserialize<Dictionary<string, object>>(res.description);
				if(json != null){
					string userName = json["displayName"].ToString();
					logText = logText + " displayName: " + userName;
				}	
			}else if(res.commandType == GooglePlus.instance.MOMENTS_INSERT){
				var json = JsonReader.Deserialize<Dictionary<string, object>>(res.description);
				if(json != null){
					string strID = json["id"].ToString();
					logText = logText + " Activity id: " + strID;
				}					
			}
		}
		// if failed, display response error messgge
		else if(!string.IsNullOrEmpty(res.description)){
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
		GooglePlus.instance.OnCallBack -= onUpdateReturn;
	}
}

