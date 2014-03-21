using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;

public class Foursquare : OpenPlatformBase {
	private static Foursquare s_instance = null;
	public readonly string METHOD_CHECKINS_ADD = "checkins/add";
	public readonly string METHOD_PHOTOS_ADD = "photos/add";
	public readonly string METHOD_USERS_SELF = "users/self";
	public readonly string METHOD_VENUES_EXPLORE = "venues/explore";
	// Use this for initialization
	void Init() {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_FOURSQUARE, "foursquare");
		//string res = "access_token=RAMJB3UTTDTANX3VQPSAOHPTAAVRRNV3LQP3Q2QNUOL10SKO";
		//AuthCallback(res);
		isInited = true;
	}
	
	public static Foursquare instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Foursquare)) as Foursquare;
				if(s_instance!=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Foursquare");
				s_instance = obj.AddComponent(typeof(Foursquare)) as Foursquare;
				s_instance.Init();
				Debug.Log("Could not locate an Foursquare object. Foursquare was Generated Automaticly");
			}
			return s_instance;
		}
	}
	protected void OnApplicationQuit() {
		if(oauth!=null){
			oauth.SavePlayerPrefers();
		}
		
		//oauth.SavePlayerPrefers();
		s_instance = null;
	}
	
	void OnDestroy() {
		if(oauth!=null){
			oauth.SavePlayerPrefers();
		}
		s_instance = null;
	}
	void OnApplicationPause() {
		if(oauth!=null){
			oauth.SavePlayerPrefers();
		}
		s_instance = null;
	}		
	/// <summary>
	/// post to user's wall 
	/// </summary>
	/// <param name="message">The message to post.</param>
	/// <returns>void</returns
	/// >
	public override void Share(string message)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("message", message)
        };
		task.commandType = "";
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			lock(resultList){
				resultList.Add(result);
			}
		}else{
			SendCommand(task);
		}
	}
	/// <summary>
	/// Get user info
	/// </summary>
	/// <returns>void</returns>	
	public void GetUserinfo()
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
        };
		task.commandType = METHOD_USERS_SELF;
		task.parameters = config;
		task.requestMethod = RequestMethod.Get;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			lock(resultList){
				resultList.Add(result);
			}
		}else{
			Debug.Log("in SendCommand");
			SendCommand(task);
		}
	}
	/// <summary>
	/// Explore venues near the given location 
	/// </summary>
	/// <param name="latitude">latitude of user's location</param>
	/// <param name="longitude">longitude of user's location</param>
	/// <returns>void</returns>	
	public void ExploreVenues(float latitude, float longitude)
	{
		ExploreVenues(latitude, longitude, 1000, 20);
	}
	/// <summary>
	/// Explore venues near the given location 
	/// </summary>
	/// <param name="latitude">latitude of user's location</param>
	/// <param name="longitude">longitude of user's location</param>
	/// <param name="radius">Radius to search within, in meters. If radius is not specified, a suggested radius will be used based on the density of venues in the area.</param>
	/// <param name="limit">Number of results to return, up to 50.</param>
	/// <returns>void</returns>		
	public void ExploreVenues(float latitude, float longitude, float radius, int limit)
	{
		Task task;
		latitude = 40.7f;
		longitude = -74f; 
		string ll = string.Format("{0},{1}", latitude, longitude);
		//Debug.Log(ll);
		//string section = "food";
		List<HttpParameter> config = new List<HttpParameter>()
        {
			new HttpParameter("ll", ll),
			new HttpParameter("radius", radius),
			new HttpParameter("section", "food"),
			new HttpParameter("limit", limit)
        };
		task.commandType = METHOD_VENUES_EXPLORE;
		task.parameters = config;
		task.requestMethod = RequestMethod.Get;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			lock(resultList){
				resultList.Add(result);
			}
		}else{
			Debug.Log("in SendCommand");
			SendCommand(task);
		}
	}
		
	/// <summary>
	/// Allows you to check in to a place.
	/// </summary>
	/// <param name="venueId">The venue where the user is checking in. Find venue IDs by call ExploreVenues</param>
	/// <param name="shout">A message about your check-in. The maximum length of this field is 140 characters.</param>
	/// <returns>void</returns>		
	public void Checkin(string venueId, string shout)
	{
		// add for test
		Debug.Log("in checkin");
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
			new HttpParameter("venueId", venueId),
            new HttpParameter("shout", shout)
        };
		task.commandType = METHOD_CHECKINS_ADD;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			lock(resultList){
				resultList.Add(result);
			}
		}else{
			Debug.Log("in SendCommand");
			SendCommand(task);
		}
	}

	
	/// <summary>
	/// Allows users to add a new photo to a checkin
	/// </summary>
	/// <param name="checkinId">the ID of a checkin owned by the user.</param>
	/// <param name="postText">Text for the photo post, up to 200 characters.</param>
	/// <param name="photoPath">local path to the photo.</param>
	/// <returns>void</returns>	
	public void AddPhoto(string checkinId, string postText, string photoPath)
	{
		//read image
		byte[] pic;
	    try
        {
            FileStream fs = new FileStream(photoPath, FileMode.Open);
            BinaryReader sr = new BinaryReader(fs);
            pic = sr.ReadBytes((int)fs.Length);
            sr.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            return;
        }		
		AddPhoto(checkinId, postText, pic);
	}
	
	/// <summary>
	/// Allows users to add a new photo to a checkin
	/// </summary>
	/// <param name="checkinId">the ID of a checkin owned by the user.</param>
	/// <param name="postText">Text for the photo post, up to 200 characters.</param>
	/// <param name="photo">binary data of the photo.</param>
	/// <returns>void</returns>		
	public void AddPhoto(string checkinId, string postText, byte[] photo)
	{
		// add for test
		Debug.Log("in checkin");
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
			new HttpParameter("checkinId", checkinId),
            new HttpParameter("postText", postText),
			new HttpParameter("photo", photo)
        };
		task.commandType = METHOD_PHOTOS_ADD;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			lock(resultList){
				resultList.Add(result);
			}
		}else{
			Debug.Log("in SendCommand");
			SendCommand(task);
		}
	}
	
	protected override string SetupTask(Task task){
		string url = oauth.BaseUrl;
		url = oauth.BaseUrl + task.commandType;
		task.parameters.Add(new HttpParameter("oauth_token", oauth.AccessToken));
		task.parameters.Add(new HttpParameter("v", System.DateTime.Today.ToString("yyyyMMdd")));
		return oauth.Request(url, task.requestMethod, task.parameters.ToArray());
		
		
	}	
	public override void Authorize(){
		Logout();
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&redirect_uri={2}", oauth.AuthorizeUrl, appKey, callbackUrl);
		base.Authorize();
	}
	
	public override void AuthCallback(string result){
		//access_token=RAMJB3UTTDTANX3VQPSAOHPTAAVRRNV3LQP3Q2QNUOL10SKO
		if(result == "UserCancel"){
			OnAuthorizingResult(false);
			return;
		}
		string[] keypairs = result.Split('&');
		if(keypairs.Length>=1){
			string[] tokenItem = keypairs[0].Split('=');
			oauth.AccessToken = tokenItem[1];			
			Debug.Log("AccessToken=" + oauth.AccessToken);
			OnAuthorizingResult(true);
		}else{
			OnAuthorizingResult(false);
		}
	}

//	protected override string SetupTask(Task task){
//		task.parameters.Add(new HttpParameter("format", "json"));
//		return base.SetupTask(task);
//	}	
	
	protected override void HandleResponse(ResponseResult result){
		Debug.Log("HandleResponse"+result.description);
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}else if(result.description.IndexOf("id")>=0){
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
		}
		else if(result.description.IndexOf("error")>=0){
		// error example
		//{"error":{"message":"An active access token must be used to query information about the current user.","type":"OAuthException","code":2500}}
			
			var json = JsonReader.Deserialize<Dictionary<string, object>>(result.description);
			if(json == null){
				result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
			}else{
				Dictionary<string, object> errorEntity = (Dictionary<string, object>)json["error"];
				string strErrorType = errorEntity["type"].ToString();
				if(strErrorType == "OAuthException"){
					result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
				}else{
					result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
				}
			}
		}
		base.HandleResponse(result);
	}			
}
