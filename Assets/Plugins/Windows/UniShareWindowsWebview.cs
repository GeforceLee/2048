using UnityEngine;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using UniShareWindows;
public class UniShareWindowsWebview : MonoBehaviour {
	ReceiverSocket clientSocket;
	PlatformType currentPlatformType;
	string[] platformNameArray;
	string authorizingUrl;
	private static UniShareWindowsWebview s_instance = null;
	private bool isInited;
	private bool isWebviewOpen;
	private bool isConnecting;
	public static UniShareWindowsWebview instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(UniShareWindowsWebview)) as UniShareWindowsWebview;
				//if(s_instance!=null)
					//s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("UniShareWindowsWebview");
				s_instance = obj.AddComponent(typeof(UniShareWindowsWebview)) as UniShareWindowsWebview;
				//s_instance.Init();
				Debug.Log("Could not locate an UniShareWindowsWebview object. UniShareWindowsWebview was Generated Automaticly");
			}
			return s_instance;
		}
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    while (clientSocket.messageQueue.Count > 0)
        {
            MessageBase message = clientSocket.messageQueue.Dequeue();
			string logstr = string.Format("%s%s,%s,%s", message.platformtype.ToString(), message.messageType.ToString(), message.url);
			Debug.Log(message.platformtype.ToString() + message.messageType.ToString() + message.url);
			
			if(message.messageType == MessageType.MESSAGE_TYPE_AUTHORIZE_CALLBACK){
				//string platformName = platformNameArray[message.platformtype];
				sendAuthorizeCallback(message.platformtype, message.url);
			}
        }
	}
	
	void Init () {
		if(isInited)
			return;
		//platformNameArray = new string[]{"新浪微博", "腾讯微博", "人人网", "开心", "豆瓣", "微信", "Facebook", "Twitter", "Linkedin", "Foursquare", "GooglePlus"};
		
		clientSocket = new ReceiverSocket(55555);
		clientSocket.SocketConnected += SocketConnectedHandler;
		clientSocket.DataSent += DataSentHandler;
		clientSocket.DataRecieved += DataRecievedHandler;
		clientSocket.SocketDisconnected += SocketDisconnectedHandler;
		isInited = true;
	}
	
	void newClient()
	{
		clientSocket = new ReceiverSocket(55555);
		clientSocket.SocketConnected += SocketConnectedHandler;
		clientSocket.DataSent += DataSentHandler;
		clientSocket.DataRecieved += DataRecievedHandler;
		clientSocket.SocketDisconnected += SocketDisconnectedHandler;
	}
	
	void connect()
	{
		//clientSocket.Connect();
		newClient();
		StartCoroutine(connectCoroutine());
	}
	
	IEnumerator connectCoroutine()
	{
		Debug.Log("clientSocket.Connected = " + clientSocket.Connected.ToString());
		while(!isConnecting&&!clientSocket.Connected){
			isConnecting = true;
			clientSocket.Connect();
			yield return null;
		}
		yield return null;
	}
	
	void sendNavigationMessage()
	{
		MessageBase message = new MessageBase();
		message.platformtype = currentPlatformType;
		message.url = authorizingUrl;
		clientSocket.Send(message);
	}
	
	public void LoadUrl(PlatformType platform, string url)
	{
		currentPlatformType = platform;
		authorizingUrl = url;
		
		if(!isWebviewOpen){
			string externalAppPath = Application.dataPath + "/../unisharewindows/UniShareWebView.exe";
			Debug.Log(externalAppPath);
			if(!File.Exists(externalAppPath)){
				Debug.Log("external webview for windows not found, please check it.");
				return;
			}
			int intPt = (int)platform;
			string argString = string.Format("{0} {1}", intPt, authorizingUrl);
			Debug.Log(argString);
			//System.Diagnostics.Process.Start(externalAppPath);
			System.Diagnostics.Process.Start(externalAppPath, argString);
			isWebviewOpen = true;
		}
		//if(!clientSocket.Connected){
			connect();
		//}
	}
	
	public void SocketConnectedHandler(Socket socket)
	{
		Debug.Log("connected success!");
		isConnecting = false;
		//sendNavigationMessage();
	}
	
	public void DataSentHandler(Socket socket, int length)
	{
		Debug.Log("message send success!");	
	}
	
	 public void DataRecievedHandler(Socket socket, MessageBase msg)
    {
//        while (clientSocket.messageQueue.Count > 0)
//        {
//            MessageBase message = clientSocket.messageQueue.Dequeue();
//			string logstr = string.Format("%s%s,%s,%s", message.platformtype.ToString(), message.messageType.ToString(), message.url);
//			Debug.Log(message.platformtype.ToString() + message.messageType.ToString() + message.url);
//			
//			if(message.messageType == MessageType.MESSAGE_TYPE_AUTHORIZE_CALLBACK){
//				//string platformName = platformNameArray[message.platformtype];
//				sendAuthorizeCallback(message.platformtype, message.url);
//			}
//        }
    }
	
	private void sendAuthorizeCallback(PlatformType platform, string url)
	{
		
		Debug.Log("sendAuthorizeCallback");
		Debug.Log(url);
		int index = -1;
		string accesstokenUrl = url;
		if(platform == PlatformType.PLATFORM_SINAWEIBO){
			index = url.IndexOf("code=");
			if(index>0){
				accesstokenUrl = url.Substring(index);
			}
			SinaWeibo.instance.AuthCallback(accesstokenUrl);
			return;
		}
	
		index = url.IndexOf("access_token");
		
		if(index>0){
			accesstokenUrl = url.Substring(index);
		}else if(url.IndexOf("oauth_verifier")>0){
			accesstokenUrl = url.Substring(url.IndexOf("oauth_token"));
		}else if(accesstokenUrl != "UserCancel"){
			return;
		}
		Debug.Log("sendAuthorizeCallback UserCancel");
		if(platform == PlatformType.PLATFORM_FACEBOOK){
			Facebook.instance.AuthCallback(accesstokenUrl);
		}
		else if(platform == PlatformType.PLATFORM_FOURSQUARE){
			Foursquare.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_GOOGLEPLUS){
			GooglePlus.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_KAIXIN){
			Kaixin.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_LINKEDIN){
			Linkedin.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_RENREN){
			Renren.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_SINAWEIBO){
			SinaWeibo.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_TENCENTWEIBO){
			TencentWeibo.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_TWITTER){
			Twitter.instance.AuthCallback(accesstokenUrl);
		}else if(platform == PlatformType.PLATFORM_QZONE){
			QZone.instance.AuthCallback(accesstokenUrl);
		}
	}
	
	public void SocketDisconnectedHandler()
	{
		//clientSocket.Disconnect(false);
		clientSocket.Close();
		Debug.Log("socket disconneccted!");	
		isWebviewOpen = false;
	}
}
