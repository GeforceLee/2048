using UnityEngine;
using System.Collections;

public struct GamePostion{
	public int x;
	public int y;

	public GamePostion(int x,int y):this(){
		this.x = x;
		this.y = y;
	}

	public string ToString(){
		return "postion x:" + this.x + " y:"+this.y;
	}
}
