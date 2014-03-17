using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct Grid{
	public int size;
	public Tile[,] cells;
	public Grid(int size){
		this.size = size;
		this.cells = new Tile[size,size];

	}


	public List<Vector2> availableCells(){
		List<Vector2> cells =  new List<Vector2>();
		for(int i = 0; i < this.size;i++){
			for(int j = 0; j < this.size;j++){
				Tile t = this.cells[i,j];
				if(t.enable){
					cells.Add( new Vector2(t.x,t.y));
				}
			}
		}
		return cells;
	}

	public Vector2 randomAvailableCell(){
		List<Vector2> cells = this.availableCells();
		return cells[Random.Range(0,cells.Count)];
	}

	public bool cellsAvailable(){
		return this.availableCells().Count > 0 ;
	}


	public bool cellAvailable(Vector2 position){
		return !this.cellOccupied(position);
	}

	public bool cellOccupied(Vector2 postion){
		return this.cellContent(postion) != null;
	}

	public Tile? cellContent(Vector2 postion){
		if(this.withinBounds(postion)){
			return this.cells[(int)postion.x,(int)postion.y];
		}
		return null;
	}


	public void insertTile(Tile tile){
		this.cells[tile.x,tile.y] = tile;
	}

	public void removeTile(Tile tile){
		this.cells[tile.x,tile.y].enable = false;
	}

	public bool withinBounds(Vector2 position){
		return position.x >= 0 && position.x < this.size && position.y >= 0 && position.y < this.size;
	}

}

