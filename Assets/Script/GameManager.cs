using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {

	private int currentScore = 0;
	private int hightestScore = 0;

	private Tile tile;

	private Game _game;

	public GameObject scoreText;
	public GameObject hScoreText;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Awake(){
		StartGame();
	}

	void StartGame(){
		currentScore = 0;
		int tempHightestScore = PlayerPrefs.GetInt("HightestScore");
		hightestScore = tempHightestScore;
		hScoreText.GetComponent<tk2dTextMesh>().text = tempHightestScore.ToString();
		scoreText.GetComponent<tk2dTextMesh>().text = "0";
		_game = gameObject.GetComponent<Game>();
		_game.setup(4);

	}

	void option(Game game){
		GameObject[] gameobjects =  GameObject.FindGameObjectsWithTag("TileText");
		string stri = "";
		for(int i =0;i<_game.grid.size;i++){
			string s = "";
			for(int j =0;j<_game.grid.size;j++){
				Tile t = _game.grid.cells[j,i];
				if(t != null){
					Tile t1 = (Tile)t;
					s = s + t1.ToString();
				}else{
					s = s +"     0         ";
				}
				string name = "TextMesh"+j+i;
				string str;
				foreach(GameObject go in gameobjects){
					if(go.name == name){
						if(t != null){
							Tile tile = (Tile)t;
							str = ""+tile.value;
						}else{
							str = "";
						}	
						go.GetComponent<tk2dTextMesh>().text = str;
					}

				}
			}
			stri = stri + s + "\n";
		}
		Debug.Log(stri);

		currentScore = game.score;
		if (currentScore > hightestScore) {
			PlayerPrefs.SetInt("HightestScore",currentScore);
			hightestScore = currentScore;
			hScoreText.GetComponent<tk2dTextMesh>().text = hightestScore.ToString();
		}
		scoreText.GetComponent<tk2dTextMesh>().text = currentScore.ToString();
	}
	void OnSwipe( SwipeGesture gesture ) 
	{
		// Total swipe vector (from start to end position)
//		Vector2 move = gesture.Move;
		
		// Instant gesture velocity in screen units per second
//		float velocity = gesture.Velocity;
		
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		Debug.Log("OnSwipe  :"+direction);

//		hightestScore ++;
//		PlayerPrefs.SetInt("HightestScore",hightestScore);
//		int i = Random.Range(0,2);
//		Debug.Log(i);
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
