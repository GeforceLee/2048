using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;

/// <summary> 
/// Base class for all of open platforms.
/// </summary>

public class OpenPlatformBase : MonoBehaviour {
	
	public string appKey = "your Appkey";
	public string appSecret = " your AppSecrect";
	public string callbackUrl = "your callback url";
	
//	[HideInInspector]
//	public bool isBinded;
	
	[HideInInspector]
	public bool isAuthorizing = false;	
	[HideInInspector]
	public string platformName;
	//callback after successully authorized
	public delegate void OauthCallBack(PlatformType pType, bool success);
	public event OauthCallBack OnOauthCallBack;
	
	//callback on request return
	public delegate void CallBack(ResponseResult res);
	public event CallBack OnCallBack;
	
	[HideInInspector]
	public string uid;
	
	private string callbackResult = "";
	
	protected string fullAuthorizeUrl = "";
	protected PlatformType platformType;
	protected OAuth oauth = null;
	
	protected bool isSelected;
	protected bool isInited = false;
	// task response queue
	protected List<ResponseResult> resultList = new List<ResponseResult>();

	//commandType define for oauth1
	protected const string CommandTypeRequestToken = "CommandTypeRequestToken";
	protected const string CommandTypeAccessToken = "CommandTypeAccessToken";
	
	[HideInInspector]
	public ResponseResult lastResponse;
	/// <summary> 
	/// Init with specific platform and name.
	/// </summary>	
	/// <param name="pType"> PlatformType</param>
	/// <param name="pName"> platform name, string type, eg:"facebook", "twitter", etc. </param>
	public void InitWithPlatform(PlatformType pType, string pName){
		platformType = pType;
		platformName = pName;
		oauth = new OAuth(platformType, appKey, appSecret, callbackUrl);
		//this.IsBinded = oauth.VerifierAccessToken();
		if(!this.IsBinded){
			this.IsSelected = false;
		}else{
			isSelected = (PlayerPrefs.GetInt(platformName + "_isSelected") != 0);
		}
	}
	
	/// <summary> 
	/// Handle task responses and callback in main thread.
	/// </summary>
	public void Update(){
//		if(platformType == PlatformType.PLATFORM_SINAWEIBO)
//			Debug.Log("update parent");
		if(resultList.Count>0){
			lock(resultList){
				foreach(var res in resultList){	
					if(oauth.oauthVersion == OAuthVersion.VERSION_1){
						if(res.commandType == CommandTypeRequestToken ){
							if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
								OauthWebView.instance.OpenURL(platformType, fullAuthorizeUrl);
							}else{
								OnAuthorizingResult(false);
							}
						}else if(res.commandType == CommandTypeAccessToken){
							if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
								OnAuthorizingResult(true);
							}else{
								OnAuthorizingResult(false);
							}							
						}else{
							isAuthorizing = false;
							if(OnCallBack!=null)
								OnCallBack(res);
						}

					}else if(OnCallBack!=null){
						Debug.Log("OnCallBack call in Update");
						OnCallBack(res);
						
					}
					
					if(res.returnType == ReturnType.RETURNTYPE_OAUTH_FAILED){
						Logout();
					}
				}
				
				resultList.Clear();
			}
		}
	}
	/// <summary>
	/// inherited class should implement it
	/// guide user to authorize in webview
	/// </summary>

	public virtual void Authorize(){
		//isBinded = false;
		isAuthorizing = true;
		Debug.Log("in openurl:" + fullAuthorizeUrl);
		OauthWebView.instance.OpenURL(platformType, fullAuthorizeUrl);
	}

	/// <summary>
	/// inherited class should implement it
	/// it'll be called by plugin side after user successfully authorized in the webview
	/// </summary>
	/// <param name="resultUrl">redirect url after authorized, may includes info like accesstoken, etc.</param>
	public virtual void  AuthCallback(string result){
		if(result == "UserCancel" || string.IsNullOrEmpty(result)){
			OnAuthorizingResult(false);
			return;
		}
		//access_token=USER_ACCESS_TOKEN&expires_in=NUMBER_OF_SECONDS_UNTIL_TOKEN_EXPIRES
		string[] keypairs = result.Split('&');
		if(keypairs.Length>=2){
			foreach(string keypair in keypairs )
			{
				if( keypair.Contains("access_token") )
				{
					string[] tokenItem = keypair.Split('=');
					oauth.AccessToken = tokenItem[1];	
				}
				else if( keypair.Contains("expires_in") )
				{
					string[] expireInItem = keypair.Split('=');
					oauth.ExpiresIn = System.DateTime.Now.AddSeconds(System.Convert.ToDouble(expireInItem[1]));
					OnAuthorizingResult(true);
				}
			}
		}else{
			OnAuthorizingResult(false);
		}
	}
	
	protected void OnAuthorizingResult(bool success){
		isAuthorizing = false;
		if(OnOauthCallBack!=null)
			OnOauthCallBack(platformType, success);	
	}
	/// <summary>
	/// add extra http parameters,
	/// inherited class may override if need to do custom setup.
	/// </summary>
	/// <param name="task">the task need to be setup </param>
	protected virtual string SetupTask(Task task){
		string url = oauth.BaseUrl;
		url = oauth.BaseUrl + task.commandType;
		task.parameters.Add(new HttpParameter("access_token", oauth.AccessToken));
		return oauth.Request(url, task.requestMethod, task.parameters.ToArray());
	}	

	/// <summary>
	/// it'll be called after request return
	/// you should parse the response here
	/// inherited class should implement it
	/// </summary>
	/// <param name="result">response result, the full response text is in result.description</param>
	protected virtual void HandleResponse(ResponseResult result){
		lastResponse = result;
		lock(resultList){
			resultList.Add(result);
		}
	}
	
	/// <summary>
	/// asynchronously send request to an open platform.
	/// </summary>
	/// <param name="t">the task to be send</param>
	protected void SendCommand(Task task )
	{
		oauth.AsyncInvoke<string>(
			delegate()
			{
				return SetupTask(task);
			},
			delegate(AsyncCallback<string> callback)
			{
				ResponseResult result = new ResponseResult();
				result.platformType = platformType;
				result.commandType = task.commandType;
				result.description = callback.Data;
				Debug.Log(result.description);
				HandleResponse(result);
			}
		);
	}
	
	/// <summary>
	/// virtual share function, override it in subclass.
	/// </summary>
	/// <param name="text">the text to share</param>	
	public virtual void Share(string text)
	{
	}
	
	/// <summary>
	/// log out to the open platform. It only delete saved accesstoken.
	/// Write your own logout function refer to open platform if you want to a "real" logout.
	/// </summary>
	/// <param name="text">the text to share</param>	
	public virtual void Logout(){
		if(oauth!=null){
			oauth.ResetToken();
		}
		OauthWebView.instance.Logout(platformType);
		this.isSelected = false;
	}

    public OAuthVersion Version
    {
        get{return oauth.oauthVersion;}
    }
	
    public bool IsBinded
    {
        get {return oauth!=null && oauth.VerifierAccessToken();}
    }
	
    public bool IsSelected
    {
        get {return isSelected;}
        set{
            isSelected = value;
            PlayerPrefs.SetInt(platformName + "_isSelected", isSelected?1:0);
        }
    }
	// use for test
	void OnGUI(){
//		callbackResult = GUI.TextField (new Rect (200, 140, 100, 30), callbackResult, 512);
//		if(GUI.Button(new Rect(100, 200, 300, 60), "Authorize")){
//			Authorize();
//		}
//
//		if(GUI.Button(new Rect(100, 300, 300, 60), "AuthorizeCallBack")){
//			
//			AuthCallback(callbackResult);
//		}
//
//		if(GUI.Button(new Rect(100, 400, 300, 60), "share")){
//			string text = "test at " + System.DateTime.Now.ToString();
//			Share(text);
//		}
	}
}
