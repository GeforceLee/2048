using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;
/// <summary> 
/// Linkedin singleton class.
/// <para> mainpage: http://linkedin.com/ </para>
/// <para> open platform: http://developer.linkedin.com/ </para>
/// <para> Implemented API: "people/~/shares", "statuses/update_with_media.json" </para>
/// </summary> 
public class Linkedin : OpenPlatformBase {
	private static Linkedin s_instance = null;
	
	public readonly string METHOD_SHARE = "people/~/shares";
	public readonly string STATUS_UPDATE_WITH_IMAGE = "statuses/update_with_media.json";
	
	string scope = "rw_nus";
	
	
	// Use this for initialization
	void Init () {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_LINKEDIN, "linkedin");
		isInited = true;
	}
	
	public static Linkedin instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Linkedin)) as Linkedin;
				if(s_instance !=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Linkedin");
				s_instance = obj.AddComponent(typeof(Linkedin)) as Linkedin;
				s_instance.Init();
				Debug.Log("Could not locate an Linkedin object. Linkedin was Generated Automaticly");
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
	/// Share
	/// </summary>
	/// <param name="comment">Text of member's comment. (Similar to deprecated current-status field.)</param>
	/// <returns></returns>	
	public override void Share(string comment)
	{
		Share(comment, "title", "test", "http://unisharekit.com/", "http://m.c.lnkd.licdn.com/media-proxy/ext?w=180&h=110&f=c&hash=MrOHyun5SSa68QAFeSu9ZaKHKJU%3D&url=http%3A%2F%2Fm3.licdn.com%2Fmedia%2Fp%2F3%2F000%2F124%2F1a6%2F089a29a.png");
	}
	
	/// <summary>
	/// Share
	/// </summary>
	/// <param name="comment">Text of member's comment. (Similar to deprecated current-status field.)</param>
	/// <param name="title">Title of shared document</param>
	/// <param name="description">Description of shared content</param>
	/// <param name="url">URL for shared content</param>
	/// <param name="imageUrl">URL for image of shared content</param>
	/// <returns></returns>
	public void Share(string comment, string title, string description, string url, string imageUrl)
	{
		//string status = Utility.UrlEncode(content);
//		string response = oauth.Request("https://api.twitter.com/1.1/statuses/update.json", RequestMethod.Post, new HttpParameter("status", status));
//		Debug.Log("response=" + response);
		Dictionary<string, object> content = new Dictionary<string, object>();
		content.Add("title", title);
		content.Add("description", description);
		content.Add("submitted-url", url);
		content.Add("submitted-image-url", imageUrl);
		Dictionary<string, object> visibility = new Dictionary<string, object>();
		visibility.Add("code", "anyone");
		Dictionary<string, object> share = new Dictionary<string, object>();
		share.Add("comment", comment);
		share.Add("content", content);
		share.Add("visibility", visibility);
		string shareContentString = JsonWriter.Serialize(share);
		
		Debug.Log(shareContentString);
		
		
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("share", shareContentString)
        };
		task.commandType = METHOD_SHARE;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		if(!oauth.VerifierAccessToken()){
			ResponseResult result = new ResponseResult();
			result.platformType = PlatformType.PLATFORM_LINKEDIN;
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
		isAuthorizing = true;
		oauth.OAuthToken = "";
		oauth.OAuthTokenSecret = "";

		oauth.AsyncInvoke<string>(
			delegate()
			{
				string oauthUrl = string.Format("{0}?scope={1}",oauth.RequestTokenUrl, scope);
				var response = oauth.Request(oauth.RequestTokenUrl, RequestMethod.Post, new HttpParameter("scope", scope));
				Debug.Log("response=" + response);			
				return response;
				
			},
			delegate(AsyncCallback<string> callback)
			{
				string responseString = callback.Data;
				if(responseString.IndexOf("oauth_token")>=0){
					string[] keypairs = responseString.Split('&');
					string[] tokenItem = keypairs[0].Split('=');
					
					oauth.OAuthToken = tokenItem[1];
					Debug.Log("oauth_token=" + oauth.OAuthToken);	
					tokenItem = keypairs[1].Split('=');
					oauth.OAuthTokenSecret = tokenItem[1];		
					fullAuthorizeUrl = string.Format("{0}?oauth_token={1}",oauth.AuthorizeUrl, oauth.OAuthToken);
					string[] expireInItem = keypairs[1].Split('=');
					oauth.OAuthTokenSecret = expireInItem[1];		
					ResponseResult result = new ResponseResult();
					result.platformType = platformType;
					result.commandType = CommandTypeRequestToken;
					result.description = responseString;
					result.returnType = ReturnType.RETURNTYPE_SUCCESS;
					Debug.Log("result.reult=" + oauth.OAuthToken);	
					lock(resultList){
						resultList.Add(result);
					}
				}else{
					ResponseResult result = new ResponseResult();
					result.platformType = platformType;
					result.commandType = CommandTypeRequestToken;
					result.description = responseString;
					result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
					lock(resultList){
						resultList.Add(result);
					}
				}		
			}
		);		
	}

	public override void AuthCallback(string result){
		if(result == "UserCancel" || string.IsNullOrEmpty(result)){
			Logout();
			OnAuthorizingResult(false);
			return;
		}		
		//oauth_token=469f352b-9ff8-4d47-a270-93c378b4b0a1&oauth_verifier=39702
		string oauthVerifier = "";
		//access_token=USER_ACCESS_TOKEN&expires_in=NUMBER_OF_SECONDS_UNTIL_TOKEN_EXPIRES
		string[] keypairs = result.Split('&');
		if(keypairs.Length>=2){
			string[] tokenItem = keypairs[1].Split('=');
			oauthVerifier = tokenItem[1];
			Debug.Log("oauthVerifier=" + oauthVerifier);
			
			oauth.AsyncInvoke<string>(
				delegate()
				{
					var response = oauth.Request(oauth.AccessTokenUrl, RequestMethod.Post, new HttpParameter("oauth_verifier", oauthVerifier));
					Debug.Log("AccessToken response = " + response);
					return response;
				},
				delegate(AsyncCallback<string> callback)
				{
					string responseString = callback.Data;
					if(!string.IsNullOrEmpty(responseString)&& (responseString.IndexOf("oauth_token")>=0)){
						keypairs = responseString.Split('&');
						tokenItem = keypairs[0].Split('=');
						oauth.OAuthToken = tokenItem[1];
						tokenItem = keypairs[1].Split('=');
						oauth.OAuthTokenSecret = tokenItem[1];
						Debug.Log("OAuthToken=" + oauth.OAuthToken);
						ResponseResult res = new ResponseResult();
						res.platformType = PlatformType.PLATFORM_TWITTER;
						res.commandType = CommandTypeAccessToken;
						res.description = responseString;
						res.returnType = ReturnType.RETURNTYPE_SUCCESS;
						lock(resultList){
							resultList.Add(res);
						}
					}else{
						ResponseResult res = new ResponseResult();
						res.platformType = platformType;
						res.commandType = CommandTypeRequestToken;
						res.description = responseString;
						res.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
						lock(resultList){
							resultList.Add(res);
						}
					}		
				}
			);
		} 
	}

	protected override string SetupTask(Task task){
		string url = oauth.BaseUrl;
		url = oauth.BaseUrl + task.commandType;
		return oauth.Request(url, task.requestMethod, task.parameters[0].Value.ToString());
	}

	protected override void HandleResponse(ResponseResult result){
		if(string.IsNullOrEmpty(result.description)){
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}
		else if(result.description.IndexOf("errors")>=0){
			var json = JsonReader.Deserialize<Dictionary<string, object>>(result.description);
			object[] subJson = (object[])json["errors"];
			Dictionary<string, object> errorEntity = (Dictionary<string, object>)subJson[0];
			Debug.Log("errorcode=" + errorEntity["code"]);
			string strErrorCode = errorEntity["code"].ToString();
			if(strErrorCode == "32"
			||strErrorCode == "89"
			||strErrorCode == "135"
			||strErrorCode == "215"
			){
				result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
				//Logout();
			}else
				result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
		}else if(result.description.IndexOf("errorCode")>=0){
			var json = JsonReader.Deserialize<Dictionary<string, object>>(result.description);
			string strErrorCode = json["errorCode"].ToString();

			if(strErrorCode == "0"){
				result.returnType = ReturnType.RETURNTYPE_OAUTH_FAILED;
				//Logout();
			}else
				result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;			
		}
		else
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;	

		base.HandleResponse(result);
	}
}
