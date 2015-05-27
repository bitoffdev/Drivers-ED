using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class PlayerCar : MonoBehaviour {
	CarController controller;
	float SteerAngle;
	float CurrentAcceleration = 0f;

	void Start () {
		controller = gameObject.GetComponent<CarController> ();
	}
	
	void Update () {
		CurrentAcceleration = Mathf.Lerp (CurrentAcceleration, Input.GetAxis ("Vertical"), Time.deltaTime*4);
		controller.SetAcceleration (CurrentAcceleration);
		controller.SetSteering (Input.GetAxis("Horizontal"));
	}

	/*void OnGUI(){
		GUI.Label (new Rect(10f, Screen.height-40f, Screen.width, 20f), string.Format("RPM: {0:0.00}, Speed: {1:0.00} MPH, Violation: {2}", controller.GetRPM(), rigidbody.velocity.magnitude*2.2369f, ""));
	}*/
}
