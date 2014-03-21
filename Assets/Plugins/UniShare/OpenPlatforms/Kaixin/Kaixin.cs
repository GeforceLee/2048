using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;

/// <summary> 
/// Kaixin singleton class.
/// <para> mainpage: http://kaixin001.com </para>
/// <para> open platform: http://open.kaixin001.com/ </para>
/// <para> Implemented API: "records/add.json", "statuses/upload.json" </para>
/// </summary> 

public class Kaixin : OpenPlatformBase {

	private static Kaixin s_instance = null;
	
	public readonly string RECORD_ADD = "records/add.json";
	public readonly string STATUSES_UPLOAD  = "statuses/upload.json";

	// Use this for initialization
	void Init() {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_KAIXIN, "kaixin");
		isInited = true;
	}
	
	public static Kaixin instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Kaixin)) as Kaixin;
				if(s_instance!=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Kaixin");
				s_instance = obj.AddComponent(typeof(Kaixin)) as Kaixin;
				s_instance.Init();
				Debug.Log("Could not locate an Kaixin object. Kaixin was Generated Automaticly");
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
	/// post a record 
	/// </summary>
	/// <param name="content">content of the record, less than 140 characters.</param>
	/// <returns>void</returns>
	public override void Share(string content)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("content", content)
        };
		task.commandType = RECORD_ADD;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_KAIXIN;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType;
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}
	}
	
	
    /// <summary>
    /// post a record with uploading local images
    /// </summary>
    /// <param name="content">content of the record, less than 140 characters.</param>
    /// <param name="pic">binary data of the image you want to upload, supported format: jpg/jpeg/gif/png/bmp, size limited: 10M。</param>
    /// <returns></returns>
	public void ShareWithImage(string content, byte[] pic){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("content", content),
			new HttpParameter( "pic", pic)
        };
		task.commandType = RECORD_ADD;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//chek if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_KAIXIN;
			result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
			result.commandType = task.commandType;
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}
	}	
	
	/// <summary>
    /// post a record with uploading local images
    /// </summary>
    /// <param name="content">content of the record, less than 140 characters.</param>
    /// <param name="imgPath">path of the image you want to upload, support loacl path and external url. supported format:jpg/jpeg/gif/png/bmp, size limited: 10M</param>
    /// <returns></returns>
	public void ShareWithImage(string content, string imgPath){
		//external url
		if(imgPath.StartsWith("http")){
			Task task;
			List<HttpParameter> config = new List<HttpParameter>()
	        {
	            new HttpParameter("content", content),
				new HttpParameter( "picurl", imgPath)
	        };
			task.commandType = RECORD_ADD;
			task.requestMethod = RequestMethod.Post;
			task.parameters = config;
			
			if(!oauth.VerifierAccessToken()){
				ResponseResult result = new ResponseResult();
				result.platformType = PlatformType.PLATFORM_KAIXIN;
				result.returnType = ReturnType.RETURNTYPE_NEED_OAUTH;
				result.commandType = task.commandType;
				result.description = "invalid accessToken";
				resultList.Add(result);
			}else{
				SendCommand(task);
			}		
		}else{
			//if it is local image, read it first
			byte[] pic;
		    try
	        {
	            FileStream fs = new FileStream(imgPath, FileMode.Open);
	            BinaryReader sr = new BinaryReader(fs);
	            pic = sr.ReadBytes((int)fs.Length);
	            sr.Close();
	            fs.Close();
	        }
	        catch (Exception e)
	        {
	            return;
	        }
			ShareWithImage(content, pic);			
		}

	}
	
	public override void Authorize(){
		Logout();
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&scope=create_records&redirect_uri={2}", oauth.AuthorizeUrl, appKey, Utility.UrlEncode(callbackUrl));
#elif UNITY_IPHONE||UNITY_ANDROID
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&scope=create_records&redirect_uri={2}&oauth_client=1", oauth.AuthorizeUrl, appKey, Utility.UrlEncode(callbackUrl));
#endif		
		base.Authorize();
	}

	protected override void HandleResponse(ResponseResult result){
		
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}
		else if(result.description.IndexOf("error_code")>=0){
			var json = JsonReader.Deserialize<Dictionary<string, object>>(result.description);
			string strErrorCode = json["error_code"].ToString();
			if(strErrorCode == "401"){
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
