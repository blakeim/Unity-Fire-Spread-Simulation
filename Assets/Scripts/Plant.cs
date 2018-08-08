using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
A class representing the base functionality of a plant.

Again here there are certain things delegated to the plant
over the factory, in particular the spreading of fire. Because
I based it off of a coroutine, it made more sense to 
**************************************/
public class Plant : MonoBehaviour {

	GameObject plantBody;
	PlantStatus plantStatus;
	float burnDuration;
	float windVelocity;
	float fireIntensity;
	Vector3 windDirection;

	// Use this for initialization
	void Start () {
 		
		GeneratePlant();
		plantStatus = PlantStatus.Healthy;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/* In reality, this method will only be used once, so maybe it doesn't need to exist
	I'm going to leave it, in because, despite the small amount of code repition, it allows 
	me to avoid a few branches, as well as making whats happening clear */
	void GeneratePlant(){

		/*I'm only using this for the innitial generation of the 
		plants, so I'll set them to false so they don't show up
		before the button is pressed */

		BuildPlantBody(false);
		SetPlantStatus(plantStatus);
		windDirection = Vector3.forward;
		gameObject.SetActive(false);
	}

	/* This will reset the existing plants, shuffle their order and end any active coroutines, 
	as well as resetting the status to make sure a plant doesn't continue burning or spreading
	fire after it is reset and moved */
	public void ResetPlant(){
		
		/*Theres some repeated code, a little bit, but I'm okay with it here, only because
		the two methods serve different functions, and the names are good for their functions
		I don't want to overly fill public methods with parameters and make the name confusing*/

		BuildPlantBody(true);
		SetPlantStatus(PlantStatus.Healthy);
	}	

	/* This method is actually doing the logic to build a plant. Importantly, it looks for the terrain, 
	or indeed anything underneet where the plant is meant to go, and drops the plant to that height (its
	that height at origin, so some of the plant will push through the terrain, but the player won't notice
	that)

	In the future, I'd like to set this up with a resource folder to, instead of generating a cube, generate
	a random shape from a given collection of possible plant shapes */
	public void BuildPlantBody(bool reset){

		RaycastHit hit;
		Vector3 plantPosition;

		gameObject.SetActive(true);

		//This will happen only if the plant already exists, and is being reset, which will be most of the time
		if(!reset){
			plantBody = GameObject.CreatePrimitive(PrimitiveType.Cube);
			plantBody.transform.localScale = new Vector3(2f, 4f, 2f);
			plantBody.layer = 9;
			plantBody.transform.parent = gameObject.transform;
		}
		
		//I'm honestly just doing this to save some typing, since I use this value twice
		plantBody.transform.position = plantPosition = transform.position;
		
		//check for terrain or anything below the plant and drop it to that height, and set the x and y to random values in the play space
		if(Physics.Raycast(plantPosition, Vector3.down, out hit)){
				plantBody.transform.position = new Vector3(UnityEngine.Random.Range(0,100), 
							plantPosition.y - hit.distance, UnityEngine.Random.Range(0,100));
		}
	}

    internal void ExtinguishPlant()
    {
		SetPlantStatus(PlantStatus.Healthy);
    }

    public void LightPlant(){
		SetPlantStatus(PlantStatus.OnFire);
	}

	public void PausePlantSimulation(bool simulation){
		
		if(simulation){
			if(plantStatus == PlantStatus.OnFire){
				StartCoroutine("BurnCountdown");
				StartCoroutine("SpreadFire");
			}
		}
		else{
			StopAllCoroutines();
		}
	}

	void SetPlantStatus(PlantStatus status){

		Renderer renderer = plantBody.GetComponent<Renderer>();

		switch(status){
			case PlantStatus.Healthy:
				renderer.material.color = new Color(0.13f, 0.54f, 0.13f);
				StopCoroutine("BurnCountdown");
				StopCoroutine("SpreadFire");
				//I was going to stop all coroutines, but I think in the future a "grow" coroutine might be cool, so I'll stop them by name now
				break;
			case PlantStatus.OnFire:
				if(plantStatus == PlantStatus.Healthy){
					renderer.material.color = new Color(0.50f, 0.0f, 0.0f);
					plantStatus = PlantStatus.OnFire;
					StartCoroutine("BurnCountdown");
					StartCoroutine("SpreadFire");
				}
				break;
			case PlantStatus.Burnt:
				renderer.material.color = new Color(0.0f, 0.0f, 0.0f);
				StopCoroutine("BurnCountdown");
				StopCoroutine("SpreadFire");
				break;
			default:
				break;
		}
	}

    IEnumerator BurnCountdown()
 	{
        yield return new WaitForSeconds(UnityEngine.Random.Range(3, burnDuration));
		plantStatus = PlantStatus.Burnt;
		SetPlantStatus(PlantStatus.Burnt);
 	}

	/* I set this up as a coroutine instead of just doing it, because it was hard to tell if plants were lighting
	correctly. They were lighting instantly, and the only way I could tell it was because of windspread was from 
	console logging. So I changed it up so that fire spreads every 5 seconds, on a plant by plant basis. Originally
	I had this coroutine in the PlantFactory, which worked, but it meant that a) the game was performing a lot of
	unneeded raycasts (because it was checking for every plant not just the burning ones) and b) I could be in a 
	situation where if I lit a plant right before the timer ticked it would then instantly spread and it looked weird */
	IEnumerator SpreadFire()
 	{
		while(true){
			yield return new WaitForSeconds(5f);
			RaycastHit hit;
			Plant hit_plant;

			if(Physics.Raycast(plantBody.transform.position, windDirection, out hit, 3 * windVelocity, 1 << 9)){
				if((hit_plant = hit.collider.gameObject.GetComponentInParent<Plant>()) != null){
					if(hit_plant.GetPlantStatus() == PlantStatus.Healthy){
						hit_plant.LightPlant();
					}
				}
			}
		}
	}

	public PlantStatus GetPlantStatus(){
		return plantStatus;
	}

	public void DestroyPlant(){

		/*I opted to disable the objects instead of destroying them
		outright, that way I can just switch them off and on and reset
		their status and position. Saves a few cycles, because now the
		app doesn't have to regenerate the innitial plants everytime they
		are cleared */

		//plant_body.gameObject.SetActive(false);
		gameObject.SetActive(false);
	}

	public void SetWindDirection(float angle){
		this.windDirection = Quaternion.Euler(0, angle, 0) * windDirection;
	}

	public void SetBurnTime(float max_burn_time){
		this.burnDuration = max_burn_time;
	}

	public void SetFireIntensity(float max_burn_time){
		this.burnDuration = max_burn_time;
	}

	public void SetWindSpeed(float wind_velocity){
		this.windVelocity = wind_velocity;
	}
}
