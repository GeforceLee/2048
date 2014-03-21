using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UniShare;
using UniShare.Json;

/// <summary> 
/// Renren singleton class.
/// <para> mainpage: http://renren.com/ </para>
/// <para> open platform: http://dev.renren.com/ </para>
/// <para> Implemented API: "status.set", "feed.publishFeed", "share.share" </para>
/// </summary> 
public class QZone : OpenPlatformBase {

	private static QZone s_instance = null;
	
	public readonly string METHOD_STATUS_SET = "status/put";
	public readonly string METHOD_FEED_PUBLISH = "feed/put";
	public readonly string METHOD_SHARE_SHARE  = "share/add_share";
	public readonly string METHOD_GET_OPENID  = "oauth2.0/me";
	// Use this for initialization
	void Init() {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_QZONE, "QZone");
		isInited = true;
	}
	
	public static QZone instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(QZone)) as QZone;
				if (s_instance != null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("QZone");
				s_instance = obj.AddComponent(typeof(QZone)) as QZone;
				s_instance.Init();
				Debug.Log("Could not locate an QZone object. QZone was Generated Automaticly");
			}
			
			return s_instance;
		}
	}
	
	void Update()
	{
//		//Debug.Log("update child");
		if(resultList.Count>0){
			lock(resultList){
				foreach(var res in resultList){	
					if(res.commandType == METHOD_GET_OPENID){
						
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
	
	private void GetOpenID(string accessToekn){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("access_token", accessToekn),
        };
		task.commandType = METHOD_GET_OPENID;
		task.parameters = config;
		task.requestMethod = RequestMethod.Get;
		//check if accesstoken expired
		SendCommand(task);
	}
	
	public void GetOpenIDCallback(ResponseResult result){
		string description = result.description;
		Debug.Log("GetOpenIDCallback" + description);
		if(!string.IsNullOrEmpty(description)&& (description.IndexOf("openid")>=0)){
			int index = result.description.IndexOf("callback(");
			if(index>=0){
				string jsontxt = result.description.Substring(9, result.description.Length-10);
				var json = JsonReader.Deserialize<Dictionary<string, object>>(jsontxt);
				oauth.OpenID = json["openid"].ToString();
				result.returnType = ReturnType.RETURNTYPE_SUCCESS;
			}else{
				result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			}
		
			
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
			base.HandleResponse(result);
			//base.AuthCallback(result);
		}else{
			result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
			base.HandleResponse(result);
		}
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
	/// post a feed
	/// </summary>
	/// <param name="title">feeds的标题,最长36个中文字，超出部分会被截断。 </param>
	/// <param name="url"> 分享所在网页资源的链接，点击后跳转至第三方网页 </param>
	/// <param name="comment"> 用户评论内容，也叫发表分享时的分享理由, 最长40个中文字，超出部分会被截断。 </param>
	/// <returns></returns>	
	public void Share(string title, string url, string comment)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("title", title),
			new HttpParameter("url", url),
//			new HttpParameter("format", "json"),
			new HttpParameter("comment", comment),
//			new HttpParameter("site", "UniSocial"),
//			new HttpParameter("fromurl", "http://unisharekit.com"),
			//new HttpParameter("method", "feed.publishFeed"),
        };
		task.commandType = METHOD_SHARE_SHARE;
		task.requestMethod = RequestMethod.Get;
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
	/// post a feed
	/// </summary>
	/// <param name="title">feeds的标题,最长36个中文字，超出部分会被截断。 </param>
	/// <param name="url"> 分享所在网页资源的链接，点击后跳转至第三方网页 </param>
	/// <param name="comment"> 用户评论内容，也叫发表分享时的分享理由, 最长40个中文字，超出部分会被截断。 </param>
	/// <param name="images"> 所分享的网页资源的代表性图片链接,长度限制255字符 </param>
	/// <param name="nswb"> 值为1时，表示分享不默认同步到微博，其他值或者不传此参数表示默认同步到微博。 </param>
	/// <returns></returns>	
	public void Share(string title, string url, string comment, string images, string nswb)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("title", title),
			new HttpParameter("url", url),
			new HttpParameter("comment", comment),
			new HttpParameter("images", images),
			new HttpParameter("nswb", nswb),
        };
		task.commandType = METHOD_SHARE_SHARE;
		task.requestMethod = RequestMethod.Get;
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
		fullAuthorizeUrl = string.Format("{0}?client_id={1}&scope=get_user_info,list_album,add_share&response_type=token&redirect_uri={2}", oauth.AuthorizeUrl, appKey, callbackUrl);
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
		//fullAuthorizeUrl = string.Format("{0}?client_id={1}&scope=get_user_info,list_album,add_share&response_type=token&redirect_uri={2}", oauth.AuthorizeUrl, appKey, callbackUrl);
#elif UNITY_IPHONE||UNITY_ANDROID
		//fullAuthorizeUrl = string.Format("{0}?client_id={1}&response_type=token&redirect_uri=", oauth.AuthorizeUrl, appKey, callbackUrl);
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
			Debug.Log("AuthCallbackAuthCallbackAuthCallbackAuthCallback");
			string[] expireInItem = keypairs[1].Split('=');
			oauth.ExpiresIn = System.DateTime.Now.AddSeconds(System.Convert.ToDouble(expireInItem[1]));
			GetOpenID(oauth.AccessToken);
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
		if(result.commandType == METHOD_GET_OPENID){
			GetOpenIDCallback(result);
			return;
		}
		
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
	
	protected override string SetupTask(Task task){
		string url = oauth.BaseUrl;
		url = oauth.BaseUrl + task.commandType;
		
		task.parameters.Add(new HttpParameter("access_token", oauth.AccessToken));
		if(task.commandType != METHOD_GET_OPENID){
			Debug.Log("in setup task:" + url);
			
			Debug.Log(task.requestMethod);
			task.parameters.Add(new HttpParameter("oauth_consumer_key", appKey));
			task.parameters.Add(new HttpParameter("openid", oauth.OpenID));
			//task.parameters.Add(new HttpParameter("url", Uri.EscapeDataString("http://www.baidu.com")));
		}
		
		return oauth.Request(url, task.requestMethod, task.parameters.ToArray());
	}
	
}

