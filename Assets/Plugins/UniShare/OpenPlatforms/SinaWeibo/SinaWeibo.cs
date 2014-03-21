using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;
/// <summary> 
/// SinaWeibo singleton class.
/// <para> mainpage: http://weibo.com/ </para>
/// <para> open platform: http://open.weibo.com/ </para>
/// <para> Implemented API: "statuses/update.json", "statuses/upload_url_text.json", "statuses/upload.json", "account/end_session.json" </para>
/// </summary> 
public class SinaWeibo : OpenPlatformBase {

	private static SinaWeibo s_instance = null;
	
	public readonly string GET_ACCESSTOKEN_URL = "https://api.weibo.com/oauth2/access_token";
	public readonly string API_STATUSES_UPDATE = "statuses/update.json";
	public readonly string API_STATUSES_UPLOAD  = "statuses/upload.json";
	public readonly string API_STATUSES_UPLOAD_URL_TEXT = "statuses/upload_url_text.json";
	public readonly string API_ACCOUNT_ENDSESSION = "account/end_session.json";
	
//	void Start()
//	{
//		Debug.Log("start child");
//	}
//	
	void Update()
	{
//		//Debug.Log("update child");
		if(resultList.Count>0){
			lock(resultList){
				foreach(var res in resultList){	
					if(res.commandType == GET_ACCESSTOKEN_URL){
						
						Debug.Log("Removed");
						if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
							OnAuthorizingResult(true);
						}else{
							OnAuthorizingResult(false);
						}
						resultList.Remove(res);
						//base.AuthCallback(res.description);
						break;
					}
				}
			}
		}
		base.Update();
		
	}
	
	// Use this for initialization
	void Init () {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_SINAWEIBO, "sinaweibo");
		isInited = true;
	}
	
	public static SinaWeibo instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(SinaWeibo)) as SinaWeibo;
				if(s_instance!=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("SinaWeibo");
				s_instance = obj.AddComponent(typeof(SinaWeibo)) as SinaWeibo;
				s_instance.Init();
				Debug.Log("Could not locate an SinaWeibo object. SinaWeibo was Generated Automaticly");
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
	/// post a status 
	/// </summary>
	/// <param name="status">content of the status, less than 140 characters. </param>
	/// <returns></returns>
	public override void Share(string status)
	{
		Share(status, 0, 0, "");
		
	}
	
	/// <summary>
	/// post a status with location info 
	/// </summary>
	/// <param name="status">content of the status, less than 140 characters.</param>
	/// <param name="lat">latitude from -90.0 to 90.0 default is 0.0</param>
	/// <param name="log">longitude from -180.0 to 180.0, default is 0.0 </param>
	/// <param name="annotations">custom infomation, json format, less than 512 characters</param>
	/// <returns>void</returns>
	public void Share(string status, float lat, float log, string annotations)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", status),
			new HttpParameter("lat", lat),
			new HttpParameter("long", log),
			new HttpParameter("annotations", annotations)
        };

		task.commandType = API_STATUSES_UPDATE;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			
			ResponseResult result = new ResponseResult();
			result.platformType = platformType;
			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			result.commandType = task.commandType;
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}		
	}
	
    /// <summary>
    /// post a status with uploading image 
    /// </summary>
    /// <param name="txt">content of status, less than 140 characters </param>
    /// <param name="pic">binary data of the uploading image, support format: JPEG、GIF、PNG, size limited: 5M </param>
    /// <returns></returns>
	public void ShareWithImage(string txt, byte[] pic){
		ShareWithLocalImage(txt, pic, 0, 0, "");
	}
	
	/// <summary>
    /// post a status with uploading image 
    /// </summary>
    /// <param name="txt">content of status, less than 140 characters </param>
    /// <param name="pic">binary data of the uploading image, support format: JPEG、GIF、PNG, size limited: 5M </param>
	/// <param name="lat">latitude from -90.0 to 90.0 default is 0.0</param>
	/// <param name="log">longitude from -180.0 to 180.0, default is 0.0 </param>
	/// <param name="annotations">custom infomation, json format, less than 512 characters</param> 
	/// <returns></returns>
	private void ShareWithLocalImage(string txt, byte[] pic, float lat, float log, string annotations){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", txt),
			new HttpParameter("lat", lat),
			new HttpParameter("long", log),
			new HttpParameter( "pic", pic),
			new HttpParameter("annotations", annotations)
        };
        task.commandType = API_STATUSES_UPLOAD;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){

			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_SINAWEIBO;
			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}
	}	
	
	/// <summary>
    /// post a status with uploading image 
    /// </summary>
    /// <param name="txt">content of status, less than 140 characters </param>
    /// <param name="imgPath">local path of the uploading image, support format: JPEG、GIF、PNG, size limited: 5M </param>
	/// <param name="lat">latitude from -90.0 to 90.0 default is 0.0</param>
	/// <param name="log">longitude from -180.0 to 180.0, default is 0.0 </param>
	/// <param name="annotations">custom infomation, json format, less than 512 characters</param> 
	/// <returns></returns>	
	
	private void ShareWithLocalImage(string txt, string imgPath, float lat, float log, string annotations){
		//read image
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
		ShareWithLocalImage(txt, pic, lat, log, annotations);
	}	
	
	/// <summary>
    /// post a status with uploading image 
    /// </summary>
    /// <param name="txt">content of status, less than 140 characters </param>
    /// <param name="imgPath">local path of the uploading image, support format: JPEG、GIF、PNG, size limited: 5M </param>
	/// <returns></returns>	
	private void ShareWithLocalImage(string txt, string imgPath){
		//read image
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
			Debug.Log("read file error");
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_SINAWEIBO;
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
			result.commandType = API_STATUSES_UPLOAD_URL_TEXT;
			result.description = "read file error";
			resultList.Add(result);
            return;
        }
		ShareWithLocalImage(txt, pic, 0, 0, "");
	}	
	/// <summary>
    /// post a status with image link 
    /// </summary>
    /// <param name="txt"> content of status, less than 140 characters </param>
    /// <param name="url"> url of image </param>
	/// <param name="lat"> latitude from -90.0 to 90.0 default is 0.0</param>
	/// <param name="log"> longitude from -180.0 to 180.0, default is 0.0 </param>
	/// <param name="annotations">custom infomation, json format, less than 512 characters</param> 
	/// <returns></returns>	
	private void ShareWithImageUrl(string txt, string url, float lat, float log, string annotations){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", txt),
			new HttpParameter("lat", lat),
			new HttpParameter("long", log),
			new HttpParameter( "url", url),
			new HttpParameter("annotations", annotations)
        };
        task.commandType = API_STATUSES_UPLOAD_URL_TEXT;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_SINAWEIBO;
			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}
	}	
	/// <summary>
    /// post a status with image link 
    /// </summary>
    /// <param name="txt"> content of status, less than 140 characters </param>
    /// <param name="url"> url of image </param>
	/// <returns></returns>		
	private void ShareWithImageUrl(string txt, string url){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", txt),
			new HttpParameter( "url", url)
        };
        task.commandType = API_STATUSES_UPLOAD_URL_TEXT;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		//check if accesstoken expired
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_SINAWEIBO;
			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			result.commandType = task.commandType.ToString();
			result.description = "invalid accessToken";
			resultList.Add(result);
		}else{
			SendCommand(task);
		}
	}		
	/// <summary>
    /// post a status with image link 
    /// </summary>
    /// <param name="txt"> content of status, less than 140 characters </param>
    /// <param name="imgPath"> local path or external url of image </param>
	/// <returns></returns>	
	public void ShareWithImage(string txt, string imgPath){
		if(imgPath.StartsWith("http")){
			ShareWithImageUrl(txt, imgPath);
		}else{
			ShareWithLocalImage(txt, imgPath);
		}
	}
	/// <summary>
    /// post a status with image link 
    /// </summary>
    /// <param name="txt"> content of status, less than 140 characters </param>
    /// <param name="imgPath"> local path or external url of image </param>
	/// <param name="lat"> latitude from -90.0 to 90.0 default is 0.0</param>
	/// <param name="log"> longitude from -180.0 to 180.0, default is 0.0 </param>
	/// <param name="annotations">custom infomation, json format, less than 512 characters</param> 	
	public void ShareWithImage(string txt, string imgPath, float lat, float log, string annotations){
		if(imgPath.StartsWith("http")){
			ShareWithImageUrl(txt, imgPath, lat, log, annotations);
		}else{
			ShareWithLocalImage(txt, imgPath, lat, log, annotations);
		}
	}	
	
//	public override void Logout(){
//		Task task;
//		task.commandType = API_ACCOUNT_ENDSESSION;
//		task.requestMethod = RequestMethod.Get;
//		task.parameters = new List<HttpParameter>();
//		SendCommand(task);
//	}
	
	private void GetAccessToken(string code){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("client_id", oauth.AppKey),
			new HttpParameter( "client_secret", oauth.AppSecret),
			new HttpParameter( "grant_type", "authorization_code"),
			new HttpParameter( "code", code),
			new HttpParameter( "redirect_uri", oauth.CallbackUrl)
        };
		task.commandType = GET_ACCESSTOKEN_URL;
		task.parameters = config;
		task.requestMethod = RequestMethod.Post;
		//check if accesstoken expired
		SendCommand(task);
	}
	
	public override void Authorize(){
		Logout();
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}", oauth.AuthorizeUrl, appKey, callbackUrl);
#elif UNITY_IPHONE||UNITY_ANDROID
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=code&redirect_uri={2}&display=mobile", oauth.AuthorizeUrl, appKey, callbackUrl);
#endif
		base.Authorize();
	}
	
	public void GetCodeCallback(string result){
		string[] tokenItem = result.Split('=');
		string code = tokenItem[1];
		GetAccessToken(code);
	}
	
	public void GetAccessTokenCallback(ResponseResult result){
		string description = result.description;
		Debug.Log("GetAccessTokenCallback" + description);
		if(!string.IsNullOrEmpty(description)&& (description.IndexOf("access_token")>=0)){
			var json = JsonReader.Deserialize<Dictionary<string, object>>(description);
			oauth.AccessToken = json["access_token"].ToString();		
			oauth.ExpiresIn = System.DateTime.Now.AddSeconds(System.Convert.ToDouble(json["expires_in"]));
			uid = json["uid"].ToString();
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
			base.HandleResponse(result);
			//base.AuthCallback(result);
		}else{
			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			base.HandleResponse(result);
		}
//		else{
//			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
//			base.HandleResponse(result);
//		}
//					{
//			       "access_token": "ACCESS_TOKEN",
//			       "expires_in": 1234,
//			       "remind_in":"798114",
//			       "uid":"12341234"
//			 		}		
		//access_token=2.008MiFvBbrROFBa8169df37fTa7yRB&remind_in=157550212&expires_in=157550212&uid=1759745531
	}
	
	public override void AuthCallback(string result){
		
		if(result=="UserCancel"){
			OnAuthorizingResult(false);
			return;
		}
		Debug.Log("AuthCallback" + result);
		GetCodeCallback(result);
	}
	
	protected override string SetupTask(Task task){
		string url = "";
		if(task.commandType == GET_ACCESSTOKEN_URL){
			url = task.commandType;
		}else{
			task.parameters.Add(new HttpParameter("access_token", oauth.AccessToken));
			url = oauth.BaseUrl + task.commandType;
		}
		return oauth.Request(url, task.requestMethod, task.parameters.ToArray());
	}
	
	protected override void HandleResponse(ResponseResult result){
		if(result.commandType == GET_ACCESSTOKEN_URL){
			GetAccessTokenCallback(result);
			return;
		}
		Debug.Log("ResponseResult : " + result.description);
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}else if(result.description == HTTPException.NETWORK_ERROR){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}
		else if(result.description.IndexOf("error_code")>=0){
			var json = JsonReader.Deserialize<Dictionary<string, object>>(result.description);
			string strErrorCode = json["error_code"].ToString();
			if(strErrorCode == "21301" 
				|| strErrorCode == "21314" 
				|| strErrorCode == "21315"
				|| strErrorCode == "21316"
				|| strErrorCode == "21317"
				|| strErrorCode == "21319"
				|| strErrorCode == "21327")
			{
				result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
				//Logout();
			}else{
				result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
			}
		}else{
			Debug.Log("ReturnType.RETURNTYPE_SUCCESS");
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
		}
		
		base.HandleResponse(result);
	}

}
