using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using System.IO;
using UniShare.Json;
public class SampleFoursquare : MonoBehaviour {
	public GUIText debugText;
	bool shouldStopTimer = false;
	string url = "http://unisharekit.com";
	string imgUrl = "http://ww4.sinaimg.cn/mw690/bd83d650gw1e1hlzfnjd1j.jpg";
	string imgPath = "testPicture.jpg";
	string template;
	string currentCheckID = "";
	string userID;
	string firstName;
	string lastName;
	enum GameState{
		GameStateAuthorize,
		GameStateExplore,
		GameStateCheckin,
		GameStateUploadPhoto
	}
	GameState gameState = GameState.GameStateExplore;
	
	List<Dictionary<string, string>> venueList = new List<Dictionary<string, string>>();
	// Use this for initialization
	void Start () { 
		imgPath = Application.persistentDataPath + "/" + imgPath; 
		TextAsset bindata= Resources.Load("picture") as TextAsset;
		File.WriteAllBytes(imgPath, bindata.bytes);
		template = "Post byUniShar(sharekit for unity)at{0}.https://www.assetstore.unity3d.com/#/search/unishare";
		Foursquare.instance.OnCallBack += onUpdateReturn;
		Foursquare.instance.OnOauthCallBack += onAuthCallback;
	}
	
	// Update is called once per frame
	void Update () {
		if(!shouldStopTimer)
			debugText.text = System.DateTime.Now.Millisecond.ToString();
	}
	
	void OnGUI(){
		//Foursquare
		GUI.Label(new Rect(400, 20, 80, 40), "Foursquare");

		if(gameState == GameState.GameStateAuthorize){
			if(GUI.Button(new Rect(100, 100, 100, 60), "Authorize")){
				Foursquare.instance.Authorize();
			}
			if(GUI.Button(new Rect(100, 180, 100, 60), "GetUserInfo")){
				Foursquare.instance.GetUserinfo();
			}			
		}else if(gameState == GameState.GameStateExplore){
			if(GUI.Button(new Rect(100, 100, 120, 60), "Explore Venues")){
				Foursquare.instance.ExploreVenues(LocationManager.instance.latitude, LocationManager.instance.longitude);
				// if there's no venue nearby and you want to see a demo, use line below
				//Foursquare.instance.ExploreVenues(40.7f, -74f);
			}
		}
		else if(gameState == GameState.GameStateCheckin){
			if(venueList.Count>0){
				GUI.Label(new Rect(200, 70, 300, 40), "venues nearby:(click to checkin)");
				for(int i=0; i<venueList.Count; i++){
					var venue = venueList[i];
					if(GUI.Button(new Rect(300, 130+40*i, 180, 30), venue["name"])){
						string text = string.Format(template, System.DateTime.Now.ToString());
						Foursquare.instance.Checkin(venue["id"], text);
					}				
				}
			}			
		}else if(gameState == GameState.GameStateUploadPhoto){
			if(GUI.Button(new Rect(100, 100, 120, 60), "Add photo")){
				string text = "upload" + string.Format(template, System.DateTime.Now.ToString());
				Foursquare.instance.AddPhoto(currentCheckID, text, imgPath);
			}

		}
		if(gameState > GameState.GameStateAuthorize){
			if(GUI.Button(new Rect(50, 20, 40, 30), "Back")){
				gameState = gameState - 1;
			}
		}
		
	}
	
	public void onAuthCallback(PlatformType pType, bool success){
		gameState = GameState.GameStateExplore;
	}
	
	public void onUpdateReturn(ResponseResult res){
		shouldStopTimer = true;
		string logText = res.returnType.ToString();
		//debugText.text = "get response";
		// if success
		Debug.Log("commond type:" + res.commandType);
		if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
			if(res.commandType == Foursquare.instance.METHOD_USERS_SELF){
				var json = JsonReader.Deserialize<Dictionary<string, object>>(res.description);
				if(json != null){
					var responseObject = (Dictionary<string, object>)json["response"];
					var user = (Dictionary<string, object>)responseObject["user"];
					userID = user["id"].ToString();
					firstName = user["firstName"].ToString();
					lastName = user["lastName"].ToString();
					logText = logText + " firstName: " + firstName + " lastName: " + lastName;
				}	
			}else if(res.commandType == Foursquare.instance.METHOD_VENUES_EXPLORE){
				venueList.Clear();
				Debug.Log("METHOD_VENUES_EXPLORE");
				var json = JsonReader.Deserialize<Dictionary<string, object>>(res.description);
				if(json != null){
					var responseObject = (Dictionary<string, object>)json["response"];
					var groups = (Dictionary<string, object>[])responseObject["groups"];
					foreach(var aGroup in groups){
						var items = (Dictionary<string, object>[])aGroup["items"];
						foreach(var item in items){
							var venue = (Dictionary<string, object>)item["venue"];
							Dictionary<string, string> venueDic = new Dictionary<string, string>();
							venueDic.Add("id", venue["id"].ToString());
							venueDic.Add("name", venue["name"].ToString());
							venueList.Add(venueDic);
							logText += " | " + venue["id"].ToString() + ", " + venue["name"].ToString();
						}
					}
		
					
					//logText = logText + " Activity id: " + strID;
				}
				if(venueList.Count>0){
					gameState = GameState.GameStateCheckin;	
				}
			}else if(res.commandType == Foursquare.instance.METHOD_CHECKINS_ADD){
				var json = JsonReader.Deserialize<Dictionary<string, object>>(res.description);
				if(json != null){
					var responseObject = (Dictionary<string, object>)json["response"];
					var checkin = (Dictionary<string, object>)responseObject["checkin"];
					currentCheckID = checkin["id"].ToString();
					logText = logText + " checkID: " + currentCheckID;
				}
				gameState = GameState.GameStateUploadPhoto;
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
		Foursquare.instance.OnCallBack -= onUpdateReturn;
		Foursquare.instance.OnOauthCallBack -= onAuthCallback;	
	}
}

