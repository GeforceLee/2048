using UnityEngine;
using System.Collections;
using UniShare;
public class AccountManagerWindow : MonoBehaviour {
	public UIButton btnBindFacebook;
	public UIButton btnBindTwitter;
	public UIButton btnBindSinaWeibo;
	public UIButton btnBindTencentWeibo;
	public UIButton btnBindLinkedin;
	public UIButton btnBindRenren;
	public UIButton btnBindKaixin;
	public UIButton btnBindQZone;
	
	GameObject uiShareWindow;
	GameObject uiSendWindow;
	GameObject uiAccountManagerWindow;
	GameObject uiIndicator;
	// Use this for initialization
	void Start () {
		uiShareWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelShare");
		uiSendWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelSend");
		uiAccountManagerWindow = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager");
		uiIndicator = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelWaitingIndicator");
		
		GameObject btnBack = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ButtonBackToShare");
		UIEventListener.Get(btnBack).onClick = OnButtonBack;
		
		GameObject btnSinaWeibo = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemSinaWeibo/ButtonSinaWeibo");
		UIEventListener.Get(btnSinaWeibo).onClick = OnSocialButtonClick;
		GameObject btnFacebook = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemFacebook/ButtonFacebook");
		UIEventListener.Get(btnFacebook).onClick = OnSocialButtonClick;
		GameObject btnKaixin = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemKaixin/ButtonKaixin");
		UIEventListener.Get(btnKaixin).onClick = OnSocialButtonClick;
		GameObject btnRenren = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemRenren/ButtonRenren");
		UIEventListener.Get(btnRenren).onClick = OnSocialButtonClick;
		GameObject btnTwitter= GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemTwitter/ButtonTwitter");
		UIEventListener.Get(btnTwitter).onClick = OnSocialButtonClick;
		GameObject btnTencentWeibo = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemTencentWeibo/ButtonTencentWeibo");
		UIEventListener.Get(btnTencentWeibo).onClick = OnSocialButtonClick;
		GameObject btnLinkedin = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemLinkedin/ButtonLinkedin");
		UIEventListener.Get(btnLinkedin).onClick = OnSocialButtonClick;
		GameObject btnQZone = GameObject.Find("UI Root (2D)/Camera/Anchor/PanelAccountManager/ItemQZone/ButtonQZone");
		UIEventListener.Get(btnQZone).onClick = OnSocialButtonClick;
		
		RegisterOpenPlatforms();
		UpdateState();
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
	
	void UpdateButtonState(OpenPlatformBase platform, UIButton btn)
	{
		UILabel btnLabel = btn.GetComponentInChildren<UILabel>();
		btnLabel.text = platform.IsBinded?"Logout":"Login";
	
			
		UISprite bgSprit = btn.GetComponentInChildren<UISprite>();
		bgSprit.color = platform.IsBinded?new Color(106.0f/255, 133.0f/255, 240.0f/255):new Color(7.0f/255, 196.0f/255, 69.0f/255);
		btn.defaultColor = platform.IsBinded?new Color(106.0f/255, 133.0f/255, 240.0f/255):new Color(7.0f/255, 196.0f/255, 69.0f/255);
		// force button update color immediately
		btn.enabled = false;
		btn.enabled = true;
	}
	
	public void UpdateState()
	{
		UpdateButtonState(Facebook.instance, btnBindFacebook);
		UpdateButtonState(Twitter.instance, btnBindTwitter);
		UpdateButtonState(SinaWeibo.instance, btnBindSinaWeibo);
		UpdateButtonState(TencentWeibo.instance, btnBindTencentWeibo);
		UpdateButtonState(Linkedin.instance, btnBindLinkedin);
		UpdateButtonState(Kaixin.instance, btnBindKaixin);
		UpdateButtonState(Renren.instance, btnBindRenren);	
		UpdateButtonState(QZone.instance, btnBindQZone);	
	}
	
	void OnButtonBack(GameObject button)
	{
		NGUITools.SetActive(uiShareWindow, true);
		NGUITools.SetActive(uiAccountManagerWindow, false);
	}
	
	void OnSocialButtonClick(GameObject button)
	{
    	Debug.Log("GameObject " + button.name);
		
		if(button.name == "ButtonFacebook"){
			if(Facebook.instance.IsBinded){
				Facebook.instance.Logout();
				UpdateButtonState(Facebook.instance, btnBindFacebook);
			}else{
				ShowIndicator(true);
				Facebook.instance.Authorize();
			}
		}
		else if(button.name == "ButtonSinaWeibo"){
			if(SinaWeibo.instance.IsBinded){
				SinaWeibo.instance.Logout();
				UpdateButtonState(SinaWeibo.instance, btnBindSinaWeibo);
			}else{
				ShowIndicator(true);
				SinaWeibo.instance.Authorize();
			}
		}
		else if(button.name == "ButtonRenren"){
			if(Renren.instance.IsBinded){
				Renren.instance.Logout();
				UpdateButtonState(Renren.instance, btnBindRenren);	
			}else{
				ShowIndicator(true);
				Renren.instance.Authorize();
			}
		}
		else if(button.name == "ButtonKaixin"){
			if(Kaixin.instance.IsBinded){
				Kaixin.instance.Logout();
				UpdateButtonState(Kaixin.instance, btnBindKaixin);
			}else{
				ShowIndicator(true);
				Kaixin.instance.Authorize();
			}
		}else if(button.name == "ButtonTwitter"){
			if(Twitter.instance.IsBinded){
				Twitter.instance.Logout();
				UpdateButtonState(Twitter.instance, btnBindTwitter);
			}else{
				ShowIndicator(true);
				Twitter.instance.Authorize();
			}
		}else if(button.name == "ButtonLinkedin"){
			if(Linkedin.instance.IsBinded){
				Linkedin.instance.Logout();
				UpdateButtonState(Linkedin.instance, btnBindLinkedin);
			}else{
				ShowIndicator(true);
				Linkedin.instance.Authorize();
			}
		}else if(button.name == "ButtonTencentWeibo"){
			if(TencentWeibo.instance.IsBinded){
				TencentWeibo.instance.Logout();
				UpdateButtonState(TencentWeibo.instance, btnBindTencentWeibo);
			}else{
				ShowIndicator(true);
				TencentWeibo.instance.Authorize();
			}
		}else if(button.name == "ButtonQZone"){
			if(QZone.instance.IsBinded){
				QZone.instance.Logout();
				UpdateButtonState(QZone.instance, btnBindQZone);
			}else{
				ShowIndicator(true);
				QZone.instance.Authorize();
			}
		}
		
	}
	
	void oauthCallback(PlatformType pType, bool success)
	{
		ShowIndicator(false);
		if(success){
			if(!uiShareWindow.active){
				if(pType == PlatformType.PLATFORM_FACEBOOK){
					UpdateButtonState(Facebook.instance, btnBindFacebook);
				}else if(pType == PlatformType.PLATFORM_TWITTER){
					UpdateButtonState(Twitter.instance, btnBindTwitter);
				}else if(pType == PlatformType.PLATFORM_SINAWEIBO){
					UpdateButtonState(SinaWeibo.instance, btnBindSinaWeibo);
				}else if(pType == PlatformType.PLATFORM_TENCENTWEIBO){
					UpdateButtonState(TencentWeibo.instance, btnBindTencentWeibo);
				}else if(pType == PlatformType.PLATFORM_LINKEDIN){
					UpdateButtonState(Linkedin.instance, btnBindLinkedin);
				}else if(pType == PlatformType.PLATFORM_RENREN){
					UpdateButtonState(Renren.instance, btnBindRenren);
				}else if(pType == PlatformType.PLATFORM_KAIXIN){
					UpdateButtonState(Kaixin.instance, btnBindKaixin);
				}else if(pType == PlatformType.PLATFORM_QZONE){
					UpdateButtonState(QZone.instance, btnBindQZone);
				}	
			}
		}
	}
	public void ShowIndicator(bool isShow)
	{
		NGUITools.SetActive(uiIndicator, isShow);		
	}		
}
