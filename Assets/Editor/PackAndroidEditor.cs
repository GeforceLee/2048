using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;


public class PackAndroidEditor {
	//得到工程中所有场景名称
	static string[] SCENES = FindEnabledEditorScenes();







	//一系列批量build的操作
//	[MenuItem ("Custom/Build Android QQ")]
//	static void PerformAndroidQQBuild ()
//	{   
//		BulidTarget("QQ","Android");
//	}
//	
//	[MenuItem ("Custom/Build Android UC")]
//	static void PerformAndroidUCBuild ()
//	{   
//		BulidTarget("UC","Android");
//	}
//	
//	[MenuItem ("Custom/Build Android CMCC")]
//	static void PerformAndroidCMCCBuild ()
//	{   
//		BulidTarget("CMCC","Android");
//	}
//	
	[MenuItem ("Custom/Build Android ALL")]
	static void PerformAndroidALLBuild ()
	{   
		string[] publishids = new string[] {"qq","qihu","wandoujian","meizu","xiaomi","baidu","nineone","android","pconline"};
//		string[] publishids = new string[] {"pconline"};
		foreach(string s in publishids){
			BulidTarget(s,"Android");
		}

	}

	
	//这里封装了一个简单的通用方法。
	static void BulidTarget(string name,string target)
	{
		string app_name = name;
		string target_dir = Application.dataPath + "/TargetAndroid";
		string target_name = app_name + ".apk";
		BuildTargetGroup targetGroup = BuildTargetGroup.Android;
		BuildTarget buildTarget = BuildTarget.Android;
		string applicationPath = 	Application.dataPath.Replace("/Assets","");
		
		if(target == "Android")
		{
			target_dir = applicationPath + "/TargetAndroid"+"/"+PlayerSettings.bundleVersion;
			target_name = app_name + ".apk";
			targetGroup = BuildTargetGroup.Android;
		}
		if(target == "IOS")
		{
			target_dir = applicationPath + "/TargetIOS";
			target_name = app_name;
			targetGroup = BuildTargetGroup.iPhone;
			buildTarget = BuildTarget.iPhone;
		}
		
		//每次build删除之前的残留
		if(Directory.Exists(target_dir)) 
		{
			if (File.Exists(target_name))
			{
				File.Delete(target_name);
			}
		}else
		{
			Directory.CreateDirectory(target_dir); 
		}
		PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,name);  
		//==================这里是比较重要的东西=======================
//		switch(name)
//		{
//		case "QQ":
////			PlayerSettings.bundleIdentifier = "com.game.qq";
////			PlayerSettings.bundleVersion = "v0.0.1";
//			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,"QQ");  
//			break;
//		case "UC":
//
//			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,"UC");        				
//			break;
//		case "CMCC":
//
//			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup,"CMCC");        				
//			break;
//		}
		
		//==================这里是比较重要的东西=======================
		
		//开始Build场景，等待吧～
		GenericBuild(SCENES, target_dir + "/" + target_name, buildTarget,BuildOptions.None);
		
	}
	
	private static string[] FindEnabledEditorScenes() {
		List<string> EditorScenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (!scene.enabled) continue;
			EditorScenes.Add(scene.path);
		}
		return EditorScenes.ToArray();
	}
	
	static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
	{   
		EditorUserBuildSettings.SwitchActiveBuildTarget(build_target);
		string res = BuildPipeline.BuildPlayer(scenes,target_dir,build_target,build_options);
		
		if (res.Length > 0) {
			throw new Exception("BuildPlayer failure: " + res);
		}
	}

}


