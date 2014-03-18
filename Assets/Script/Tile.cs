using UnityEngine;
using System.Collections;

public class Tile{

	public int  x;
	public int  y;

	public int  value;
	public Hashtable previousPosition;
	public Tile[] mergedFrom;

	

	public Tile(GamePostion postion,int value){
		this.x = postion.x;
		this.y = postion.y;
		this.value = value;
		this.previousPosition = new Hashtable();
		
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

	public void updatePostion(GamePostion position){
		Debug.Log("updatePostion:" + position.ToString());
		this.x = position.x;
		this.y = position.y;
		Debug.Log("x:"+this.x);
		Debug.Log("y:"+this.y);
	}



	public string ToString(){
		return "x :"+this.x + "  y:"+this.y + " value:"+ this.value;

	}
}
