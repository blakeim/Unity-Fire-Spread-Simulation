using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**************************************
A factory for creating and doing mass manipulation of a collection of plants

This class will generate the innitial collection of plants, and tell them 
when they should be switched on or off, as well as changing their statuses
and attributes. This class contains and manipulates the list of plants
**************************************/
public class PlantFactory : MonoBehaviour {

	[SerializeField]
	Plant base_plant;//this is used as a base prefab to innitialize the plants in the begining, to be manipulated later

	//dynamic variables for sliding
	public float wind_speed{get; set;}
	public float spread_frequency{get; set;}
	public float max_burn_time{get; set;}
	public float max_plant_count{get; set;}//maybe I'm just stupid, but only floats showed up as dynamics, so this is a float even though its a whole numebr

    ArrayList plant_list;

	bool play_simulation = true;
	// Use this for initialization
	void Start () {
		
		max_plant_count = 50;
		plant_list = new ArrayList();
		/*When the app is started, I'll front load the work by generating the plants now, and just
		switching them off so that they don't show up until the button is pressed to generate them
		users won't know the difference and it should help on performance.
		one disadvantage is it means the ammount of plants is static, but I can get around that
		by only activating a random sample each time the button is pressed, or making the max plant
		count tunable with a slider, similar to the wind velocity*/
		for(int i = 0; i < max_plant_count; i++){
			plant_list.Add(Instantiate(base_plant, new Vector3(10, 100, 10), Quaternion.identity));
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

    /*I realize the name of this method might make the fact that its resetting, not generating confusing,
	but its also tied to the generate button, and I think all in all this naming makes more sense. Whats 
	important here is that this method resets all the plants in the plant array, giving the appearance
	of generating new plants, but actually just shuffling them all around and resetting their burns*/
    public void GeneratePlants(){

		for(int i = 0; i < max_plant_count; i++){
			((Plant)plant_list[i]).ResetPlant();	
		}
	}

	public void ClearPlants(){

		foreach(Plant p in plant_list){
			p.DestroyPlant();
		}

		//Since im disabling, not deleting objects, don't reset the list
		//plant_list = new Plant[list_size];
	}

	/*Select a random subset of plants, at most a third of the plants, and light them.
	I could also make this configurable later, but honestly the UI started to get a little
	crowded*/
	public void LightPlants(){

		//I'm using the unityEngine random becaue its a little cleaner, not sure if it will drastically impact performance but I'll check
		for(int i = UnityEngine.Random.Range(0, ((int)max_plant_count / 3)); i > 0; i--){
			int random_num = UnityEngine.Random.Range(0, (int)max_plant_count - 1);
			if(((Plant)plant_list[random_num]).GetPlantStatus() == PlantStatus.Healthy){
				((Plant)plant_list[random_num]).LightPlant();
			}
			else{
				i++;
			}
		}
	}

	public void PlayPause(){

		play_simulation = !play_simulation;

		for(int i = 0; i < max_plant_count; i++){
			if(plant_list[i] != null){
				((Plant)plant_list[i]).PausePlantSimulation(play_simulation);
			}
		}
	}

	public void UpdateWindSpeed(float wind_speed){

		for(int i = 0; i < max_plant_count; i++){
			((Plant)plant_list[i]).SetWindSpeed(wind_speed);
		}
	}

	public void UpdateFireSpreadRate(float fire_spread_rate){
		
		for(int i = 0; i < max_plant_count; i++){
			((Plant)plant_list[i]).SetFireIntensity(fire_spread_rate);
		}

	}

	public void UpdateBurnTime(float burn_time){

		for(int i = 0; i < max_plant_count; i++){
			((Plant)plant_list[i]).SetBurnTime(burn_time);
		}
	}

	public void AddPlant(Vector3 point){

		max_plant_count++;
		plant_list.Add(Instantiate(base_plant, point, Quaternion.identity));
	}

	/*public void UpdatePlantCount(float plant_count){

	}*/

	public void UpdateWindDirection(float angle){

		for(int i = 0; i < max_plant_count; i++){
			((Plant)plant_list[i]).SetWindDirection(angle);
		}
	}
}
