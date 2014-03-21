using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniShare;
using System.IO;
using UniShare.Json;

public class MultiShare : MonoBehaviour {
	// texture for bind button
	public Texture bindTexture;
	// texture for unbind  button
	public Texture unbindTexture;
	// texture for share button
	public Texture shareTexture;
	// if set true, the full response string will be shown, otherwise only show return type
	public bool showDetailLog = false;

	// platforms array
	OpenPlatformBase[] platforms;
	// array for platform logo image
	Texture[] platformLogo;
	// array for platform logo image under select
	Texture[] platformLogoChecked;

	Texture usedTexture;
	Texture displayBindTexture;
	// text to share 
	string shareText;
	// log text displayed on screen
	string logText;
	// default is false, true after inited
	bool isInited = false;
	// total count of responsed platform after shareing
	int returnCount;
	// selected platform count waiting to be share
	int checkedCount = 0;
	//  is current shareing?
	bool isSharing = false;
	// failed list, it saves all of response results of failed platforms
	protected List<ResponseResult> failedList = new List<ResponseResult>();
	// Use this for initialization
	void Start () {
		// init platform instance
		platforms = new OpenPlatformBase[7];
		platformLogo = new Texture[7];
		platformLogoChecked = new Texture[7];

		platforms[0] = Facebook.instance;
		platforms[1] = Twitter.instance;
		platforms[2] = Linkedin.instance;
		platforms[3] = SinaWeibo.instance;
		platforms[4] = Kaixin.instance;
		platforms[5] = Renren.instance;
		platforms[6] = TencentWeibo.instance;
		
		// load images based on platform names, these image locate at Assets/Sample/Resources, 
		// and named like: facebook.png facebook_checked.png
		for(int i=0; i<7; i++)
		{

			// get platform name and save to logoname
			string logoname = platforms[i].platformName;
			//Debug.Log(platforms[i]);
			// use Resources.Load to load platform texture
			Texture logo = Resources.Load(logoname) as Texture;
			string logoCheckedName = string.Format("{0}_checked", logoname);
			Texture logoChecked = Resources.Load(logoCheckedName) as Texture;
			// save logo texture to platformLogo array 
			platformLogo[i] = logo;
			// save checked logo texture to platformLogoChecked array
			platformLogoChecked[i] = logoChecked;

			// This line is very important, set the callback function,
			// which will be called after getting response from platform server
			platforms[i].OnCallBack += onShareCallBack;
		}
		
		// set initial sharetext
		shareText = "Post by UniShare(sharekit for Unity) http://u3d.as/content/chhren/uni-share";
		// set initial log text
		logText = "click bind button to bind social platforms, " +
				"click platform logo to check it on, only checked " +
				"platform will be shared to. Logs will be shown here," +
				 "check showDetailLog on if you want to see the log in more detail";
		// after successfully inited,  set isInited = true
		isInited = true;
	}
	
	// the actually share entry, will be called when share button clicked
	void Share()
	{
		// reset returnCount to 0 before shareing.
		returnCount = 0;
		// clear failedList
		failedList.Clear();
		// reset checkedCount to 0
		checkedCount = 0;
		// set logText to indicate shareing start
		logText = "start shareing...\n";
		// create text tails append after shareing text, to avoid duplicate posting
		// note: if you keep post the same text, some platforms may return context duplication error
		string sText = string.Format("{0} create at {1}", shareText, System.DateTime.Now.ToString());
		// go through every platform, only checked platform will be shared to.
		for(int i=0; i<platforms.Length; i++){
			if(platforms[i].IsSelected){
				// call paltform's Share api to actually share
				platforms[i].Share(sText);
				// gather checked platform count
				checkedCount++;
			}
		}

		if(checkedCount > 0){
			// change state
			isSharing = true;
		}else{
			// if no platform selected, simply remind user
			logText = "Please select at least one platform.";
		}
	}

	// This is the callback function, it will be call everytime when platform responsed, 
	// You should handle the response here.
	public void onShareCallBack(ResponseResult res){

		// log string
		string aPlatformLog = "";
		// log every shareing platform name and result
		if(res.returnType == ReturnType.RETURNTYPE_SUCCESS){
			aPlatformLog = string.Format("Share to {0} success!\n", res.platformType.ToString());
		}else{
			if(showDetailLog){
				aPlatformLog = string.Format("Share to {0} failed:{1}\n", 
				res.platformType.ToString(), 
				res.returnType.ToString());				
			}else{
				aPlatformLog = string.Format("Share to {0} failed:{1}|{2}\n", 
				res.platformType.ToString(), 
				res.returnType.ToString(),
				res.description.ToString());
			}
			// if failed, add ResoposeResult to failedList
			failedList.Add(res);
		}

		// print to log panel
		logText = string.Format("{0}{1}", logText, aPlatformLog);
		// add returned platform count 
		returnCount++;
		// if returnCount>=checkedCount, all platform return, sharing finished
		if(returnCount >= checkedCount){
			// change state
			isSharing = false;
		}
	}	
	


	void OnGUI(){
		//GUI.skin = skin;
		if(!isInited){
			return;
		}
		// shareText TextArea
		GUI.Label(new Rect(200, 0, 200, 20), "Multi-Share Demo:");		
		// shareText TextArea
		GUI.Label(new Rect(20, 20, 100, 20), "Share Text:");
		
		shareText = GUI.TextArea(new Rect(20, 40, 250, 60), shareText);
		if(isSharing){
			GUI.enabled = false;
		}	
		// the share button
		if(GUI.Button(new Rect(280, 40, 100, 36), shareTexture)){
			Share();
		}
		int startY = 120;
		int padding = 10;
		// platform logos and bind/unbind buttons
		for(int i=0; i<platforms.Length; i++){
			// only enabled after binded
			if(isSharing){
				GUI.enabled = false;
			}else{
				GUI.enabled = platforms[i].IsBinded;
			}
			usedTexture = platforms[i].IsSelected?platformLogoChecked[i]:platformLogo[i];
			if(GUI.Button(new Rect(40, startY, 64, 64), usedTexture)){
				platforms[i].IsSelected = !platforms[i].IsSelected;
			}
			// disable when shareing
			if(!isSharing){
				GUI.enabled = true;
			}
			displayBindTexture = platforms[i].IsBinded?unbindTexture:bindTexture;
			if(GUI.Button(new Rect(80 + usedTexture.width + padding, startY + 14, 100, 36), displayBindTexture)){
				//platform.isBinded = !platform.isBinded;
				// if binded, call logout to unbind
				if(platforms[i].IsBinded){
					platforms[i].Logout();
				}else{
					// reset IsSelected to false then call Authorize() to guide user to login.
					platforms[i].IsSelected = false;
					platforms[i].Authorize();
				}
			}
			
			startY += usedTexture.height + padding;			
		}
		// this is a log panel
		GUI.Label(new Rect(20, startY, 100, 20), "Log Panel:"); 
		logText = GUI.TextArea(new Rect(20, startY+20, 400, 160), logText);

	}
	
	void OnDestroy()
	{
		for(int i=0; i<7; i++)
		{
			platforms[i].OnCallBack -= onShareCallBack;
		}
	}	
}
