using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class PlayerCar : MonoBehaviour {
	CarController controller;
	float SteerAngle;

	void Start () {
		controller = gameObject.GetComponent<CarController> ();
	}
	
	void Update () {
		controller.SetAcceleration (Random.Range(-1, 1));
		controller.SetSteering (Random.Range(-1, 1));
	}

	void OnGUI(){
		GUI.Label (new Rect(10f, 10f, 200f, 20f), string.Format("RPM: {0:0.00}, Speed: {1:0.00} MPH", controller.GetRPM(), rigidbody.velocity.magnitude*2.2369f));
	}
}
