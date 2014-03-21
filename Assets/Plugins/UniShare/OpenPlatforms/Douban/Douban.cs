using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;


public class Douban : MonoBehaviour {
	public string appKey = "你的Appkey";
	public string appSecret = "你的AppSecrect";
	public string callbackUrl = "你的回调页面";
	
	public delegate void CallBack(ResponseResult res);
	public event CallBack OnCallBack;
	
	private string accessToken = "";
	
	private static Douban s_instance = null;
	
	private const string BASE_URL = "https://api.douban.com/";
	private const string AUTHORIZE_URL = "https://www.douban.com/service/auth2/auth";
	
	private string callbackResult = "";
	private List<Task> delayedTasks = null;
	
	const string STATUSES = "shuo/v2/statuses/";
	//const string STATUSES  = "statuses/upload.json";
	const string PLAYER_PREFS_ACCESSTOKEN = "DoubanAccessToken";
	//accesstoken过期时间
	const string PLAYER_PREFS_EXPIRES_IN = "DoubanExpiresIn";
	
	private System.DateTime expiresIn;
	OAuth oauth;
	private string uid;
	// Use this for initialization
	void Start () {
		//PlayerPrefs.DeleteAll();
		accessToken = PlayerPrefs.GetString(PLAYER_PREFS_ACCESSTOKEN);
		delayedTasks = new List<Task>();
		if(PlayerPrefs.HasKey(PLAYER_PREFS_EXPIRES_IN)){
			string strExpiresIn = PlayerPrefs.GetString(PLAYER_PREFS_EXPIRES_IN);
			expiresIn = System.Convert.ToDateTime(strExpiresIn);		
		}
		oauth = new OAuth(PlatformType.PLATFORM_DOUBAN, appKey, appSecret, callbackUrl);
	}
	
	public static Douban instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Douban)) as Douban;
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Douban");
				s_instance = obj.AddComponent(typeof(Douban)) as Douban;
				Debug.Log("Could not locate an Douban object. Douban was Generated Automaticly");
			}
			return s_instance;
		}
	}
	
	protected void OnApplicationQuit() {
		s_instance = null;
	}
	
	void OnDestroy() {
		s_instance = null;
	}
	/// <summary>
	/// 发布一条广播信息 
	/// </summary>
	/// <param name="text">广播内容，不超过140个汉字。</param>
	/// <returns></returns>
	public void Share(string text)
	{
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
			
			new HttpParameter("source", appKey),
			new HttpParameter("accesstoken", accessToken),
            new HttpParameter("text", text)
        };
		task.commandType = STATUSES;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//先验证accessToken有效性，若验证失败则调用授权页面引导用户登录
		if(!VerifierAccessToken()){
//			//添加到任务队列里
//			delayedTasks.Add(task);
//			//授权
//			Authorize();
		}else{
			SendCommand(task);
		}
	}
	
	
    /// <summary>
    /// 发布一条记录并上传本地图片
    /// </summary>
    /// <param name="content">发记录的内容，不超过140个汉字。</param>
    /// <param name="pic">要上传的图片的二进制数据，仅支持jpg/jpeg/gif/png/bmp格式，图片大小小于10M。</param>
    /// <returns></returns>
	public void ShareWithImage(string content, byte[] pic){
		Task task;
		List<HttpParameter> config = new List<HttpParameter>()
        {
            new HttpParameter("content", content),
			new HttpParameter( "pic", pic)
        };
		task.commandType = STATUSES;
		task.requestMethod = RequestMethod.Post;
		task.parameters = config;
		//先验证accessToken有效性，若验证失败则调用授权页面引导用户登录
		if(!VerifierAccessToken()){
			//添加到任务队列里
			delayedTasks.Add(task);
			//授权
			Authorize();

		}else{
			SendCommand(task);
		}
	}	
	
	/// <summary>
    /// 发布一条记录并上传本地图片
    /// </summary>
    /// <param name="content">要发布的微博文本内容，内容不超过140个汉字。 </param>
    /// <param name="imgPath">要上传的图片的路径，可以是本地图片路径也可以是网上图片链接，图片大小小于10M，支持jpg/jpeg/gif/png/bmp</param>
    /// <returns></returns>
	//带图片分享
	public void ShareWithImage(string content, string imgPath){
		//图片链接
		if(imgPath.StartsWith("http")){
			Task task;
			List<HttpParameter> config = new List<HttpParameter>()
	        {
	            new HttpParameter("content", content),
				new HttpParameter( "picurl", imgPath)
	        };
			task.commandType = STATUSES;
			task.requestMethod = RequestMethod.Post;
			task.parameters = config;
			//先验证accessToken有效性，若验证失败则调用授权页面引导用户登录
			if(!VerifierAccessToken()){
				//添加到任务队列里
				delayedTasks.Add(task);
				//授权
				Authorize();
	
			}else{
				SendCommand(task);
			}		
		}else{
			//本地图片路径
			//读取图片
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
	
	void Authorize(){
		string authUrl = string.Format("{0}?client_id={1}&response_type=token&scope=douban_basic_common,shuo_basic_r,shuo_basic_w&redirect_uri={2}", AUTHORIZE_URL, appKey, callbackUrl);
		//string authUrl = string.Format("{0}?client_id={1}&response_type=token", AUTHORIZE_URL, appKey);

		Application.OpenURL(authUrl);
	}
	
	void AuthCallback(string result){
		//access_token=USER_ACCESS_TOKEN&expires_in=NUMBER_OF_SECONDS_UNTIL_TOKEN_EXPIRES
		string[] keypairs = result.Split('&');
		if(keypairs.Length>=2){
			string[] tokenItem = keypairs[0].Split('=');
			accessToken = tokenItem[1];
			PlayerPrefs.SetString(PLAYER_PREFS_ACCESSTOKEN, accessToken);
			
			string[] expireInItem = keypairs[1].Split('=');
			expiresIn = System.DateTime.Now.AddSeconds(System.Convert.ToDouble(expireInItem[1]));
			PlayerPrefs.SetString(PLAYER_PREFS_EXPIRES_IN, expiresIn.ToString());
			
//			string[] uidItem = keypairs[5].Split('=');
//			uid = uidItem[1];			
			
			Debug.Log("AccessToken=" + accessToken);
			//执行任务队列里的任务
			foreach(var task in delayedTasks){
				SendCommand(task);
			}
			delayedTasks.Clear();
		} 
	}
	//验证AccessToken有效性
	public bool VerifierAccessToken()
	{
		if(!PlayerPrefs.HasKey(PLAYER_PREFS_ACCESSTOKEN)||!(PlayerPrefs.HasKey(PLAYER_PREFS_EXPIRES_IN)))
			return false;
		return (DateTime.Compare(expiresIn, DateTime.Now)>=0);
	}
	
	private void SendCommand(Task task )
	{
		string url = BASE_URL;
		RequestMethod requestMethod = RequestMethod.Post;
		
		url = BASE_URL + STATUSES;


		//task.parameters.Add(new HttpParameter("access_token", accessToken));
		var response = oauth.Request(url, requestMethod, task.parameters.ToArray());
		Debug.Log("response="+response);
//		if (OnCallBack != null)
//			OnCallBack(task.commandType, response);
	}
	
	void TestShareWithImage()
	{
		string text = "test at " + System.DateTime.Now.ToString();
		ShareWithImage(text, "1.jpg");
	}

//	void OnGUI(){
//		callbackResult = GUI.TextField (new Rect (200, 140, 100, 30), callbackResult, 512);
//	
//		if(GUI.Button(new Rect(100, 200, 150, 40), "授权")){
//			//Request r = new Request("get", "http://www.baidu.com");
//			//string result = r.Send();
//			//Debug.Log(result);
//			Authorize();
//		}
//
//		
//		if(GUI.Button(new Rect(270, 200, 150, 40), "授权回调")){
//			
//			AuthCallback(callbackResult);
//		}
//		
//		if(GUI.Button(new Rect(420, 200, 150, 40), "发布记录")){
//			string url = "https://api.douban.com/shuo/v2/statuses/";
//			RequestMethod method = RequestMethod.Post;
//			List<HttpParameter> config = new List<HttpParameter>()
//	        {
//	            new HttpParameter("source", appKey),
//	            new HttpParameter("text", "safsafsda")
//	        };
//			HttpParameter[] parameters = config.ToArray();
//			//var response = HttpEngine.Request("https://api.douban.com/v2/user/~me", RequestMethod.Get, new HttpParameter("access_token", accessToken+"1"));
//			//Debug.Log("response="+response);
//
//            Request r = null;
//
//            //foreach (var param in parameters)
//            //    r.AddHeader(param.Name, string.Format("{0}", param.Value));
//
//            string rawUrl = string.Empty;
//            UriBuilder uri = new UriBuilder(url);
//            
//
//            string result = string.Empty;
//            bool multi = false;
//            foreach (var item in parameters)
//            {
//                if (item.IsBinaryData)
//                {
//                    multi = true;
//                    break;
//                }
//            }
//
//            switch (method)
//            {
//                case RequestMethod.Get:
//                    {
//                        uri.Query = Utility.BuildQueryString(parameters);
//                    }
//                    break;
//                case RequestMethod.Post:
//                    {
//                        if (!multi)
//                        {
//                            uri.Query = Utility.BuildQueryString(parameters);
//                        }
//                    }
//                    break;
//            }
//
////            if (string.IsNullOrEmpty(AccessToken))
////            {
////                if (uri.Query.Length == 0)
////                {
////                    uri.Query = "source=" + AppKey;
////                }
////                else
////                {
////                    uri.Query += "&source=" + AppKey;
////                }
////            }
//
//            switch (method)
//            {
//                case RequestMethod.Get:
//                    {
//                        r = new Request("get", uri.Uri);
//                    }
//                    break;
//                case RequestMethod.Post:
//                    {
//                        r = new Request("post", uri.Uri);
//                    }
//                    break;
//            }
//
//            r.SetHeader("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.0)");
//            r.AddHeader("Connection", "keep-alive");
//			r.AddHeader("Content-Type", "text/html");
//
////            if (!string.IsNullOrEmpty(AccessToken))
////            {
////                r.SetHeader("Authorization", string.Format("OAuth2 {0}", AccessToken));
////            }
//			r.SetHeader("Authorization", string.Format("Bearer {0}", accessToken));
//            switch (method)
//            {
//                case RequestMethod.Get:
//                    {
//                        //http.Method = "GET";
//                    }
//                    break;
//                case RequestMethod.Post:
//                    {
//                        
//                    }
//                    break;
//            }
//            result = r.Send();
//            Debug.Log(result);			
//			
//		}
//		
//		if(GUI.Button(new Rect(620, 200, 150, 40), "分享图片")){
//			Share("test");
//			//ShareWithImage("upload image at" + System.DateTime.Now.ToString(), "http://ww1.sinaimg.cn/mw690/b29b22acgw1dy4dor334sj.jpg");
//			//ShareWithImage("upload image at" + System.DateTime.Now.ToString(), "1.jpg"); 
//			//TestTask();
//		}		
//	}
}
