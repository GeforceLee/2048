using UnityEngine;
using System.Collections;

public struct Tile{

	public int  x { get;  set; }
	public int  y { get;  set; }

	public int  value { get;  set; }
	public Hashtable previousPosition {get;  set;}
	public Tile?[] mergedFrom {get;  set;}

	public bool enable {get;set;}

	public Tile(Vector2 postion,int value = 2):this(){
		this.x = (int)postion.x;
		this.y = (int)postion.y;
		this.value = value;
		this.previousPosition = new Hashtable();
		this.enable = true;
	}


	public void savePostion(){
		if(this.previousPosition.Contains("x")){
			this.previousPosition.Remove("x");
		}
		this.previousPosition.Add("x",this.x);
		if(this.previousPosition.Contains("y")){
			this.previousPosition.Remove("y");
		}
		this.previousPosition.Add("y",this.y);
	}

	public void updatePostion(Vector2 position){
		this.x = (int)position.x;
		this.y = (int)position.y;
	}




}
