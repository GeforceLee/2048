using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {

	private int currentScore = 0;
	private int hightestScore = 0;

	public GameObject tile;

	private Game _game;

	public GameObject scoreText;
	public GameObject hScoreText;

	public GameObject bgObject;

	public GameObject UI;

	Vector3 firstPostion;
	float tileBetween = 1.44f;

	public MyDirection lastDirection;


	public bool enableGameCenter = false;


	public float staticAngle = 40.0f;




	public AudioClip audio2_4;
	public AudioClip audio8_16_32_64;
	public AudioClip audio128_256_512;
	public AudioClip audio1024_2048;
	public AudioClip audioGame_Over;
	public AudioClip audioOff;
	// Use this for initialization
	void Start () {
		Social.localUser.Authenticate (ProcessAuthentication);
	}

	void ProcessAuthentication (bool success) {
		if (success) {
//			Debug.Log ("Authenticated, checking achievements");
			enableGameCenter = true;
			// Request loaded achievements, and register a callback for processing them

		}
//		else
//			Debug.Log ("Failed to authenticate");
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
	}
	void Awake(){
		float x = bgObject.transform.position.x-2.16f;
		
		float y = bgObject.transform.position.y+2.16f;
		
		firstPostion = new Vector3(x,y,0f);
		StartGame();
	}

	public void StartGame(){
		currentScore = 0;
		GameObject[] allTile = GameObject.FindGameObjectsWithTag("Tile");
		for(int i = 0;i<allTile.Length;i++){
			Destroy(allTile[i]);
		}
		int tempHightestScore = PlayerPrefs.GetInt("HightestScore");
		hightestScore = tempHightestScore;
		hScoreText.GetComponent<tk2dTextMesh>().text = tempHightestScore.ToString();
		scoreText.GetComponent<tk2dTextMesh>().text = "0";
		_game = gameObject.GetComponent<Game>();
		_game.setup(4);
		hideUI();

	}

	public Vector3 getTilePosition(int x,int y,int z=0){
		Vector3 result = new Vector3(firstPostion.x+x*tileBetween,firstPostion.y- y*tileBetween,z);
		
		return result;
	}


	void option(bool mov){
		if(!mov){
			// have not mov  play audio
			audio.clip = audioOff;
			audio.Play();
			return;
		}
		GameObject[] gameobjects =  GameObject.FindGameObjectsWithTag("TileText");


		GamePostion vector = _game.getVector(lastDirection);
		Hashtable traversals = _game.buildTraversals(vector);
		List<int> xList = (List<int>)traversals["x"];
		List<int> yList = (List<int>)traversals["y"];
		for(int ii=0;ii<4;ii++){
			int i = xList[ii];
			for(int jj=0;jj<4;jj++){
				int j = yList[jj];

				Tile t = _game.grid.cells[i,j];
				if(t != null){
					GameObject newTile;
					if(t.mergedFrom != null){
						int x1 = (int)t.mergedFrom[0].previousPosition["x"];
						int y1 = (int)t.mergedFrom[0].previousPosition["y"];
						GameObject perTile1 = GameObject.Find("Tile"+x1+y1);
						perTile1.GetComponent<TIleScript>().move(getTilePosition(i,j,1),t.mergedFrom[0].value);
						perTile1.name = "Tile";
						Destroy(perTile1,0.1f);
						int x2 = (int)t.mergedFrom[1].previousPosition["x"];
						int y2 = (int)t.mergedFrom[1].previousPosition["y"];
						GameObject perTile2 = GameObject.Find("Tile"+x2+y2);
						perTile2.GetComponent<TIleScript>().move(getTilePosition(i,j,0),t.value);
						perTile2.name = "Tile"+i+j;;
						if(t.value == 2||t.value == 4 ){
							audio.clip = audio2_4;
							audio.Play();
						}else if(t.value == 8||t.value == 16 || t.value == 32||t.value == 64){
							audio.clip = audio8_16_32_64;
							audio.Play();
						}else if(t.value == 128||t.value == 256 || t.value == 512){
							audio.clip = audio128_256_512;
							audio.Play();
						}else {
							audio.clip = audio1024_2048;
							audio.Play();
						}


					}else{
						if(t.previousPosition != null){
							int x = (int)t.previousPosition["x"];
							int y = (int)t.previousPosition["y"];
							newTile = GameObject.Find("Tile"+x+y);
							newTile.GetComponent<TIleScript>().move(getTilePosition(i,j),t.value);
							newTile.name = "Tile"+i+j;
//							Debug.Log("move  form change yuan x:"+x+" y:"+y +"  value:"+t.value +"  xian:"+"Tile"+i+j);
						}else{
							{
								newTile = Instantiate(tile,getTilePosition(i,j),Quaternion.identity) as GameObject;
								newTile.GetComponent<TIleScript>().setCurrentValue(t.value);
								newTile.name = "Tile"+i+j;
//								Debug.Log("new   x:"+i+" y:"+j +" value:"+t.value);
							}
						}
					}
				}

			}
		}

		currentScore = _game.score;
		if (currentScore > hightestScore) {
			PlayerPrefs.SetInt("HightestScore",currentScore);
			hightestScore = currentScore;
			hScoreText.GetComponent<tk2dTextMesh>().text = hightestScore.ToString();
			if(enableGameCenter){
				Social.ReportScore(hightestScore,"com.royalgame.2048.bestscore", result => {
//					if (result)
//						Debug.Log ("Successfully reported achievement progress");
//					else
//						Debug.Log ("Failed to report achievement");
				});
			}

		}
		scoreText.GetComponent<tk2dTextMesh>().text = currentScore.ToString();


		if(_game.over){
			audio.clip = audioGame_Over;
			audio.Play();
			showUI();
		}
	}


	public void showUI(){
		UI.transform.position = bgObject.transform.position;
	}
	public void hideUI(){
		UI.transform.position = new Vector3(10,10,0);
	}

	public void showGameCenterScore(){
		if(enableGameCenter){
			Social.ShowLeaderboardUI();
		}
	}

	void OnSwipe( SwipeGesture gesture ) 
	{
		
		if(_game.over){
			return;
		}
		FingerGestures.SwipeDirection direction = gesture.Direction;
		bool isMoved = false;
		if(Vector2.Angle(gesture.Move,Vector2.right) < staticAngle){
			lastDirection = MyDirection.DirectionRight;
			isMoved = true;
		}else if(Vector2.Angle(gesture.Move,Vector2.up) < staticAngle ){
			lastDirection = MyDirection.DirectionUp;
			isMoved = true;
		}else if(Vector2.Angle(gesture.Move,Vector2.right) >  180 - staticAngle ){
			lastDirection = MyDirection.DirectionLeft;
			isMoved = true;
		}else if(Vector2.Angle(gesture.Move,Vector2.up) > 180 - staticAngle ){
			lastDirection = MyDirection.DirectionDown;
			isMoved = true;
		}
		if(isMoved){
			_game.move(lastDirection);
		}



	}
}
