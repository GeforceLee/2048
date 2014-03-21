using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;
/// <summary> 
/// Twitter singleton class.
/// <para> mainpage: http://twitter.com/ </para>
/// <para> open platform: https://dev.twitter.com/ </para>
/// <para> Implemented API: "statuses/update.json", "statuses/update_with_media.json" </para>
/// </summary> 
public class Twitter : OpenPlatformBase {

	private static Twitter s_instance = null;
	
	public readonly string STATUS_UPDATE = "statuses/update.json";
	public readonly string STATUS_UPDATE_WITH_IMAGE = "statuses/update_with_media.json";

	// Use this for initialization
	void Init () {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_TWITTER, "twitter");
		isInited = true;
	}
	
	public static Twitter instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Twitter)) as Twitter;
				if(s_instance!=null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Twitter");
				s_instance = obj.AddComponent(typeof(Twitter)) as Twitter;
				s_instance.Init();
				Debug.Log("Could not locate an Twitter object. Twitter was Generated Automaticly");
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
	/// Updates user's current
	/// </summary>
	/// <param name="status">The text of your status update, less than 140 characters.</param>
	/// <returns></returns>
	public override void Share(string status)
	{
		//string status = Utility.UrlEncode(content);
//		string response = oauth.Request("https://api.twitter.com/1.1/statuses/update.json", RequestMethod.Post, new HttpParameter("status", status));
//		Debug.Log("response=" + response);
		
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", status)
        };
		task.commandType = STATUS_UPDATE;
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
	
	/// <summary>
	/// Updates the authenticating user's current status with location
	/// </summary>
	/// <param name="status">The text of your status update, less than 140 characters.</param>
	/// <param name="lat">The latitude of the location this tweet refers to</param>
	/// <param name="lon">The longitude of the location this tweet refers to</param>
	/// <returns></returns>
	public void Share(string status, float lat, float lon)
	{
		//string status = Utility.UrlEncode(content);
//		string response = oauth.Request("https://api.twitter.com/1.1/statuses/update.json", RequestMethod.Post, new HttpParameter("status", status));
//		Debug.Log("response=" + response);
		
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", status),
            new HttpParameter("lat", lat),
            new HttpParameter("long", lon)
        };
		task.commandType = STATUS_UPDATE;
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
	
    /// <summary>
    /// Updates the authenticating user's current status with uploading image
    /// </summary>
    /// <param name="status">The text of your status update, less than 140 characters.</param>
    /// <param name="media">the image bytes you want to upload</param>
    /// <returns></returns>
	public void ShareWithImage(string status, byte[] media){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("status", status),
            new HttpParameter("media[]", media)
        };
		task.commandType = STATUS_UPDATE_WITH_IMAGE;
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
	
	/// <summary>
    /// Updates the authenticating user's current status with uploading image
    /// </summary>
    /// <param name="status">The text of your status update, less than 140 characters.</param>
    /// <param name="mediaPath">the local path of the image you want to upload</param>
    /// <returns></returns>
	public void ShareWithImage(string status, string mediaPath){
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
		ShareWithImage(status, pic);			
	}
	
	public override void Authorize(){
		Logout();
		isAuthorizing = true;
		oauth.OAuthToken = "";
		oauth.OAuthTokenSecret = "";
		
		oauth.AsyncInvoke<string>(
			delegate()
			{
				var response = oauth.Request(oauth.RequestTokenUrl, RequestMethod.Post);
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

					fullAuthorizeUrl = string.Format("{0}?oauth_token={1}",oauth.AuthorizeUrl, oauth.OAuthToken);
					string[] expireInItem = keypairs[1].Split('=');
					oauth.OAuthTokenSecret = expireInItem[1];
					ResponseResult result = new ResponseResult();
					result.platformType = platformType;
					result.commandType = CommandTypeRequestToken;
					result.description = responseString;
					result.returnType = ReturnType.RETURNTYPE_SUCCESS;
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
		//oauth_token=tw0uJBgLW6b5m7lCy3e49JARF2PmjxjNyQqWTajXI&oauth_verifier=dRWNX8W8bc95mURiufEldBqMIBp1ufTsRUgGtW9p0
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
								
					return response;
				},
				delegate(AsyncCallback<string> callback)
				{
					string responseString = callback.Data;
					Debug.Log("AuthCallback" + responseString);
					if(!string.IsNullOrEmpty(responseString)&& (responseString.IndexOf("oauth_token")>=0)){
						keypairs = responseString.Split('&');
						tokenItem = keypairs[0].Split('=');
						oauth.OAuthToken = tokenItem[1];
						tokenItem = keypairs[1].Split('=');
						oauth.OAuthTokenSecret = tokenItem[1];
						
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
						res.commandType = CommandTypeAccessToken;
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
		return oauth.Request(url, task.requestMethod, task.parameters.ToArray());
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
		}else
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;		

		base.HandleResponse(result);
	}
}
