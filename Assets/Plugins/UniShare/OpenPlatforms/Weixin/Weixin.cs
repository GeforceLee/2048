using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using UniShare.Json;

public enum  WXErrCode {
    WXSuccess           = 0,
    WXErrCodeCommon     = -1,
    WXErrCodeUserCancel = -2,
    WXErrCodeSentFail   = -3,
    WXErrCodeAuthDeny   = -4,
    WXErrCodeUnsupport  = -5,
};

public enum WXScene {
    WXSceneSession  = 0, 
    WXSceneTimeline = 1,
};

/// <summary> 
/// TencentWeibo singleton class.
/// <para> mainpage: http://t.qq.com/ </para>
/// <para> open platform: http://dev.t.qq.com/ </para>
/// <para> Implemented API: "t/add", "t/add_pic", "t/add_pic_url" </para>
/// </summary> 
public class Weixin : OpenPlatformBase {

	private static Weixin s_instance = null;
	public readonly string WECHAT_TASK = "WechatTask";


	void Init() {
		if(isInited)
			return;
		InitWithPlatform(PlatformType.PLATFORM_TENCENTWEIBO, "weixin");
		OauthWebView.instance.registerWeixinApi(appKey);
		isInited = true;
	}

	public static Weixin instance {
		get {
			if (s_instance == null) {
				s_instance = FindObjectOfType(typeof(Weixin)) as Weixin;
				if(s_instance != null)
					s_instance.Init();
			}
			
			if (s_instance == null) {
				GameObject obj = new GameObject("Weixin");
				s_instance = obj.AddComponent(typeof(Weixin)) as Weixin;
				s_instance.Init();
				Debug.Log("Could not locate an Weixin object. Weixin was Generated Automaticly");
			}
			return s_instance;
		}
	}
	
	public void onResp(string errorCode)
	{
		WXErrCode wxErrorcode = (WXErrCode)Convert.ToInt32(errorCode);
		
		ResponseResult result = new ResponseResult();
		result.platformType = platformType;
		result.commandType = WECHAT_TASK;
		result.description = "";

		if(wxErrorcode == WXErrCode.WXSuccess){
			result.returnType = ReturnType.RETURNTYPE_SUCCESS;
		}else if(wxErrorcode == WXErrCode.WXErrCodeUserCancel){
			result.returnType = ReturnType.RETURNTYPE_USERCANCEL;
			result.description = "User cancel";
		}else{
			result.returnType = ReturnType.RETURNTYPE_OTHER_ERROR;
			result.description = string.Format("errorcode = {0}", wxErrorcode);
		}
		
		lock(resultList){
			resultList.Add(result);
		}
	}
	
	public void Share(string title, string contentString, string thumbImagePath, string url, WXScene scene)
	{
		OauthWebView.instance.shareToWechatWithText(title, contentString, thumbImagePath, url, (int)scene);
	}
	
	public void ShareWithImage(string imagePath, WXScene scene)
	{
		OauthWebView.instance.shareToWechatWithImage(imagePath, (int)scene);
	}
		
}
