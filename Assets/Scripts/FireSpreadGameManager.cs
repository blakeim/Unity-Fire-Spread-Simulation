using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

/**************************************
A general 'game manager', to control the interaction between 
the UI and the componenets of the simulation.

I didn't want to assign this functionality to PlantFactory, because 
I wanted that to deal purely with making and updating the plant list,
and it made more sense to pull this out in to its own oversoul type class
**************************************/

public class FireSpreadGameManager : MonoBehaviour {

	[SerializeField]
	Slider fire_spread_rate_slider;
	[SerializeField]
	Slider wind_speed_slider;
	[SerializeField]
	Slider max_burn_time_slider;
	[SerializeField]
	Slider plant_count_slider;
	[SerializeField]
	Text   wind_speed_num;
	[SerializeField]
	Text   fire_spread_rate_num;
	[SerializeField]
	Text   max_burn_time_num;
	[SerializeField]
	Text   plant_count_num;
	[SerializeField]
	Button game_mode_updater;
	[SerializeField]
	PlantFactory plant_factory;

	InteractionMode mode;

	// Use this for initialization
	void Start () {

		mode = InteractionMode.Add;

		//Add button listeners
		wind_speed_slider.onValueChanged.AddListener(delegate {plant_factory.UpdateWindSpeed(wind_speed_slider.value);});
		fire_spread_rate_slider.onValueChanged.AddListener(delegate {plant_factory.UpdateFireSpreadRate(fire_spread_rate_slider.value);});
		max_burn_time_slider.onValueChanged.AddListener(delegate {plant_factory.UpdateBurnTime(max_burn_time_slider.value);});
		game_mode_updater.onClick.AddListener(UpdateGameMode);
		//plant_count_slider.onValueChanged.AddListener(delegate {plant_factory.UpdatePlantCount(plant_count_slider.value);});

	}
	
	// Update is called once per frame
	void Update () {
		
		wind_speed_num.text = wind_speed_slider.value.ToString("## mph");
		fire_spread_rate_num.text = fire_spread_rate_slider.value.ToString("##");
		max_burn_time_num.text = max_burn_time_slider.value.ToString("## secs");
		//plant_count_num.text = plant_count_slider.value.ToString("0");

	}

	void UpdateGameMode(){

		InteractionMode mode = game_mode_updater.GetComponent<ModeLabelUpdater>().UpdateLabel();

		game_mode_updater.GetComponentInChildren<Text>().text = "Mode: " + mode.ToString();
		this.mode = mode;
	}

	public void QuiteGame(){
		Application.Quit();
	}

    internal InteractionMode GetMode()
    {
        return mode;
    }
}
