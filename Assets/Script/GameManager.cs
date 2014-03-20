using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {

	private int currentScore = 0;
	private int hightestScore = 0;

	public GameObject tile;

	private Game _game;

	public GameObject scoreText;
	public GameObject hScoreText;

	public GameObject bgObject;

	Vector3 firstPostion;
	float tileBetween = 1.44f;

	public Direction lastDirection;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void Awake(){
		float x = bgObject.transform.position.x-2.16f;
		
		float y = bgObject.transform.position.y+2.16f;
		
		firstPostion = new Vector3(x,y,0f);
		StartGame();
	}

	void StartGame(){
		currentScore = 0;
		GameObject[] allTile = GameObject.FindGameObjectsWithTag("Tile");
		for(int i = 0;i<allTile.Length;i++){
			Destroy(allTile[i]);
		}
		int tempHightestScore = PlayerPrefs.GetInt("HightestScore");
		hightestScore = tempHightestScore;
		hScoreText.GetComponent<tk2dTextMesh>().text = tempHightestScore.ToString();
		scoreText.GetComponent<tk2dTextMesh>().text = "0";
		_game = gameObject.GetComponent<Game>();
		_game.setup(4);

	}

	public Vector3 getTilePosition(int x,int y,int z=0){
		Vector3 result = new Vector3(firstPostion.x+x*tileBetween,firstPostion.y- y*tileBetween,z);
		
		return result;
	}


	void option(Game game){
		GameObject[] gameobjects =  GameObject.FindGameObjectsWithTag("TileText");
		string row ="";
		for(int i =0;i<_game.grid.size;i++){

			for(int j =0;j<_game.grid.size;j++){
				Tile t = _game.grid.cells[j,i];
				if(t != null){
					row += " "+t.value+" ";
				}else{
					row += "  0  ";
				}
			}
			row +="\n";
		}
		Debug.Log(row);

		GamePostion vector = _game.getVector(lastDirection);
		Hashtable traversals = _game.buildTraversals(vector);
		List<int> xList = (List<int>)traversals["x"];
		List<int> yList = (List<int>)traversals["y"];
		for(int ii=0;ii<4;ii++){
			int i = xList[ii];
			for(int jj=0;jj<4;jj++){
				int j = yList[jj];

//		for(int i =0;i<_game.grid.size;i++){
//			for(int j =0;j<_game.grid.size;j++){
				Tile t = _game.grid.cells[i,j];
				if(t != null){
					GameObject newTile;
					if(t.mergedFrom != null){
						Debug.Log(t.mergedFrom[0].ToString());
						Debug.Log(t.mergedFrom[1].ToString());
						int x1 = (int)t.mergedFrom[0].previousPosition["x"];
						int y1 = (int)t.mergedFrom[0].previousPosition["y"];
						GameObject perTile1 = GameObject.Find("Tile"+x1+y1);
						perTile1.GetComponent<TIleScript>().move(getTilePosition(i,j,1),t.mergedFrom[0].value);
						perTile1.name = "Tile";
						Destroy(perTile1,0.3f);
						int x2 = (int)t.mergedFrom[1].previousPosition["x"];
						int y2 = (int)t.mergedFrom[1].previousPosition["y"];
						GameObject perTile2 = GameObject.Find("Tile"+x2+y2);
						perTile2.GetComponent<TIleScript>().move(getTilePosition(i,j,0),t.value);
						perTile2.name = "Tile"+i+j;;


						Debug.Log("merge  form destory x:"+x1+" y:"+y1 +" value:"+t.mergedFrom[0].value);
						Debug.Log("merge  form change x:"+x2+" y:"+y2 +" yuan value:"+t.mergedFrom[1].value +"  xian:"+t.value);

					}else{
						if(t.previousPosition != null){
							int x = (int)t.previousPosition["x"];
							int y = (int)t.previousPosition["y"];
							newTile = GameObject.Find("Tile"+x+y);
							newTile.GetComponent<TIleScript>().move(getTilePosition(i,j),t.value);
							newTile.name = "Tile"+i+j;
							Debug.Log("move  form change yuan x:"+x+" y:"+y +"  value:"+t.value +"  xian:"+"Tile"+i+j);
						}else{
							{
								newTile = Instantiate(tile,getTilePosition(i,j),Quaternion.identity) as GameObject;
								newTile.GetComponent<TIleScript>().setCurrentValue(t.value);
								newTile.name = "Tile"+i+j;
								Debug.Log("new   x:"+i+" y:"+j +" value:"+t.value);
							}
						}
					}
				}

			}
		}

//		for(int i =0;i<_game.grid.size;i++){
//			for(int j =0;j<_game.grid.size;j++){
//				Tile t = _game.grid.cells[i,j];
//				if(t != null){
//					GameObject newTile;
//					if(t.mergedFrom == null){
//						if(t.previousPosition == null){
//							newTile = Instantiate(tile,getTilePosition(i,j),Quaternion.identity) as GameObject;
//							newTile.GetComponent<TIleScript>().setCurrentValue(t.value);
//							newTile.name = "Tile"+i+j;
//							Debug.Log("new   x:"+i+" y:"+j +" value:"+t.value);
//						}
//					}
//				}
//				
//			}
//			
//		}





		currentScore = game.score;
		if (currentScore > hightestScore) {
			PlayerPrefs.SetInt("HightestScore",currentScore);
			hightestScore = currentScore;
			hScoreText.GetComponent<tk2dTextMesh>().text = hightestScore.ToString();
		}
		scoreText.GetComponent<tk2dTextMesh>().text = currentScore.ToString();
	}

	
	void OnSwipe( SwipeGesture gesture ) 
	{
		
		// Approximate swipe direction
		FingerGestures.SwipeDirection direction = gesture.Direction;
		Debug.Log("OnSwipe  :"+direction);


		switch(direction){
			case FingerGestures.SwipeDirection.Right:
			lastDirection = Direction.DirectionRight;
				_game.move(Direction.DirectionRight);
			break;
		case FingerGestures.SwipeDirection.Up:
			lastDirection = Direction.DirectionUp;
			_game.move(Direction.DirectionUp);
			break;
		case FingerGestures.SwipeDirection.Left:
			lastDirection = Direction.DirectionLeft;
			_game.move(Direction.DirectionLeft);
			break;
		case FingerGestures.SwipeDirection.Down:
			lastDirection = Direction.DirectionDown;
			_game.move(Direction.DirectionDown);
			break;
		default :
			break;
		}

	}
}
