using UnityEngine;
using System.Collections;



public class TIleScript : MonoBehaviour {

	Color color2 = new Color(0xee/255.0f,0xe4/255.0f,0xda/255.0f);
	Color color4 = new Color(0xed/255.0f,0xe0/255.0f,0xc8/255.0f);
	Color color8 = new Color(0xf2/255.0f,0xb1/255.0f,0x79/255.0f);
	Color color16 = new Color(0xf5/255.0f,0x95/255.0f,0x63/255.0f);
	Color color32 = new Color(0xf6/255.0f,0x7c/255.0f,0x5f/255.0f);
	Color color64 = new Color(0xf6/255.0f,0x5e/255.0f,0x3b/255.0f);
	Color color128 = new Color(0xed/255.0f,0xcf/255.0f,0x72/255.0f);
	Color color256 = new Color(0xed/255.0f,0xcc/255.0f,0x61/255.0f);
	Color color512 = new Color(0xed/255.0f,0xc8/255.0f,0x50/255.0f);
	Color color1024 = new Color(0xed/255.0f,0xc5/255.0f,0x3f/255.0f);
	Color color2048 = new Color(0xed/255.0f,0xc2/255.0f,0x2e/255.0f);
	Color colorOther = new Color(0xed/255.0f,0xc2/255.0f,0x1f/255.0f);

	bool moved = false;
	float movedTime = 0f;
	float duringTime = 0.2f;
	Vector3 targetPostion;

	public int currentValue=0;
	public GameObject text;
	public int targetValue;
	


	public void setCurrentValue(int value){
		Color t;
		Color textColor = new Color(1,1,1);
		switch(value){
		case 2:
			t = color2;
			textColor = new Color(0.39f,0.35f,0.31f);
			break;
		case 4:
			t = color4;
			textColor = new Color(0.39f,0.35f,0.31f);
			break;
		case 8:
			t = color8;
			break;
		case 16:
			t = color16;
			break;
		case 32:
			t = color32;
			break;
		case 64:
			t = color64;
			break;
		case 128:
			t = color128;
			break;
		case 256:
			t = color256;
			break;
		case 512:
			t = color512;
			break;
		case 1024:
			t = color1024;
			break;
		case 2048:
			t = color2048;
			break;
		default:
			t = colorOther;
			break;
		}
		gameObject.GetComponent<tk2dSprite>().color = t;
		string str = ""+value;
		currentValue = value;
		text.GetComponent<tk2dTextMesh>().text = str;
		text.GetComponent<tk2dTextMesh>().color = textColor;
		if(str.Length >3){
			text.GetComponent<tk2dTextMesh>().scale = new Vector3(0.55f,0.55f,0.0f);
		}else if(str.Length == 3){
			text.GetComponent<tk2dTextMesh>().scale = new Vector3(0.75f,0.75f,0.0f);
		}else {
			text.GetComponent<tk2dTextMesh>().scale = new Vector3(0.8f,0.8f,0.0f);
		}

		
	}

	public void playScaleAnim(){
		Animator anim = gameObject.GetComponent<Animator>();
		anim.SetTrigger("TileScale"); 
	}
	// Use this for initialization
	void Start () {
		Animator anim = gameObject.GetComponent<Animator>();
//		anim.SetTrigger("TileInit");
	}


	public void move(Vector3 target,int value){
		Debug.Log("move target value:"+ value  +"  currentValue:"+currentValue +" moved:"+moved);
		if(!moved){
			movedTime = 0;
			targetPostion = target;
			targetValue = value;
			moved = true;
		}
	}


	void Update(){
		if(moved){
			movedTime += Time.deltaTime;
			float lerp = 0;
			if(movedTime>= duringTime){
				Debug.Log("update move over");
				moved = false;
				lerp = 1.0f;
			}else{
				lerp = movedTime/duringTime;
			}
			transform.position = Vector3.Lerp(transform.position,targetPostion,lerp);

			if(targetValue > currentValue && movedTime>duringTime*0.6f){
				Debug.Log("targetValue:"+targetValue+ " currentValue:"+currentValue+" movedTime:"+movedTime+" duringTime:"+ duringTime*0.6f);
				setCurrentValue(targetValue);
				playScaleAnim();
			}
		}

	}
}
