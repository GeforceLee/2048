using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;

public class GooglePlus : OpenPlatformBase {
	private static GooglePlus s_instance = null;

	public readonly string MOMENTS_INSERT = "people/me/moments/vault";
	public readonly string PEOPLE_ME =  "people/me";
	
	const string SCOPE_LOGIN = "https://www.googleapis.com/auth/plus.login";
	const string SCOPE_ME = "https://www.googleapis.com/auth/plus.me";
	const string SHARE_URL = "https://plus.google.com/share?url=";
	void Init () {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_GOOGLEPLUS, "googleplus");
		isInited = true;
	}	
	
	public static GooglePlus instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(GooglePlus)) as GooglePlus;
				if(s_instance!=null){
					s_instance.Init();
				}
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("GooglePlus");
				s_instance = obj.AddComponent(typeof(GooglePlus)) as GooglePlus;
				s_instance.Init();
				Debug.Log("Could not locate an GooglePlus object. GooglePlus was Generated Automaticly");
			}
			return s_instance;
		}
	}

	
	public override void Share(string message)
	{
//		{
//  "type":"http://schemas.google.com/AddActivity",
//  "target":{
//      "url":"http://www.qontext.com"
//  }
//}		
		string url = "http://unisharekit.com";
		Dictionary<string, object> target = new Dictionary<string, object>();
		target.Add("url", url);
		Dictionary<string, object> share = new Dictionary<string, object>();
		string momentType = "http://schemas.google.com/AddActivity";
		
		share.Add("type", momentType);
		share.Add("target", target);
		string shareContentString = JsonWriter.Serialize(share);
		
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
			//new HttpParameter("message", "test")
            new HttpParameter("message", shareContentString)
        };
		Debug.Log("json:" + shareContentString);
		task.commandType = MOMENTS_INSERT;
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
	public void GetUserInfo(){
		Task task;
		task.commandType = PEOPLE_ME;
		task.requestMethod = RequestMethod.Get;
		task.parameters = null;
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
	/// Share with url using share link https://plus.google.com/share?url=[url]
	/// </summary>
	/// <param name="url">url to be share</param>
	/// <returns>void</returns>		
	public void ShareWithUrl(string url)
	{
		string shareUrl = SHARE_URL+Uri.EscapeDataString(url);
		OauthWebView.instance.OpenURL(platformType, shareUrl);
	}
	
	public override void Authorize(){
		Logout();
		//string encodedScope = Uri.EscapeDataString(SCOPE_ME);
		string encodedScope = Uri.EscapeDataString(SCOPE_LOGIN);
		string redirectUrl = Uri.EscapeDataString(callbackUrl);
		string requestAction = Uri.EscapeDataString("http://schemas.google.com/AddActivity");
		fullAuthorizeUrl = string.Format("{0}?request_visible_actions={4}&redirect_uri={1}&response_type=token&client_id={2}&approval_prompt=force&scope={3}", oauth.AuthorizeUrl, redirectUrl, appKey, encodedScope, requestAction);
		base.Authorize();
	}
	
	protected override string SetupTask(Task task){
		string url = oauth.BaseUrl;
		url = oauth.BaseUrl + task.commandType;
		string parameString = "";
		if(task.parameters!=null && task.parameters[0]!=null){
			parameString = task.parameters[0].Value.ToString();
		}
		return oauth.Request(url, task.requestMethod, parameString);
		
	}
	
	protected override void HandleResponse(ResponseResult result){
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}
		else {
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
		}
		
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
				string strErrorCode = errorEntity["code"].ToString();
				if(strErrorCode == "401"){
					result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
					// clean accesstoken
					oauth.AccessToken = "";
				}else{
					result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
				}
			}
		}		
		
		
		base.HandleResponse(result);
	}	
	
	protected void OnApplicationQuit() {
		if(oauth!=null){
			oauth.SavePlayerPrefers();
		}
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
}
