using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UniShare;
using UniShare.Json;

/// <summary> 
/// Renren singleton class.
/// <para> mainpage: http://renren.com/ </para>
/// <para> open platform: http://dev.renren.com/ </para>
/// <para> Implemented API: "status.set", "feed.publishFeed", "share.share" </para>
/// </summary> 
public class Renren : OpenPlatformBase {

	private static Renren s_instance = null;
	
	public readonly string METHOD_STATUS_SET = "status/put";
	public readonly string METHOD_FEED_PUBLISH = "feed/put";
	public readonly string METHOD_SHARE_SHARE  = "share.share";

	// Use this for initialization
	void Init() {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_RENREN, "renren");
		isInited = true;
	}
	
	public static Renren instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Renren)) as Renren;
				if (s_instance != null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Renren");
				s_instance = obj.AddComponent(typeof(Renren)) as Renren;
				s_instance.Init();
				Debug.Log("Could not locate an Renren object. Renren was Generated Automaticly");
			}
			
			return s_instance;
		}
	}
	
	protected void OnApplicationQuit() {
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
	void OnDestroy() {
		if(oauth!=null){
			oauth.SavePlayerPrefers();
		}
		s_instance = null;
	}
	
    /// <summary>
    /// generate signature from param
    /// </summary>
    /// <param name="paras">all of params needed to generate signature </param>
    /// <returns></returns>
    string CalSig(List<HttpParameter> paras)
    {
        paras.Sort(new HttpParameterComparer());
        StringBuilder sbList = new StringBuilder();
        foreach (HttpParameter para in paras)
        {
            sbList.AppendFormat("{0}={1}", para.Name, para.Value);
        }
        sbList.Append(appSecret);
        return Utility.MD5Encrpt(sbList.ToString());
    }	
	
	/// <summary>
	/// post a status 
	/// </summary>
	/// <param name="status">content of the status, less than 140 characters. </param>
	/// <returns></returns>
	public override void Share(string status)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("content", status),
			//new HttpParameter("method", "status.set")
        };
		task.commandType = METHOD_STATUS_SET;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			Debug.Log("!oauth.VerifierAccessToken()");
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_RENREN;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType;
			result.description = "invalid accessToken";
			lock(resultList){
				resultList.Add(result);
			}
		}else{
			SendCommand(task);
		}			
		
	}

	/// <summary>
	/// post a feed
	/// </summary>
	/// <param name="name">name of the feed </param>
	/// <param name="description"> description of the feed </param>
	/// <param name="url"> url of the feed </param>
	/// <param name="message"> message of the feed, less than 200 characters. </param>
	/// <returns></returns>	
	public void Share(string name, string description, string url, string message)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("name", name),
			new HttpParameter("description", description),
			new HttpParameter("url", url),
			new HttpParameter("message", message),
			//new HttpParameter("method", "feed.publishFeed"),
        };
		task.commandType = METHOD_FEED_PUBLISH;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_RENREN;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType;
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}			
	}	
	
	/// <summary>
	/// post a feed with image
	/// </summary>
	/// <param name="name">name of the feed </param>
	/// <param name="description"> description of the feed </param>
	/// <param name="url"> url of the feed </param>
	/// <param name="message"> message of the feed, less than 200 characters. </param>
	/// <param name="image">url of image share in the feed </param>
	/// <returns></returns>	
	public void ShareWithImage(string name, string description, string url, string message, string image)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("title", name),
			new HttpParameter("description", description),
			new HttpParameter("targetUrl", url),
			new HttpParameter("message", message),
			//new HttpParameter("method", "feed.publishFeed"),
			new HttpParameter("imageUrl", image)
        };
		task.commandType = METHOD_FEED_PUBLISH;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_RENREN;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType;
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}			
	}
	

	public override void Authorize(){
		Logout();
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&scope=publish_feed&redirect_uri={2}", oauth.AuthorizeUrl, appKey, callbackUrl);
#elif UNITY_IPHONE||UNITY_ANDROID
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&scope=status_update&redirect_uri={2}&display=touch", oauth.AuthorizeUrl, appKey, callbackUrl);
#endif		
		base.Authorize();
	}

	public override void AuthCallback(string result){
		if(result == "UserCancel" || string.IsNullOrEmpty(result)){
			OnAuthorizingResult(false);
			return;
		}
		//access_token=USER_ACCESS_TOKEN&expires_in=NUMBER_OF_SECONDS_UNTIL_TOKEN_EXPIRES
		string[] keypairs = result.Split('&');
		if(keypairs.Length>=2){
			string[] tokenItem = keypairs[0].Split('=');
			string accessToken = tokenItem[1];
			oauth.AccessToken = UrlDecoder.UrlDecode(accessToken, Encoding.UTF8);
			
			string[] expireInItem = keypairs[1].Split('=');
			oauth.ExpiresIn = System.DateTime.Now.AddSeconds(System.Convert.ToDouble(expireInItem[1]));
			OnAuthorizingResult(true);
		}else{
			OnAuthorizingResult(false);
		}
	}
	
//	protected override string SetupTask(Task task){
////		task.parameters.Add(new HttpParameter("v", "1.0"));
////		task.parameters.Add(new HttpParameter("format", "JSON"));
//		task.parameters.Add(new HttpParameter("method", task.commandType));
//		task.parameters.Add(new HttpParameter("access_token", oauth.AccessToken));
////		task.parameters.Add(new HttpParameter("sig", CalSig(task.parameters)));
//		string url = oauth.BaseUrl;
//		url = oauth.BaseUrl;
//		
//		return oauth.Request(url, task.requestMethod, task.parameters.ToArray());
//	}

	protected override void HandleResponse(ResponseResult result){
		
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}
		var json = JsonReader.Deserialize<Dictionary<string, object>>(result.description);
//		if(json.ContainsKey("result")&&json["result"]=="1"){
//			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
//		}
//		else 
		if(json.ContainsKey("error_code")){
		
			string strErrorCode = json["error_code"].ToString();
			if(strErrorCode=="104"
				||strErrorCode=="105"
				||strErrorCode=="2000"
				||strErrorCode=="2001"
				||strErrorCode=="2002"){
				result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
				//Logout();
			}else
				result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}else{
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
		}		
		
		base.HandleResponse(result);
	}	
}
