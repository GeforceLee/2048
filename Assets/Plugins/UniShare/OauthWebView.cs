using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using UniShare;

#if UNITY_EDITOR || UNITY_STANDALONE_OSX
public class UnitySendMessageDispatcher
{
	public static void Dispatch(string name, string method, string message)
	{
		GameObject obj = GameObject.Find(name);
		if (obj != null)
			obj.SendMessage(method, message);
	}
}
#endif

public enum WechatMessageType{
	WXMESSAGE_TYPE_TEXT,
	WXMESSAGE_TYPE_IMG,
	WXMESSAGE_TYPE_MUSIC,
	WXMESSAGE_TYPE_VIDEO,
	WXMESSAGE_TYPE_WEBPAGE
}

/// <summary>
/// webview class for unity, current only support iOS/Android
/// </summary>
public class OauthWebView : MonoBehaviour {
	
	private static OauthWebView s_instance = null;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
	[DllImport("webviewosx")]
	private static extern void _LoadUrl_osx(string url, int platformType, bool isInEditor);

	[DllImport("webviewosx")]
	private static extern void _Logout(int platformType);
#elif UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern void _LoadUrl(string url, int platformType);
	
	[DllImport("__Internal")]
	private static extern void _Logout(int platformType);
	
	[DllImport("__Internal")]
	private static extern void _registerWeixinApi(string urlSchemes);	
	
	[DllImport("__Internal")]
	private static extern bool _isWechatAvailable();
	
	[DllImport("__Internal")]
	private static extern void _shareToWechat(int msgType, string titleString, string content, string thumbImagePath, string url, int scene);
	
	[DllImport("__Internal")]
	private static extern void _shareToWechatWithText(string titleString, string content, string thumbImagePath, string url, int scene);
	
	[DllImport("__Internal")]
	private static extern void _shareToWechatWithImage(string imagePath, int scene);
#elif UNITY_ANDROID
	AndroidJavaObject webView;
#endif
	// Use this for initialization
	void Start () {
		
	}
	void Init (){
#if UNITY_EDITOR || UNITY_STANDALONE_OSX

#elif UNITY_IPHONE
		//webView = _WebViewPlugin_Init(name);

#elif UNITY_ANDROID
		//AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //webView = jc.GetStatic<AndroidJavaObject>("currentActivity");
		using( var jc = new AndroidJavaClass( "com.unisocial.UnisocialPlugin" ) )
   		webView = jc.CallStatic<AndroidJavaObject>( "instance" );
		
#endif	
	}
	/// <summary>
	/// OauthWebView instance
	/// </summary>	
	public static OauthWebView instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(OauthWebView)) as OauthWebView;
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("OauthWebView");
				s_instance = obj.AddComponent(typeof(OauthWebView)) as OauthWebView;
				s_instance.Init();
				Debug.Log("Could not locate an OauthWebView object. OauthWebView was Generated Automaticly");
			}
			return s_instance;
		}
	}
	/// <summary>
	/// open url in internal webview
	/// </summary>		
	/// <param name="platformType">platformType, current only support 6 open platform</param>
	/// <param name="url">url</param>
	public void OpenURL(PlatformType platformType, string url){
		bool isInEditor = Application.platform == RuntimePlatform.OSXEditor;
		Debug.Log("isInEditor=" + isInEditor);
#if UNITY_EDITOR
		if(Application.platform == RuntimePlatform.OSXEditor){
			_LoadUrl_osx(url, (int)platformType, Application.platform == RuntimePlatform.OSXEditor);		
		}else if(Application.platform == RuntimePlatform.WindowsEditor){
			UniShareWindows.PlatformType platform = (UniShareWindows.PlatformType)platformType;
			UniShareWindowsWebview.instance.LoadUrl(platform, url);
			// to be added
		}
#elif UNITY_STANDALONE_OSX
		_LoadUrl_osx(url, (int)platformType, Application.platform == RuntimePlatform.OSXEditor);
#elif UNITY_STANDALONE_WIN
		UniShareWindows.PlatformType platform = (UniShareWindows.PlatformType)platformType;
		UniShareWindowsWebview.instance.LoadUrl(platform, url);
#elif UNITY_IPHONE
		_LoadUrl(url, (int)platformType);
#elif UNITY_ANDROID
		if(webView == null)
			return;
		webView.Call("StartWebView",(int)platformType, url);
#endif		
	}
	
	/// <summary>
	/// logout and end session
	/// </summary>		
	/// <param name="platformType">platformType, current only support 6 open platform</param>
	/// <param name="url">url</param>
	public void Logout(PlatformType platformType){

#if UNITY_EDITOR 
		if(Application.platform == RuntimePlatform.OSXEditor){
			_Logout((int)platformType);		
		}else if(Application.platform == RuntimePlatform.WindowsEditor){
			// to be added
		}
#elif UNITY_STANDALONE_OSX
		_Logout((int)platformType);
#elif UNITY_IPHONE
		_Logout((int)platformType);
#elif UNITY_ANDROID
		if(webView == null)
			return;
		webView.Call("Logout",(int)platformType);
#endif		
	}
	
	void OnDestroy()
	{
#if UNITY_EDITOR || UNITY_STANDALONE_OSX
#endif
	}
	
	public bool isWechatAvailable(){
#if UNITY_EDITOR
		return false;
#elif UNITY_IPHONE		
		return _isWechatAvailable();
#elif UNITY_ANDROID
		return false;
#else
		return false;
#endif		
	}

	public void registerWeixinApi(string urlScheme){
#if UNITY_EDITOR
		
#elif UNITY_IPHONE		
		_registerWeixinApi(urlScheme);
#elif UNITY_ANDROID
		if(webView != null){
			webView.Call("RegisterWeixinApi", urlScheme);
		}
#endif		
	}
	
	public void shareToWechatWithText(string titleString, string content, string thumbImagePath, string url, int scene)
	{
#if UNITY_EDITOR
#elif UNITY_IPHONE
		_shareToWechat((int)WechatMessageType.WXMESSAGE_TYPE_WEBPAGE, titleString, content, thumbImagePath, url, scene);
		//_shareToWechatWithText(titleString, content, thumbImagePath, url, scene);
#elif UNITY_ANDROID
		if(webView != null){
			if(titleString == null){
				titleString = "";
			}
			if(content == null){
				content = "";
			}
			if(thumbImagePath == null){
				thumbImagePath = "";
			}
			if(url == null){
				url = "";
			}
			//webView.Call("ShareToWechat", (int)WechatMessageType.WXMESSAGE_TYPE_WEBPAGE, titleString, content, thumbImagePath, url, scene);
			webView.Call("ShareToWechat", (int)WechatMessageType.WXMESSAGE_TYPE_TEXT, titleString, content, thumbImagePath, url, scene);
		}		
#endif			
	}
	public void shareToWechatWithImage(string imagePath, int scene)
	{
#if UNITY_EDITOR
#elif UNITY_IPHONE	
		_shareToWechat((int)WechatMessageType.WXMESSAGE_TYPE_IMG, "", "", "", imagePath, scene);
		//_shareToWechatWithImage(imagePath, scene);
#elif UNITY_ANDROID
		webView.Call("ShareToWechat", (int)WechatMessageType.WXMESSAGE_TYPE_IMG, "", "", "", imagePath, scene);
#else
		
#endif			
	}
}
