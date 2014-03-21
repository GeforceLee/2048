using UnityEngine;
using System.IO;
using System.Collections;
using UniShare;
public class SendWindow : MonoBehaviour {
	[HideInInspector]
	public PlatformType platformType;
	public UILabel logLabel;
	public UILabel contentLabel;
	public UILabel titleLabel;
	public UICheckbox checkBox;
	public UICheckbox wxSceneCheckbox;
	public UIButton sendButton;
	
	GameObject uiShareWindow;
	GameObject uiSendWindow;
	
	string url = "http://unisharekit.com";
	string imgUrl = "http://ww4.sinaimg.cn/mw690/bd83d650gw1e1hlzfnjd1j.jpg";
	string imgPath = "testPicture.jpg";
	
	// for wechat only
	WXScene wxScene;
	
	// Use this for initialization
	void Start () {
		imgPath = Application.persistentDataPath + "/" + imgPath; 
		if(!File.Exists(imgPath)){
			TextAsset bindata= Resources.Load("picture") as TextAsset;
			File.WriteAllBytes(imgPath, bindata.bytes);
		}
		
		uiShareWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare");
		uiSendWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelSend");
		
		GameObject btnSend = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelSend/ButtonSend");
		UIEventListener.Get(btnSend).onClick = OnButtonSend;
		
		GameObject btnBack = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelSend/ButtonBack");
		UIEventListener.Get(btnBack).onClick = OnButtonBack;
		
		Debug.Log("checkBox: " + checkBox.isChecked.ToString());
		
		
		RegisterOpenPlatforms();
		
		wxScene = WXScene.WXSceneSession;
	}
	
	public void ShowSendWindowWithPlatform(PlatformType pType)
	{
		platformType = pType;
		checkBox.isChecked = false;
		GameObject option = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelSend/OptionsWXScene");
		NGUITools.SetActive(option, false);
		if(platformType == PlatformType.PLATFORM_FACEBOOK){
			titleLabel.text = "Share to Facebook";
		}else if(platformType == PlatformType.PLATFORM_TWITTER){
			titleLabel.text = "Share to Twitter";
		}else if(platformType == PlatformType.PLATFORM_LINKEDIN){
			titleLabel.text = "Share to Linkedin";
		}else if(platformType == PlatformType.PLATFORM_SINAWEIBO){
			titleLabel.text = "Share to SinaWeibo";
		}else if(platformType == PlatformType.PLATFORM_TENCENTWEIBO){
			titleLabel.text = "Share to TencentWeibo";
		}else if(platformType == PlatformType.PLATFORM_RENREN){
			titleLabel.text = "Share to Renren";
		}else if(platformType == PlatformType.PLATFORM_KAIXIN){
			titleLabel.text = "Share to Kaixin";
		}else if(platformType == PlatformType.PLATFORM_WEIXIN){
			titleLabel.text = "Share to Weixin";
			NGUITools.SetActive(option, true);
		}else if(platformType == PlatformType.PLATFORM_QZONE){
			titleLabel.text = "Share to QZone";
		}
		logLabel.text = "";
		sendButton.isEnabled = true;
	}
	
	void RegisterOpenPlatforms()
	{
		Facebook.instance.OnCallBack += OnShareCallBack;
		Twitter.instance.OnCallBack += OnShareCallBack;
		SinaWeibo.instance.OnCallBack += OnShareCallBack;
		TencentWeibo.instance.OnCallBack += OnShareCallBack;
		Renren.instance.OnCallBack += OnShareCallBack;
		Kaixin.instance.OnCallBack += OnShareCallBack;
		Linkedin.instance.OnCallBack += OnShareCallBack;
		Weixin.instance.OnCallBack += OnShareCallBack;
		QZone.instance.OnCallBack += OnShareCallBack;
	}
	
	void OnButtonSend(GameObject button)
	{
		string shareText = string.Format("{0} create at {1}", contentLabel.text, System.DateTime.Now.ToString());
		if(!checkBox.isChecked){
			if(platformType == PlatformType.PLATFORM_FACEBOOK){
				Facebook.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_TWITTER){
				Twitter.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_SINAWEIBO){
				SinaWeibo.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_RENREN){
				Renren.instance.Share(shareText);
				SinaWeibo.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_TENCENTWEIBO){
				TencentWeibo.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_KAIXIN){
				Kaixin.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_LINKEDIN){
				Linkedin.instance.Share(shareText);
			}else if(platformType == PlatformType.PLATFORM_WEIXIN){
				Weixin.instance.Share("Title", shareText, null, "http://unisharekit.com", wxScene);
			}else if(platformType == PlatformType.PLATFORM_QZONE){
				QZone.instance.Share("title", "http://unisharekit.com", shareText);
			}		
		}else{
			if(platformType == PlatformType.PLATFORM_FACEBOOK){
				Facebook.instance.ShareWithImage(shareText, "name", url, "caption", "description", imgUrl);
			}else if(platformType == PlatformType.PLATFORM_TWITTER){
				Twitter.instance.ShareWithImage(shareText, imgPath);
			}else if(platformType == PlatformType.PLATFORM_RENREN){
				Renren.instance.ShareWithImage("name", "description", url, shareText, imgUrl);
			}else if(platformType == PlatformType.PLATFORM_SINAWEIBO){
				SinaWeibo.instance.ShareWithImage(shareText, imgPath);
			}else if(platformType == PlatformType.PLATFORM_TENCENTWEIBO){
				TencentWeibo.instance.ShareWithImage(shareText, imgPath);
			}else if(platformType == PlatformType.PLATFORM_KAIXIN){
				Kaixin.instance.ShareWithImage(shareText, imgPath);
			}else if(platformType == PlatformType.PLATFORM_LINKEDIN){
				Linkedin.instance.Share("comment", "title", shareText, url, imgUrl);
			}else if(platformType == PlatformType.PLATFORM_WEIXIN){
				Weixin.instance.ShareWithImage(imgPath, wxScene);
			}else if(platformType == PlatformType.PLATFORM_QZONE){
				QZone.instance.Share("title", "http://unisharekit.com", shareText, imgUrl, "1");
			}			
		}
		if(platformType!=PlatformType.PLATFORM_WEIXIN){
			sendButton.isEnabled = false;
		}
		logLabel.text = "Sending, Please wait...";
	}
	
	void OnButtonBack(GameObject button)
	{
		NGUITools.SetActive(uiShareWindow, true);
		NGUITools.SetActive(uiSendWindow, false);
	}	
	
	void OnShareCallBack(ResponseResult res)
	{
		
		string strRes;
		if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
			strRes = string.Format("Share to {0} success.", platformType.ToString());
		}else{
			strRes = string.Format("Share to {0} failed.\nDetail: {1}", platformType.ToString(), res.description);
		}
		logLabel.text = strRes;
		
		Debug.Log(string.Format("sendButton.enabled = {0}", sendButton.isEnabled.ToString()));
		sendButton.isEnabled = true;
	}
	
	void OnWXSceneChanged(bool isChecked){
		string currentCheckBoxName = UICheckbox.current.name;
		if(currentCheckBoxName == "Checkbox - Session"){
			wxScene = WXScene.WXSceneSession;
		}else if(currentCheckBoxName == "Checkbox - Timeline"){
			wxScene = WXScene.WXSceneTimeline;
		}
	}
	
	
}
