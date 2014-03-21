using UnityEngine;
using System.Collections;
using UniShare;
public class ShareWindow : MonoBehaviour {
	public SendWindow sendWIndow;
	public AccountManagerWindow accountManagerWindow; 
	GameObject uiShareWindow;
	GameObject uiSendWindow;
	GameObject uiAccountManagerWindow;
	GameObject uiIndicator;
	// Use this for initialization
	void Start () {
		GameObject btnSinaWeibo = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonSinaWeibo");
		UIEventListener.Get(btnSinaWeibo).onClick = OnSocialButtonClick;
		GameObject btnFacebook= GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonFacebook");
		UIEventListener.Get(btnFacebook).onClick = OnSocialButtonClick;
		GameObject btnKaixin = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonKaixin");
		UIEventListener.Get(btnKaixin).onClick = OnSocialButtonClick;
		GameObject btnRenren = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonRenren");
		UIEventListener.Get(btnRenren).onClick = OnSocialButtonClick;
		GameObject btnTwitter= GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonTwitter");
		UIEventListener.Get(btnTwitter).onClick = OnSocialButtonClick;
		GameObject btnTencentWeibo = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonTencentWeibo");
		UIEventListener.Get(btnTencentWeibo).onClick = OnSocialButtonClick;
		GameObject btnLinkedin = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonLinkedin");
		UIEventListener.Get(btnLinkedin).onClick = OnSocialButtonClick;
		GameObject btnWechat = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonWechat");
		UIEventListener.Get(btnWechat).onClick = OnSocialButtonClick;		
		GameObject btnQZone = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonQZone");
		UIEventListener.Get(btnQZone).onClick = OnSocialButtonClick;
		
		GameObject btnAccountManager = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare/ButtonAccountManager");
		UIEventListener.Get(btnAccountManager).onClick = OnButtonAccountManager;
		
		uiShareWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare");
		uiSendWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelSend");
		uiAccountManagerWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager");
		uiIndicator = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelWaitingIndicator");
		NGUITools.SetActive(uiSendWindow, false);
		NGUITools.SetActive(uiAccountManagerWindow, false);
		NGUITools.SetActive(uiIndicator, false);
		RegisterOpenPlatforms();
	}	
	
	void OnButtonAccountManager(GameObject button)
	{
		NGUITools.SetActive(uiAccountManagerWindow, true);
		NGUITools.SetActive(uiShareWindow, false);
		accountManagerWindow.UpdateState();
	}
	
	void OnSocialButtonClick(GameObject button)
	{
    	Debug.Log("GameObject " + button.name);
		
		if(button.name == "ButtonFacebook"){
			if(Facebook.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_FACEBOOK);
			}else{
				ShowIndicator(true);
				Facebook.instance.Authorize();
			}
			
		}else if(button.name == "ButtonTwitter"){
			if(Twitter.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_TWITTER);
			}else{
				ShowIndicator(true);
				Twitter.instance.Authorize();
			}
		}
		else if(button.name == "ButtonSinaWeibo"){
			if(SinaWeibo.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_SINAWEIBO);
			}else{
				ShowIndicator(true);
				SinaWeibo.instance.Authorize();
			}
		}
		else if(button.name == "ButtonRenren"){;
			if(Renren.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_RENREN);
			}else{
				ShowIndicator(true);
				Renren.instance.Authorize();
			}			
		}
		else if(button.name == "ButtonKaixin"){
			if(Kaixin.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_KAIXIN);
			}else{
				ShowIndicator(true);
				Kaixin.instance.Authorize();
			}
		}else if(button.name == "ButtonLinkedin"){
			if(Linkedin.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_LINKEDIN);
			}else{
				ShowIndicator(true);
				Linkedin.instance.Authorize();
			}
		}else if(button.name == "ButtonTencentWeibo"){
			if(TencentWeibo.instance.IsBinded){
				SendToPlatform(PlatformType.PLATFORM_TENCENTWEIBO);
			}else{
				ShowIndicator(true);
				TencentWeibo.instance.Authorize();
			}
		}else if(button.name == "ButtonWechat"){
			SendToPlatform(PlatformType.PLATFORM_WEIXIN);
		}else if(button.name == "ButtonQZone"){
			SendToPlatform(PlatformType.PLATFORM_QZONE);
		}
	}
	
	void RegisterOpenPlatforms()
	{
		Facebook.instance.OnOauthCallBack += oauthCallback;
		Twitter.instance.OnOauthCallBack += oauthCallback;
		SinaWeibo.instance.OnOauthCallBack += oauthCallback;
		TencentWeibo.instance.OnOauthCallBack += oauthCallback;
		Renren.instance.OnOauthCallBack += oauthCallback;
		Kaixin.instance.OnOauthCallBack += oauthCallback;
		Linkedin.instance.OnOauthCallBack += oauthCallback;
		QZone.instance.OnOauthCallBack += oauthCallback;
	}
	
	void UnregisterOpenPlatforms()
	{
		Facebook.instance.OnOauthCallBack -= oauthCallback;
		Twitter.instance.OnOauthCallBack -= oauthCallback;
		SinaWeibo.instance.OnOauthCallBack -= oauthCallback;
		TencentWeibo.instance.OnOauthCallBack -= oauthCallback;
		Renren.instance.OnOauthCallBack -= oauthCallback;
		Kaixin.instance.OnOauthCallBack -= oauthCallback;
		Linkedin.instance.OnOauthCallBack -= oauthCallback;
		QZone.instance.OnOauthCallBack -= oauthCallback;
	}	

	
	void SendToPlatform(PlatformType pType)
	{
		NGUITools.SetActive(uiSendWindow, true);
		NGUITools.SetActive(uiShareWindow, false);
		
		sendWIndow.ShowSendWindowWithPlatform(pType);
	}
	
	void oauthCallback(PlatformType pType, bool success)
	{
		ShowIndicator(false);
		if(success){
			if(uiShareWindow.active){
				SendToPlatform(pType);
			}
		}
	}
	
	public void ShowIndicator(bool isShow)
	{
		NGUITools.SetActive(uiIndicator, isShow);		
	}	
	
	void OnDestroy()
	{
		//UnregisterOpenPlatforms();
	}	
}
