using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum MyDirection
{
	DirectionUp,
	DirectionRight,
	DirectionDown,
	DirectionLeft
}


public class Game : MonoBehaviour {
	public int size;
	public int startTiles = 2;
	public Grid grid;
	public int score;
	public bool over;
	public bool won;
	


	public void setup(int s){
		size = s;
		grid = new Grid(s);
		score = 0;
		over = false;


		addStartTiles();
		actuate(true);
	}
	public void load(int s,string save,int sc){
		size = s;
		grid = new Grid(s);
		score = sc;
		over = false;
		addLoadTiles(save);
		actuate(true);
	}
	public void addLoadTiles(string st){
		string[] sArray = st.Split(',');
		for(int i = 0,length = sArray.Length;i<length;i++){
			int sc = int.Parse(sArray[i]);
			if(sc>0){
				Tile tile = new Tile(new GamePostion(i/size,i%size), sc);
				grid.insertTile(tile);
			}

		}
	}
	public void addStartTiles(){
		for (int i = 0; i < startTiles; i++) {
			addRandomTile();
		}
	}

	public void addRandomTile(){
		if (grid.cellsAvailable()) {
			int value = Random.Range(0,10) < 8 ? 2 : 4;
			//int value = Random.Range(0,10) < 8 ? 512 : 1024;

			Tile tile = new Tile(grid.randomAvailableCell(), value);
			
			grid.insertTile(tile);
		}
	}


	public void actuate(bool moved){
		gameObject.BroadcastMessage("option",moved);
	}


	public void prepareTiles(){
		for(int i=0;i<size;i++){
			for(int j=0;j<size;j++){
				Tile t = grid.cells[i,j];
				if(t != null){
					Tile tile = (Tile)t;
					tile.mergedFrom = null;
					tile.savePostion();
				}
			}
		}
		
	}

	public void moveTile(Tile tile,GamePostion cell){
//		Debug.Log("moveTile:" + cell.ToString());
		grid.cells[tile.x,tile.y] = null;
		Tile o = grid.cells[tile.x,tile.y];
//		Debug.Log("moveTile null :"+o == null);
		grid.cells[cell.x,cell.y] = tile;
//		Debug.Log("moveTile hou :"+grid.cells[cell.x,cell.y].ToString());
//		Debug.Log("before up:"+tile.ToString());
		tile.updatePostion(cell);
//		Debug.Log("after up:"+tile.ToString());
	}




	public void move(MyDirection direction){
//		Debug.Log("移动");
		GamePostion vector = getVector(direction);
		Hashtable traversals = buildTraversals(vector);
		prepareTiles();
		bool moved = false;
		Tile tile;
		List<int> xList = (List<int>)traversals["x"];
		List<int> yList = (List<int>)traversals["y"];
		for(int i=0;i<size;i++){
			int x = xList[i];
			for(int j=0;j<size;j++){
				int y = yList[j];
				GamePostion cell = new GamePostion(x,y);
				tile = grid.cellContent(cell);
	
				if(tile != null){
					Hashtable postions = findFarthestPosition(cell,vector);
					Tile next = grid.cellContent((GamePostion)postions["next"]);
					if(next !=null && next.value == tile.value && next.mergedFrom == null){

//						Debug.Log("找到合并的tile :"+tile.ToString());
//						Debug.Log("找到合并的next :"+next.ToString());
						Tile merged = new Tile((GamePostion)postions["next"],tile.value*2);
						merged.mergedFrom = new Tile[]{tile,next};

						grid.insertTile(merged);
						grid.removeTile(tile);

						tile.updatePostion((GamePostion)postions["next"]);

						score += merged.value;

						if(merged.value == 2048){
							//TODO  yingle
						}

					}else{
						moveTile(tile,(GamePostion)postions["farthest"]);
					}
					if(!positionsEqual(cell,new GamePostion(tile.x,tile.y))){
						moved = true;
					}

				}

			}
		}

		if(moved){
			addRandomTile();
			if(!movesAvailable()){
//				Debug.Log("GameOver");
				this.over = true;
			}
//			actuate();
//			Debug.Log("move le");
		}
		actuate(moved);
		Debug.Log("移动结束");
	}




	public GamePostion getVector(MyDirection direction){
		GamePostion result;
		switch(direction){
			case MyDirection.DirectionUp: 
				result = new GamePostion(0,-1);
				break;
			case MyDirection.DirectionRight:
				result = new GamePostion(1,0);
				break;
			case MyDirection.DirectionDown:
				result = new GamePostion(0,1);
				break;
			case MyDirection.DirectionLeft:
				result = new GamePostion(-1,0);
				break;
			default:
				result = new GamePostion(0,0);
				break;
		}
		return result;
	}

	public Hashtable buildTraversals(GamePostion vector){
		Hashtable reslut = new Hashtable();
		List<int> x = new List<int>();
		List<int> y = new List<int>();
		for(int i = 0;i < size; i++){
			x.Add(i);
			y.Add(i);
		}

		if(vector.x == 1)
			x.Reverse();
		if(vector.y == 1)
			y.Reverse();

		reslut.Add("x",x);
		reslut.Add("y",y);

		return reslut;
	}


	public Hashtable findFarthestPosition(GamePostion cell,GamePostion vector){
//		Debug.Log("findFarthestPosition cell  x:"+cell.x + "   y:"+cell.y);

		GamePostion previous;

		do{
			previous = cell;
			cell = new GamePostion(previous.x+ vector.x,previous.y+ vector.y);
		}while(grid.withinBounds(cell) && grid.cellAvailable(cell));

		Hashtable result = new Hashtable();
		result.Add("farthest",previous);
		result.Add("next",cell);
//		Debug.Log("之前的坐标farthest x:"+ previous.ToString());
//		Debug.Log("最近的坐标next x:"+ cell.ToString());
		return result;
	}


	public bool movesAvailable(){
		return grid.cellsAvailable() || tileMatchesAvailable();
	}

	public bool tileMatchesAvailable(){
		Tile tile;
		for(int x=0;x<size;x++){
			for(int y=0;y<size;y++){
				tile = grid.cellContent(new GamePostion(x,y));
				if(tile != null){
					for(int direction=0;direction<4;direction++){
						GamePostion vector = getVector((MyDirection)direction);
						GamePostion cell = new GamePostion(x+vector.x,y+vector.y);
						Tile other = grid.cellContent(cell);
						if(other !=null){
							Tile t = (Tile)other;
							Tile t1 =(Tile)tile;
							if(t.value == t1.value){
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}


	public bool positionsEqual(GamePostion first,GamePostion second){
		return first.x == second.x && first.y == second.y;
	}


	public string ToString(){
		Debug.Log("Game ToString");



		Debug.Log("Game ToString End");
		return "";
	}
}
