using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
An event handler for general game level mouse events.

This will handle interaction like adding plants via mouse
click as well as toggling the fire or removing them. I'm 
trying to use this new-fangled event system in Unity 5 to do
this, so its possible I'm not doing it exactly right, but I
thought this was a good oppurtunity to try and learn how this
system worked
**************************************/
public class MouseInteractionManager : MonoBehaviour {

	// Use this for initialization
	void Start () {		

	}
	
	// Update is called once per frame
	void Update () {
	}

	public void ClickEvent(){

		/*I'm doing this here instead of it being a global, because this could be updated, but I don't want to fetch
		it everytime its updated or constantly poll it, so I'll just poll it when I want to use it, save some cycles*/
		InteractionMode mode = gameObject.GetComponentInChildren<FireSpreadGameManager>().GetMode();
		
		if(Input.GetMouseButtonDown(0)){
			 
			 Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 			 RaycastHit hit;
 
 			if(Physics.Raycast (ray, out hit))
			{
				ProcessClickEvent(hit, mode);
			}
		}
	}

	void ProcessClickEvent(RaycastHit hit, InteractionMode mode){

		Plant hit_plant;

		if((hit_plant = hit.collider.gameObject.GetComponentInParent<Plant>()) != null){
			switch(mode){
				case InteractionMode.Remove:
					hit_plant.DestroyPlant();
					break;
				case InteractionMode.Toggle:
					if(hit_plant.GetPlantStatus() == PlantStatus.Healthy){
						hit_plant.LightPlant();
					}
					else{
						hit_plant.ExtinguishPlant();
					}
					break;
				//I want to detect if whats hit is terrain here, because I don't want players generating plants on top of space or widgets	
				case InteractionMode.Add:
					print(hit.collider.GetType());
					break;
				default:
					break;
			}
			if(hit_plant.GetPlantStatus() == PlantStatus.Healthy){
						
			}
		}
		else if((hit.collider.gameObject.GetComponentInParent<Terrain>()) != null){

		}

	}
}
