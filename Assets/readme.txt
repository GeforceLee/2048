UniShare v1.3.7
-Add QZone support
-Update to latest Wechat iOS SDK, fixed iOS7 issue.
-------------------------------------------------------------------------------------------
UniShare v1.3.6
-Fixed SinaWeibo return share success when network disabled
-Fixed TencentWeibo share failed after next launch(OpenID not saved)
-------------------------------------------------------------------------------------------
UniShare v1.3.5
-Fixed TencentWeibo issue
-Update Renren Api to v2.0
-Add Facebook uploading image api
-------------------------------------------------------------------------------------------
UniShare v1.3.4
-Fixed "Cookie no enable" error with facebook login on Mac Editor/Standalone
-Optimize android webview call method, easy combine with other Android plugin.
-Add stripping level support
-Fixed webview crash on windows 64bits
-------------------------------------------------------------------------------------------
UniShare v1.3
-Add Wechat(н╒пе) support
-Add a complete sample showing how to integrate with NGUI, which can be used in your project directly with only small change
-Fixed crash after authorizing with some specific callbackurl on Mac Standalone
-Modify OnOauthCallBack, add two paramaters, PlatformType pType, bool success, And it will be also called if user close the webview.
-------------------------------------------------------------------------------------------
UniShare v1.2
-Added Windows Editor/Standalone support
-Fixed sinaweibo authorizing issue(error code 21337)
-Fixed iOS/ Mac Standalone logout
-Fixed crash on android under auto rotation build setting
--------------------------------------------------------------------------------------
UniShare v1.1.1
-Fixed iOS building error
-Fixed bugs on Mac Editor and Mac standalone 
-Twitter bug fixed:{"errors":[{"message":"Could not authenticate you","code":32}]}
--------------------------------------------------------------------------------------
UniShare v1.1
-Mac Editor and Mac Standalone support
-End session support(Logout)
-Twitter bug fix
--------------------------------------------------------------------------------------
UniShare v1.0.5
-Bug Fix: Incorrect return value in VerifierAccessToken();
-Bug Fix: PlayerPrefers been cleared on Start();
-Add another example show how to share with only one click.
-------------------------------------------------------------------------------------
UniShare v1.0.4
-Fixed a bug when using http Get;
-Update twitter request_token url from "http://twitter.com/oauth/request_token" to "https://api.twitter.com/oauth/request_token";Use post instead of get;
-Add Facebook API "https://graph.facebook.com/me" for getting user info
-Add Android side source code.
-----------------------------------------------------------------------------------
UniShare v1.0.2
-Fixed Api Compatibility Level conflict, now support default setting: .NET 2.0 Subset
-----------------------------------------------------------------------------------

UniShare is a share toolkit for Unity3d. It current supports sharing to Facebook, Twitter, Linkedin, SinaWeibo, TencentWeibo, Kaixin, Renren, more platforms will be added in the future.

----------------------------------------------------------------------------------------------
Features:
-Crossplatform: one solution for both iOS and Android.
-Multiple social platform support: Facebook, Twitter, Linkedin, SinaWeibo, TencentWeibo, Kaixin, Renren,Wechat(н╒пе) more platforms will be added in the future.
-Build-in webview: UniShare use build-in webview for authorizing and login.
-Flexibility: easy to extend, add more API to current platforms, if you want to add other social platforms, please contact me. 
-Full support for OAuth1.0 and 2.0;

----------------------------------------------------------------------------------------------
How to use:
1) Import plugins to your Unity project
2) Add prefabs loacation at Plugins/Prefabs to Hierarchy(Do not change the prefab names!);
3) Set parameters of instances: Appkey, AppSecret, CallbackUrl in the inspector. You need to register apps on social platform.Make sure the callbackurl are the same both on app setting and unishare side.
Registry Guide: http://unisharekit.com/register_guide
4) Call API: eg: Facebook.instance.Authorize(); See the UniShareSample demo.
5) For windows Editor/Standalone: You need to download additional files: https://www.dropbox.com/s/u9r18zx7ndcm9m8/unisharewindows.rar
Unzip to your unity project directoy, the directoy will looks like:
YourPorjectName
	-Assets
	-Library
	...
	-unisharewindows
		-UniShareWebView.exe
		-Xilium.CefGlue.dll
		...

Note: 
1.Need Unity Pro.
2.AndroidMacSrc.rar is source code for Android and Mac side. It is for guys who want extending Android/Mac native side. Please remove it from unity project when building unity app.
3.UniShare is just API collection without UI other than webview, so it leaves you high capacity to custom your own UI on Unity side, the SceneDemoUsingNGUI in the plugin shows how to integrate UniShare with NGUI, you can also integrate with any other UI plugins.You'd better take a look at this sample first. 
4.It is recommanded to ask question on UniShare forum(http://unisharekit.com/forum) instead of email.

Full documentation please visit http://unisharekit.com/node/9
e-mail : unishare@126.com


