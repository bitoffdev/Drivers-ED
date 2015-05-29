using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class PlayerCar : MonoBehaviour {
	CarController controller;
	float CurrentAcceleration = 0f;
	float CurrentSteering = 0f;

	void Start () {
		controller = gameObject.GetComponent<CarController> ();
	}
	
	void Update () {
		//Acceleration
		CurrentAcceleration = Mathf.Lerp (CurrentAcceleration, Input.GetAxis ("Vertical"), Time.deltaTime*4);
		controller.SetAcceleration (CurrentAcceleration);
		//Steering
		CurrentSteering = Mathf.Lerp (CurrentSteering, Input.GetAxis ("Horizontal"), Time.deltaTime*4);
		controller.SetSteering (CurrentSteering);
	}

	void OnGUI(){
		GUI.Label (new Rect(10f, Screen.height-40f, Screen.width, 20f), string.Format("RPM: {0:0.00}, Speed: {1:0.00} MPH, Violation: {2}", controller.GetRPM(), rigidbody.velocity.magnitude*2.2369f, ""));
	}
}
