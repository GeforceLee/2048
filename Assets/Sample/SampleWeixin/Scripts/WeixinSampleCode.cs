using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UniShare;
public class WeixinSampleCode : MonoBehaviour {
	public GUIText displayText;
	string template;
	// Use this for initialization
	void Start () {
		Debug.Log(Application.persistentDataPath);
		Weixin.instance.OnCallBack += onMessageCallBack;
		template = "Post byUniShar(sharekit for unity)at{0}.https://www.assetstore.unity3d.com/#/search/unishare";
	}
	
	IEnumerator captureAndShare()
	{
		string imgPath = Application.persistentDataPath + "/capture.png";
		Application.CaptureScreenshot("capture.png");
		while(!System.IO.File.Exists(imgPath)){
			yield return null;   
		}
		Weixin.instance.ShareWithImage(imgPath, WXScene.WXSceneSession);
		
	}
	
	void OnGUI(){
		//Facebook
		if(GUI.Button(new Rect(100, 50, 100, 60), "ShareWithText")){
			string text = string.Format(template, System.DateTime.Now.ToString());
			Weixin.instance.Share("weixin share test", text, null, "http://www.baidu.com", WXScene.WXSceneSession);
		}
		if(GUI.Button(new Rect(220, 50, 100, 60), "ShareWithImage")){
			string imgPath = Application.persistentDataPath + "/capture.png";
			displayText.text = string.Format("{0}", imgPath);
			//Weixin.instance.ShareWithImage("http://open.weixin.qq.com/static/zh_CN/app/images/open-api_0303.png", WXScene.WXSceneSession);
			StartCoroutine(captureAndShare());	
//			Application.CaptureScreenshot("capture.png");
//			Weixin.instance.ShareWithImage(imgPath, WXScene.WXSceneSession);
		}
	}
	
	public void onMessageCallBack(ResponseResult res)
	{
		if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
			displayText.text = "send success!";
		}else if(res.returnType == ReturnType.RETURNTYPE_USERCANCEL){
			displayText.text = "user canceled";
		}else{
			displayText.text = res.description;
		}
	}
	
	void OnDestroy()
	{
		Weixin.instance.OnCallBack -= onMessageCallBack;
	}
}
