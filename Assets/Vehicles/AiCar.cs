using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class AiCar : MonoBehaviour {
	CarController controller;
	float SteerAngle;

	void Start () {
		controller = gameObject.GetComponent<CarController> ();
		// We need to make SteerAngle be able to range from 0 degrees to 360 degrees.
		SteerAngle = Random.Range (-1, 1);
		controller.SetSteering (SteerAngle);
	}
	
	void Update () {
		controller.SetAcceleration (Random.Range (0f, 1f));

	}

	//void OnGUI(){
	//	GUI.Label (new Rect(10f, 100f, 200f, 20f), string.Format("CPU - RPM: {0:0.00}, Speed: {1:0.00} MPH", controller.GetRPM(), rigidbody.velocity.magnitude*2.2369f));
	//}
}
