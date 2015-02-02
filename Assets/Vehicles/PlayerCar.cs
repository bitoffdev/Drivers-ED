using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class PlayerCar : MonoBehaviour {
	CarController controller;

	void Start () {
		controller = gameObject.GetComponent<CarController> ();
	}
	
	void Update () {
		controller.SetThrottle (Input.GetAxis("Vertical"));
		controller.SetSteerAngle (20f * Input.GetAxis("Horizontal"));
	}
}
