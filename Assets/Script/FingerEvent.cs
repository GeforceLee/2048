using UnityEngine;
using System.Collections;

public class FingerEvent : MonoBehaviour {

	void OnSwipe( SwipeGesture gesture ) 
	{
		// Total swipe vector (from start to end position)
		Vector2 move = gesture.Move;
		
		// Instant gesture velocity in screen units per second
		float velocity = gesture.Velocity;
		
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		Debug.Log ("asdfadf   " + direction);
	}
}
