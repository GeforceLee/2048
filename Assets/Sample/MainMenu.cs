using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		int width = 150;
		int height = 60;
		int posX = Screen.width/2 - width/2;
		int posY = 60;
			
		if(GUI.Button(new Rect(posX, posY, width, height), "DemoScene")){
			Application.LoadLevel("SceneDemo");
		}
		posY += 100;
		
		if(GUI.Button(new Rect(posX, posY, width, height), "MultiShare")){
			Application.LoadLevel("SceneMultiShare");
		}
		
		posY += 100;
		
		if(GUI.Button(new Rect(posX, posY, width, height), "UniShareSample")){
			Application.LoadLevel("SceneUniShareSample");
		}
		
		posY += 100;
		
		if(GUI.Button(new Rect(posX, posY, width, height), "TestFoursquare")){
			Application.LoadLevel("SceneTestFoursquare");
		}
		
		posY += 100;
		
		if(GUI.Button(new Rect(posX, posY, width, height), "TestGoolgPlus")){
			Application.LoadLevel("SceneTestGoolgPlus");
		}
		
		posY += 100;
	}
}
