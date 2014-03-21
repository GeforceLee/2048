using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#if UNITY_IPHONE
using UnityEditor.Callbacks;
#endif
#endif

using System.Collections;
using System;
using System.Diagnostics;
public class CustomPostProcessScript : MonoBehaviour {
#if UNITY_EDITOR
	#if UNITY_IPHONE	
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject)
	{
		UnityEngine.Debug.Log("---Begin Execute Custome OnPostprocessBuild---"); 
		Process myCustomProcess = new Process();		
		myCustomProcess.StartInfo.FileName = "python";
		string weixinUrlScheme = Weixin.instance.appKey;
		string unityVersion = Application.unityVersion;
		//UnityEngine.Debug.Log("Facebook.instance.callbackUrl=" + Facebook.instance.callbackUrl);
        //string objCPath = "test";
		myCustomProcess.StartInfo.Arguments = string.Format("Assets/Plugins/UniShare/Editor/CustomPostProcessPy.py \"{0}\" \"{1}\" \"{2}\"", pathToBuildProject, unityVersion, weixinUrlScheme);
		myCustomProcess.StartInfo.UseShellExecute = false;
        myCustomProcess.StartInfo.RedirectStandardOutput = false;
		myCustomProcess.Start(); 
		myCustomProcess.WaitForExit();
		UnityEngine.Debug.Log("---End Execute Custome OnPostprocessBuild---");  		
	}
	#endif
#endif
}
