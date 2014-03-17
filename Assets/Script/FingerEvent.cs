using UnityEngine;
using System.Collections;

public class FingerEvent : MonoBehaviour {


	void Awake(){
		Debug.Log("game  start");
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
		
		if (direction == FingerGestures.SwipeDirection.Right)
			Application.LoadLevel(0);

	}

//	public enum SwipeDirection
//	{
//		/// <summary>
//		/// Moved to the right
//		/// </summary>
//		Right = 1 << 0,
//		
//		/// <summary>
//		/// Moved to the left
//		/// </summary>
//		Left = 1 << 1,
//		
//		/// <summary>
//		/// Moved up
//		/// </summary>
//		Up = 1 << 2,
//		
//		/// <summary>
//		/// Moved down
//		/// </summary>
//		Down = 1 << 3,
//		
//		/// <summary>
//		/// North-East diagonal
//		/// </summary>
//		UpperLeftDiagonal = 1 << 4,
//		
//		/// <summary>
//		/// North-West diagonal
//		/// </summary>
//		UpperRightDiagonal = 1 << 5,
//		
//		/// <summary>
//		/// South-East diagonal
//		/// </summary>
//		LowerRightDiagonal = 1 << 6,
//		
//		/// <summary>
//		/// South-West diagonal
//		/// </summary>
//		LowerLeftDiagonal = 1 << 7,

}
