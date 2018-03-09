using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/**************************************
An small event handler to update the play mode of the simulation, 
as well as toggling the button display so that a 
user can tell whether the simulation should be playing or not. 
**************************************/
public class PlayPauseButtonUpdate : MonoBehaviour {

	Text text;
	int play_pause_num = 0;
	// Use this for initialization
	void Start () {
		text = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateButtonText(){
		
		play_pause_num = (play_pause_num + 1) % 2;

		//a little more cumbersome in code, but slightly faster than an if/else
		switch(play_pause_num){
			case 0:
				text.text = "Play";//little confusing in code, but the button should say the opposite of the current status
				break;
			case 1:
				text.text = "Play";
				break;
			default:
				break;
		}
	}
}
