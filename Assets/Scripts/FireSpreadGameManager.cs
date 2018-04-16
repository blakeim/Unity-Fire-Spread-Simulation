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
	Slider fireSpreadRateSlider;
	[SerializeField]
	Slider windSpeedSlider;
	[SerializeField]
	Slider maxBurnTimeSlider;
	[SerializeField]
	Slider plantCountSlider;
	[SerializeField]
	Text   windSpeedNum;
	[SerializeField]
	Text   fireSpreadRateNum;
	[SerializeField]
	Text   maxBurnTimeNum;
	[SerializeField]
	Text   plantCountNum;
	[SerializeField]
	Button gameModeUpdater;
	[SerializeField]
	PlantFactory plantFactory;

	InteractionMode mode;

	// Use this for initialization
	void Start () {

		mode = InteractionMode.Add;

		//Add button listeners
		windSpeedSlider.onValueChanged.AddListener(delegate {plantFactory.UpdateWindSpeed(windSpeedSlider.value);});
		fireSpreadRateSlider.onValueChanged.AddListener(delegate {plantFactory.UpdateFireSpreadRate(fireSpreadRateSlider.value);});
		maxBurnTimeSlider.onValueChanged.AddListener(delegate {plantFactory.UpdateBurnTime(maxBurnTimeSlider.value);});
		gameModeUpdater.onClick.AddListener(UpdateGameMode);
		//plant_count_slider.onValueChanged.AddListener(delegate {plant_factory.UpdatePlantCount(plant_count_slider.value);});

	}
	
	// Update is called once per frame
	void Update () {
		
		windSpeedNum.text = windSpeedSlider.value.ToString("## mph");
		fireSpreadRateNum.text = fireSpreadRateSlider.value.ToString("##");
		maxBurnTimeNum.text = maxBurnTimeSlider.value.ToString("## secs");
		//plant_count_num.text = plant_count_slider.value.ToString("0");

	}

	void UpdateGameMode(){

		InteractionMode mode = gameModeUpdater.GetComponent<ModeLabelUpdater>().UpdateLabel();

		gameModeUpdater.GetComponentInChildren<Text>().text = "Mode: " + mode.ToString();
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
