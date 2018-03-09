using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
An small event handler to update the interaction mode for
the game, as well as toggling the button display so that a 
user can tell what mode the game is in. 
**************************************/
public class ModeLabelUpdater : MonoBehaviour {

	InteractionMode[] possible_modes = (InteractionMode[])System.Enum.GetValues(typeof(InteractionMode));
	InteractionMode current_mode;
	int mode_counter = 0;

	// Use this for initialization
	void Start () {
		/*I could have done this in declaration, using explicit values, but here if I want to change the 
		default I just change the one value for mode counter*/
		current_mode = possible_modes[mode_counter];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public InteractionMode UpdateLabel(){

		//this is nice because it works even if more game modes are added, they just have to be added to the enum
		mode_counter = (mode_counter + 1) % possible_modes.Length;
		current_mode = possible_modes[mode_counter];

		return current_mode;
	}
}
