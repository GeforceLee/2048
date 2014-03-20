using UnityEngine;
using System.Collections;

public class UMeng : MonoBehaviour {

	// Use this for initialization
	void Start () {
		#if UNITY_IPHONE
		//ReportPolicy       
		//SEND_REALTIME = 0     //send log when log created
		//SEND_LAUNCH = 1       //send log when app launch
		//SEND_ON_EXIT = 2  //send log when app exit
		//SEND_DAILY = 4        //send log every day's first launch
		//SEND_ONLY_WIFI = 5    //send log when wifi connected
		//SEND_INTERVAL = 6 //send log after a time interval
		
		MobclickAgent.StartWithAppKeyAndReportPolicyAndChannelId("532ac9d156240b2cea0044c4",5,"iOS");
		MobclickAgent.SetAppVersion("1.0");
		MobclickAgent.SetLogSendInterval(20);
//		JsonData eventAttributes = new JsonData();
//		eventAttributes["username"] = "Aladdin";
//		eventAttributes["company"] = "Umeng Inc.";
//		
//		MobclickAgent.EventWithAttributes("GameState",JsonMapper.ToJson(eventAttributes));
//		MobclickAgent.SetLogEnabled(true);
//		MobclickAgent.SetCrashReportEnabled(true);
//		MobclickAgent.CheckUpdate();
//		MobclickAgent.UpdateOnlineConfig();
//		MobclickAgent.Event("GameState");
		#elif UNITY_ANDROID
		
		//MobclickAgent.setLogEnabled(true);
		MobclickAgent.onResume();
		
		// Android: call MobclickAgent.onPause(); when Application exit.
		#endif
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
