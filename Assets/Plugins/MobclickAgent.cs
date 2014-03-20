using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using LitJson; // http://litjson.sourceforge.net/


public sealed class MobclickAgent:MonoBehaviour {

#if UNITY_ANDROID	
	private const string WRAPPER_SDK_TYPE = "Unity";
	private const string WRAPPER_SDK_VERSION = "1.0";
#endif
	
#if UNITY_IPHONE
	void updateCallBack(String jsonHash)
	{
		//Debug.Log ("Hello", gameObject);
		Debug.Log("Implement Callback "+jsonHash);
	}
#endif
	void Start ()
	{
#if UNITY_IPHONE
//ReportPolicy		
//REALTIME = 0,       //send log when log created
//BATCH = 1,          //send log when app launch
//SENDDAILY = 4,      //send log every day's first launch
//SENDWIFIONLY = 5    //send log when wifi connected
		
		MobclickAgent.StartWithAppKeyAndReportPolicyAndChannelId("4f3122f152701529bb000042",0,"Develop");
		MobclickAgent.SetAppVersion("1.2.1");
		MobclickAgent.SetLogSendInterval(20);
		JsonData eventAttributes = new JsonData();
		eventAttributes["username"] = "Aladdin";
		eventAttributes["company"] = "Umeng Inc.";
		
		MobclickAgent.EventWithAttributes("GameState",JsonMapper.ToJson(eventAttributes));
		MobclickAgent.SetLogEnabled(true);
		MobclickAgent.SetCrashReportEnabled(true);
		MobclickAgent.CheckUpdate();
		MobclickAgent.UpdateOnlineConfig();
		MobclickAgent.Event("GameState");
		MobclickAgent.BeginEventWithLabel("New-GameState","identifierID");
		MobclickAgent.EndEventWithLabel("New-GameState","identifierID");
#elif UNITY_ANDROID
		MobclickAgent.setLogEnabled(true);
		
		MobclickAgent.onResume();
		
		
// Android: can't call onEvent just before onResume is called, 'can't call onEvent before session is initialized' will be print in eclipse logcat
// Android: call MobclickAgent.onPause(); when Application exit.
#endif
	}
	public void Dispose()
	{
#if UNITY_ANDROID
		Agent.Dispose();
		Context.Dispose();
#endif
	}
#if	UNITY_ANDROID
	//lazy initialize singleton
	static class SingletonHolder {
    	public static AndroidJavaClass instance_mobclick;
		public static AndroidJavaObject instance_context;
		
		static SingletonHolder()
		{
			instance_mobclick = new AndroidJavaClass("com.umeng.analytics.MobclickAgent");
			instance_mobclick.CallStatic("setWrapper", WRAPPER_SDK_TYPE, WRAPPER_SDK_VERSION);
			
			using(AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
			{
				instance_context = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}
  	}
	
	static AndroidJavaClass Agent
	{
		get {
			return SingletonHolder.instance_mobclick;
		}
	}
	
	static AndroidJavaObject Context
	{
		get {
			return SingletonHolder.instance_context;
		}
	}
	
	public static void onResume()
	{
		Agent.CallStatic("onResume", Context);
	}
	
	public static void onPause()
	{
		Agent.CallStatic("onPause", Context);
	}
	
	public static void onEvent(string tag)
	{
		Agent.CallStatic("onEvent", Context, tag);
	}
	
	public static void onEvent(string tag, string label)
	{
		Agent.CallStatic("onEvent", Context, tag, label);
	}
	
	public static void onEvent(string tag, string label, int acc)
	{
		Agent.CallStatic("onEvent", Context, tag, label, acc);
	}
	
	public static void onEvent(string id, Dictionary<string,string> dic)
	{
		Agent.CallStatic("onEvent", Context, id, ToJavaHashMap( dic) );
	}
	
	public static void onEventDuration(string id, long duration)
	{
		Agent.CallStatic("onEventDuration", Context, id, duration);
	}
	
	public static void onEventDuration(string id, string label, long duration)
	{
		Agent.CallStatic("onEventDuration", Context, id, label, duration);
	}
	
	public static void onEventDuration(string id, Dictionary<string,string> dic, long duration)
	{		
		Agent.CallStatic("onEventDuration", Context, id, ToJavaHashMap(dic), duration);
	}
	
	public static void setContinueSessionMillis(long milliseconds)
	{
		Agent.CallStatic("setSessionContinueMillis", milliseconds);
	}
	
	public static void setLogEnabled(bool enabled)
	{
		Agent.CallStatic("setDebugMode", enabled);
	}
	
	public static void flush()
	{
		Agent.CallStatic("flush",Context);
	}
	
	public static void setEnableLocation(bool reportLocation)
	{
		Agent.CallStatic("setAutoLocation", reportLocation);
	}
	
	private static AndroidJavaObject ToJavaHashMap(Dictionary<string,string> dic)
	{
		var hashMap = new AndroidJavaObject( "java.util.HashMap" );
		var putMethod = AndroidJNIHelper.GetMethodID( hashMap.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;" );
		var arguments = new object[2];
		foreach( var entry in dic )
		{
			using( var key = new AndroidJavaObject( "java.lang.String", entry.Key ) )
			{
				using( var val = new AndroidJavaObject( "java.lang.String", entry.Value ) )
				{
					arguments[0] = key;
					arguments[1] = val;
					AndroidJNI.CallObjectMethod( hashMap.GetRawObject(), putMethod, AndroidJNIHelper.CreateJNIArgArray( arguments ) );
				}
			} // end using
		} // end foreach
		
		return hashMap;
	}
	
#endif

#if UNITY_IPHONE
	//set Your App's Version
	public static void SetAppVersion(String value){
		_SetAppVersion(value);
	}
	//get Umeng Analytics SDK's Version
	public static String GetAgentVersion(){
		return _GetAgentVersion();
	}
	
	//set Umeng Analytics SDK Log switch
	public static void SetLogEnabled(bool value){
		_SetLogEnabled(value);
	}
	
	//set Umeng Analytics SDK Crash log switch
	public static void SetCrashReportEnabled(bool value){
		_SetCrashReportEnabled(value);
	}
	
	//start Umeng Analytics SDK session with your appkey,ReportPolicy,ChanneId
	public static void StartWithAppKeyAndReportPolicyAndChannelId(String appkey,int policy,String channelId){
		_StartWithAppKeyAndReportPolicyAndChannelId(appkey,policy,channelId);
	}
	
	//set send log interval time when using SEND_INTERVAL policy,
	public static void SetLogSendInterval(double interval){
		_SetLogSendInterval(interval);
	}

	//record a event log with a eventId
	public static void Event(String eventId){
		_Event(eventId);
	}
	
	//record a event log with a eventId
	public static void EventWithLabel(String eventId, String label){
		_EventWithLabel(eventId,label);
	}
	
	//record a event log with a eventId and times
	public static void EventWithAccumulation(String eventId, int accumulation){
		_EventWithAccumulation(eventId,accumulation);
	}
	
	//record a event log with a eventId, a label and times
	public static void EventWithLabelAndAccumulation(String eventId, String label,int accumulation){
		_EventWithLabelAndAccumulation(eventId,label,accumulation);
	}
	
	public static void EventWithAttributes(String eventId, String jsonStr){
		_EventWithAttributes(eventId,jsonStr);
	}

	public static void BeginEventWithLabel(String eventId,String label){
		_BeginEventWithLabel(eventId,label);
	}

	public static void EndEventWithLabel(String eventId,String label){
		_EndEventWithLabel(eventId,label);
	}

	public static void BeginEventWithPrimarykeyAndAttributes(String eventId,String primaryKey,String jsonString){
		_BeginEventWithPrimarykeyAndAttributes(eventId,primaryKey,jsonString);
	}

	public static void EndEventWithPrimarykey(String eventId,String primaryKey){
		_EndEventWithPrimarykey(eventId,primaryKey);
	}
	
	//Check your app's update info. you should set new version's info on umeng's web portal
	public static void CheckUpdate(){
		_CheckUpdate();
	}
	
	//If your app You can change the default alert view's title and button's title
	public static void CheckUpdateAndCancelButtonTitleAndOtherButtonTitles(String title, String cancelBtnTitle,String otherBtnTitle){
		_CheckUpdateAndCancelButtonTitleAndOtherButtonTitles(title,cancelBtnTitle,otherBtnTitle);
	}
	
	//update your online configuration's info 
	public static void UpdateOnlineConfig(){
		_UpdateOnlineConfig();
	}
	
	//After update your online configuration's info, get any key's value
	public static String GetConfigParamsForKey(String key){
		return	_GetConfigParamsForKey(key);
	}
	
	//After update your online configuration's info, get all key and value
	public static JsonData GetConfigParams(){
		String jsonStr = _GetConfigParams();
		if(jsonStr!=null){
			return JsonMapper.ToObject(jsonStr);
		}
		return null;
	}
	
	//
	public static void LogPageViewWithSeconds(String pageName,int seconds){
		_LogPageViewWithSeconds(pageName,seconds);
	}
	
	public static void BeginLogPageView(String pageName){
		_BeginLogPageView(pageName);
	}
	
	public static void EndLogPageView(String pageName){
		_EndLogPageView(pageName);
	}
	
	//check if user's device JailBroken
	public static bool IsJailBroken(){
		return _IsJailBroken();
	}
	
	//Check if your App pirated
	public static bool IsPirated ()
	{
		return _IsPirated ();
	}
	
	[DllImport ("__Internal")]
	public static extern void _SetAppVersion(String value);
	
	[DllImport ("__Internal")]
	public static extern String _GetAgentVersion();
		
	[DllImport ("__Internal")]
	public static extern void _SetLogEnabled(bool value);
		
	[DllImport ("__Internal")]
	public static extern void _SetCrashReportEnabled(bool value);
	
	[DllImport ("__Internal")]
	public static extern void _StartWithAppKeyAndReportPolicyAndChannelId(String appkey,int policy,String channelId);
	
	[DllImport ("__Internal")]
	public static extern void _SetLogSendInterval(double interval);
		
	[DllImport ("__Internal")]
	public static extern void _Event(String eventId);
		
	[DllImport ("__Internal")]
	public static extern void _EventWithLabel(String eventId, String label);
		
	[DllImport ("__Internal")]
	public static extern void _EventWithAccumulation(String eventId,int accumulation);
	
	[DllImport ("__Internal")]
	public static extern void _EventWithLabelAndAccumulation(String eventId, String label,int accumulation);
	
	[DllImport ("__Internal")]
	public static extern void _EventWithAttributes(String eventId, String jsonString);
	
	[DllImport ("__Internal")]
	public static extern void _BeginEventWithLabel(String eventId,String label);
	
	[DllImport ("__Internal")]
	public static extern void _EndEventWithLabel(String eventId,String label);
		
	[DllImport ("__Internal")]
	public static extern void _BeginEventWithPrimarykeyAndAttributes(String eventId,String primaryKey,String jsonString);
	
	[DllImport ("__Internal")]
	public static extern void _EndEventWithPrimarykey(String eventId,String primaryKey);
	
	[DllImport ("__Internal")]
	public static extern void _CheckUpdate();
		
	[DllImport ("__Internal")]
	public static extern void _CheckUpdateAndCancelButtonTitleAndOtherButtonTitles(String title, String cancelBtnTitle,String otherBtnTitle);
	
	[DllImport ("__Internal")]
	public static extern void _UpdateOnlineConfig();
		
	[DllImport ("__Internal")]
	public static extern String _GetConfigParamsForKey(String key);
	
	[DllImport ("__Internal")]
	public static extern String _GetConfigParams();
	
	[DllImport ("__Internal")]
	public static extern void _LogPageViewWithSeconds(String pageName,int seconds);
		
	[DllImport ("__Internal")]
	public static extern void _BeginLogPageView(String pageName);
		
	[DllImport ("__Internal")]
	public static extern void _EndLogPageView(String pageName);
	
	[DllImport ("__Internal")]
	public static extern bool _IsJailBroken();
	
	[DllImport ("__Internal")]
	public static extern bool _IsPirated();	
#endif	

}
