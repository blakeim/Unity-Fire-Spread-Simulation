using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

/**************************************
A GUI element to show the wind direction

it should rotate to face the mouse where its clicking on 
the screen. I've done this with a top down view using a 
LERP before, which didn't really work here because of the 
angle. Breaking out the tangent angle of the difference in
the two vectors should work, it might also give me half of 
the actual angle, but as long as I've positioned the arrow
correctly I should be able to tell pretty quickly if its
giving me the correct angle or the half. 
**************************************/
public class WindRotationScript : MonoBehaviour {

	[SerializeField]
	GameObject arrowCube;
	[SerializeField]
	PlantFactory plantFactory;
	[SerializeField]
	float rotationSpeed;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void RotateArrow(){

		/*I want the position of the mouse on the screen, not in world space. I'm not actually sure about this,
		I've done this with a top down view before but never with this sort of hybrid front on view, but it seems right*/
		Vector3 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		Vector3 screenPosition = Camera.main.WorldToViewportPoint (transform.position);
		float angle = Mathf.Atan2(screenPosition.x - mousePosition.x, screenPosition.y - mousePosition.y) * Mathf.Rad2Deg;

		//Euler angle rotation only on y axis. If I've alligned the arrow correctly, this rotation should be accurate
		arrowCube.transform.rotation = Quaternion.Euler(0,angle,0);
		plantFactory.UpdateWindDirection(angle);
	}

	void OnMouseDrag(){
		RotateArrow();
	}
}
