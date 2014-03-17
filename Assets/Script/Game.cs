using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	public int size;
	public int startTiles = 2;
	public Grid grid;
	public int score;
	public bool over;
	public bool won;
	

	public void setup(){
		grid = new Grid(this.size);
		this.score = 0;
		this.over = false;
		this.won = false;

		this.addStartTiles();
	}


	public void addStartTiles(){
		for (int i = 0; i < this.startTiles; i++) {
			this.addRandomTile();
		}
	}

	public void addRandomTile(){
		if (this.grid.cellsAvailable()) {
			var value = Random.Range(0,10) < 8 ? 2 : 4;
			var tile = new Tile(this.grid.randomAvailableCell(), value);
			
			this.grid.insertTile(tile);
		}
	}

}
