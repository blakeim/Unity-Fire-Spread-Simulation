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

	GameObject plant_body;
	PlantStatus plant_status;
	float burn_duration;
	float wind_velocity;
	float fire_intensity;
	Vector3 wind_direction;

	// Use this for initialization
	void Start () {
 		
		GeneratePlant();
		plant_status = PlantStatus.Healthy;
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
		SetPlantStatus(plant_status);
		wind_direction = Vector3.forward;
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
		Vector3 plant_position;

		gameObject.SetActive(true);

		//This will happen only if the plant already exists, and is being reset, which will be most of the time
		if(!reset){
			plant_body = GameObject.CreatePrimitive(PrimitiveType.Cube);
			plant_body.transform.localScale = new Vector3(2f, 4f, 2f);
			plant_body.layer = 9;
			plant_body.transform.parent = gameObject.transform;
		}
		
		//I'm honestly just doing this to save some typing, since I use this value twice
		plant_body.transform.position = plant_position = transform.position;
		
		//check for terrain or anything below the plant and drop it to that height, and set the x and y to random values in the play space
		if(Physics.Raycast(plant_position, Vector3.down, out hit)){
				plant_body.transform.position = new Vector3(UnityEngine.Random.Range(0,100), 
							plant_position.y - hit.distance, UnityEngine.Random.Range(0,100));
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
			if(plant_status == PlantStatus.OnFire){
				StartCoroutine("BurnCountdown");
				StartCoroutine("SpreadFire");
			}
		}
		else{
			StopAllCoroutines();
		}
	}

	void SetPlantStatus(PlantStatus status){

		Renderer renderer = plant_body.GetComponent<Renderer>();

		switch(status){
			case PlantStatus.Healthy:
				renderer.material.color = new Color(0.13f, 0.54f, 0.13f);
				StopCoroutine("BurnCountdown");
				StopCoroutine("SpreadFire");
				//I was going to stop all coroutines, but I think in the future a "grow" coroutine might be cool, so I'll stop them by name now
				break;
			case PlantStatus.OnFire:
				if(plant_status == PlantStatus.Healthy){
					renderer.material.color = new Color(0.50f, 0.0f, 0.0f);
					plant_status = PlantStatus.OnFire;
					StartCoroutine("BurnCountdown");
					StartCoroutine("SpreadFire");
				}
				break;
			case PlantStatus.Burnt:
				renderer.material.color = new Color(0.0f, 0.0f, 0.0f);
				StopCoroutine("SpreadFire");
				break;
			default:
				break;
		}
	}

    /*public void SpreadFire(Vector3 wind_direction, float wind_velocity){

		RaycastHit hit;
		Plant hit_plant;

		if(Physics.Raycast(plant_body.transform.position, wind_direction, out hit, wind_velocity, 1 << 9)){
			if((hit_plant = hit.collider.gameObject.GetComponentInParent<Plant>()) != null){
				if(hit_plant.GetPlantStatus() == PlantStatus.Healthy){
					
					/*I added this little chance to the spreading of fire, because it was spreading too
					reliably, so I couldn't tell if it was actually working. Basically as soon as a plant was
					lit, all plants in a line from it were also lit, and I couldn't tell if it was random chance 
					or if the spread was working. I mean I could tell because the console logged it, but a player
					wouldn't be able to tell, so I added this chance that will be tunable, and basically give an 
					artificial lag to the spread of fires 
					if(Random.Range(0.0f, 1.0f) > 0.5f){
						hit_plant.LightPlant();
						print("Fire spread");
					}
				}
			}
		}
	}*/

    IEnumerator BurnCountdown()
 	{
        yield return new WaitForSeconds(UnityEngine.Random.Range(3, burn_duration));
		plant_status = PlantStatus.Burnt;
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

			if(Physics.Raycast(plant_body.transform.position, wind_direction, out hit, 3 * wind_velocity, 1 << 9)){
				if((hit_plant = hit.collider.gameObject.GetComponentInParent<Plant>()) != null){
					if(hit_plant.GetPlantStatus() == PlantStatus.Healthy){
						hit_plant.LightPlant();
					}
				}
			}
		}
	}

	public PlantStatus GetPlantStatus(){
		return plant_status;
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
		this.wind_direction = Quaternion.Euler(0, angle, 0) * wind_direction;
	}

	public void SetBurnTime(float max_burn_time){
		this.burn_duration = max_burn_time;
	}

	public void SetFireIntensity(float max_burn_time){
		this.burn_duration = max_burn_time;
	}

	public void SetWindSpeed(float wind_velocity){
		this.wind_velocity = wind_velocity;
	}
}
