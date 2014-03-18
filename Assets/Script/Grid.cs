using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Grid{
	public int size;
	public Tile[,] cells;
	public Grid(int size){
		this.size = size;
		this.cells = new Tile[size,size];

	}


	public List<GamePostion> availableCells(){
		List<GamePostion> cells =  new List<GamePostion>();
		for(int i = 0; i < this.size;i++){
			for(int j = 0; j < this.size;j++){
				Tile t = this.cells[i,j];
				if(t == null){
					GamePostion gp = new GamePostion(i,j);
					cells.Add( gp);
				}
			}
		}

		return cells;
	}

	public GamePostion randomAvailableCell(){
		List<GamePostion> cells = this.availableCells();
		return cells[Random.Range(0,cells.Count)];
	}

	public bool cellsAvailable(){
		//Debug.Log("Grid cellsAvailable  his.availableCells().Count:"+this.availableCells().Count);
		return this.availableCells().Count > 0 ;
	}


	public bool cellAvailable(GamePostion position){
		return !this.cellOccupied(position);
	}

	public bool cellOccupied(GamePostion postion){
		//Debug.Log("cellOccupied x:"+postion.x + "   y:"+postion.y+"   bool:"+(this.cellContent(postion) != null));
		return this.cellContent(postion) != null;
	}

	public Tile cellContent(GamePostion postion){
		if(this.withinBounds(postion)){
			return this.cells[postion.x,postion.y];
		}
		return null;
	}


	public void insertTile(Tile tile){
		Debug.Log("insertTile x:"+tile.x + "  y:"+tile.y);
		this.cells[tile.x,tile.y] = tile;
	}

	public void removeTile(Tile tile){
		Debug.Log("removeTile x:"+tile.x + "  y:"+tile.y);
		this.cells[tile.x,tile.y] = null;
	}

	public bool withinBounds(GamePostion position){
//		Debug.Log("withinBounds  x:"+position.x + "   y:"+position.y+"    "+(position.x >= 0 && position.x < this.size && position.y >= 0 && position.y < this.size));
		return position.x >= 0 && position.x < this.size && position.y >= 0 && position.y < this.size;
	}

}

