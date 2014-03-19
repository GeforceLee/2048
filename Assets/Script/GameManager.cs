﻿using UnityEngine;
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

	Vector3 firstPostion;
	float tileBetween = 1.44f;
	// Use this for initialization
	void Start () {

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

	void StartGame(){
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

	}

	public Vector3 getTilePosition(int x,int y,int z=0){
		Vector3 result = new Vector3(firstPostion.x+x*tileBetween,firstPostion.y- y*tileBetween,z);
		
		return result;
	}


	void option(Game game){
		GameObject[] gameobjects =  GameObject.FindGameObjectsWithTag("TileText");

		for(int i =0;i<_game.grid.size;i++){
			for(int j =0;j<_game.grid.size;j++){
				Tile t = _game.grid.cells[i,j];
				if(t != null){
					GameObject newTile;
					if(t.mergedFrom == null){
						if(t.previousPosition != null){
							int x = (int)t.previousPosition["x"];
							int y = (int)t.previousPosition["y"];
							newTile = GameObject.Find("Tile"+x+y);
							newTile.GetComponent<TIleScript>().move(getTilePosition(i,j));
							newTile.GetComponent<TIleScript>().setCurrentValue(t.value);
							newTile.name = "Tile"+i+j;
						}else{
							addTile(i,j,t.value);
//							 newTile = Instantiate(tile,getTilePosition(i,j),Quaternion.identity) as GameObject;
						}
					}else{
						Debug.Log(t.mergedFrom[0].ToString());
						Debug.Log(t.mergedFrom[1].ToString());
						int x1 = (int)t.mergedFrom[0].previousPosition["x"];
						int y1 = (int)t.mergedFrom[0].previousPosition["y"];
						GameObject perTile1 = GameObject.Find("Tile"+x1+y1);
						perTile1.GetComponent<TIleScript>().move(getTilePosition(i,j,1));
						Destroy(perTile1,0.39f);

						int x2 = (int)t.mergedFrom[1].previousPosition["x"];
						int y2 = (int)t.mergedFrom[1].previousPosition["y"];
						GameObject perTile2 = GameObject.Find("Tile"+x2+y2);
						perTile2.GetComponent<TIleScript>().move(getTilePosition(i,j,1));
						Destroy(perTile2,0.39f);

						newTile = Instantiate(tile,getTilePosition(i,j),Quaternion.identity) as GameObject;
						newTile.GetComponent<TIleScript>().setCurrentValue(t.value);
						newTile.name = "Tile"+i+j;
					}

				}

			}

		}


		currentScore = game.score;
		if (currentScore > hightestScore) {
			PlayerPrefs.SetInt("HightestScore",currentScore);
			hightestScore = currentScore;
			hScoreText.GetComponent<tk2dTextMesh>().text = hightestScore.ToString();
		}
		scoreText.GetComponent<tk2dTextMesh>().text = currentScore.ToString();
	}


	IEnumerable addTile(int x,int y ,int value){

		GameObject newTile = Instantiate(tile,getTilePosition(x,y),Quaternion.identity) as GameObject;
		newTile.GetComponent<TIleScript>().setCurrentValue(value);
		newTile.name = "Tile"+x+y;
		yield return new WaitForSeconds(0.1f);
	}
	void OnSwipe( SwipeGesture gesture ) 
	{
		
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		Debug.Log("OnSwipe  :"+direction);


		switch(direction){
			case FingerGestures.SwipeDirection.Right:
				_game.move(Direction.DirectionRight);
			break;
		case FingerGestures.SwipeDirection.Up:
			_game.move(Direction.DirectionUp);
			break;
		case FingerGestures.SwipeDirection.Left:
			_game.move(Direction.DirectionLeft);
			break;
		case FingerGestures.SwipeDirection.Down:
			_game.move(Direction.DirectionDown);
			break;
		default :
			break;
		}

	}
}
