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

		
	}


	public void savePostion(){
		this.previousPosition = new Hashtable();
		this.previousPosition.Add("x",this.x);
		this.previousPosition.Add("y",this.y);
	}

	public void updatePostion(GamePostion position){
//		Debug.Log("updatePostion:" + position.ToString() + "  yuan x:"+this.x+"    y:"+this.y);
		this.x = position.x;
		this.y = position.y;
	}



	public string ToString(){
		return "x :"+this.x + "  y:"+this.y + " value:"+ this.value;

	}
}
