using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;

/// <summary> 
/// Facebook singleton class.
/// <para> mainpage: http://www.facebook.com/ </para>
/// <para> open platform: http://developers.facebook.com/ </para>
/// <para> Implemented API: "status.set", "me/feed" </para>
/// </summary> 
public class Facebook : OpenPlatformBase {

	private static Facebook s_instance = null;
	
	public readonly string METHOD_STATUS_SET = "status.set";
	public readonly string METHOD_FEED = "me/feed";
	public readonly string METHOD_ME = "me";
	public readonly string METHOD_PHOTOS = "me/photos";
	string LIKE = "me/og.likes";
	// Use this for initialization
	void Init () {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_FACEBOOK, "facebook");
		isInited = true;
	}
	
	public static Facebook instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Facebook)) as Facebook;
				if(s_instance!=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Facebook");
				s_instance = obj.AddComponent(typeof(Facebook)) as Facebook;
				s_instance.Init();
				Debug.Log("Could not locate an Facebook object. Facebook was Generated Automaticly");
			}
			return s_instance;
		}
	}
	
	void OnApplicationQuit() {
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
	/// post to user's wall 
	/// </summary>
	/// <param name="message">The message to post.</param>
	/// <returns>void</returns>
	public override void Share(string message)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("message", message)
        };
		task.commandType = METHOD_FEED;
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
    /// post to user's wall with image link
    /// </summary>
    /// <param name="message">message </param>
    /// <param name="name">The name of the link</param>
    /// <param name="link">The link attached to this post</param>
    /// <param name="caption">The caption of the link (appears beneath the link name)</param>
	/// <param name="description">A description of the link (appears beneath the link caption)</param>
	/// <param name="picture">a linke to the picture</param>
	/// <returns>void</returns>
	
	public void ShareWithImage(string message, string name, string link, string caption, string description, string picture){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("message", message),
			new HttpParameter( "picture", picture),
			new HttpParameter( "name", name),
			new HttpParameter( "link", link),
			new HttpParameter( "caption", caption),
			new HttpParameter( "description", description)
        };
		task.commandType = METHOD_FEED;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}		
	}
	public void GetUserInfo()
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
//            new HttpParameter("fields", "name")
        };
		task.commandType = METHOD_ME;
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
			SendCommand(task);
		}
	}
	public void UploadPhoto(string message, byte[] source){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("message", message),
            new HttpParameter("source", source)
        };
		task.commandType = METHOD_PHOTOS;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_TWITTER;
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
	public void UploadPhoto(string message, string mediaPath){
		byte[] pic;
	    try
        {
            FileStream fs = new FileStream(mediaPath, FileMode.Open);
            BinaryReader sr = new BinaryReader(fs);
            pic = sr.ReadBytes((int)fs.Length);
            sr.Close();
            fs.Close();
        }
        catch (Exception e)
        {
            return;
        }
		UploadPhoto(message, pic);			
	}
	/// <summary>
	/// Likes the specified objID.
	/// </summary>
	/// <param name='objID'>
	/// Object ID to be liked.
	/// </param>
	public void Like(string objUrl)
	{
		LIKE = string.Format("me/og.likes");
		
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
			new HttpParameter("object", objUrl)
			
        };
		task.commandType = LIKE;
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
	
	public override void Authorize(){
		Logout();
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&redirect_uri={2}&scope=publish_stream,publish_actions,user_about_me", oauth.AuthorizeUrl, appKey, callbackUrl);
		base.Authorize();
		//Application.OpenURL(authUrl);
	}

	protected override string SetupTask(Task task){
		task.parameters.Add(new HttpParameter("format", "json"));
		return base.SetupTask(task);
	}	
	
	protected override void HandleResponse(ResponseResult result){
		Debug.Log("HandleResponse"+result.description);
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
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
					//Logout();
				}else{
					result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
				}
			}
		}else if(result.description.IndexOf("\"id\":")>=0){
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
		}
		base.HandleResponse(result);
	}	
}
