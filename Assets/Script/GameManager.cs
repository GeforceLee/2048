using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private int currentScore = 0;
	private int hightestScore = 0;

	private Tile tile;

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
		Debug.Log(tempHightestScore);
//		hightestScore = PlayerPrefs.GetInt("HightestScore");
	}


	void OnSwipe( SwipeGesture gesture ) 
	{
		// Total swipe vector (from start to end position)
		Vector2 move = gesture.Move;
		
		// Instant gesture velocity in screen units per second
		float velocity = gesture.Velocity;
		
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		Debug.Log(""+direction);
		
		if (direction == FingerGestures.SwipeDirection.Right){
			hightestScore ++;
			PlayerPrefs.SetInt("HightestScore",hightestScore);
			int i = Random.Range(0,2);
			Debug.Log(i);
		}

		
	}
}
