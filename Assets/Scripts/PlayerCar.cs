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
		// Acceleration
		CurrentAcceleration = Mathf.Lerp (CurrentAcceleration, Input.GetAxis ("Vertical"), Time.deltaTime*4);
		controller.SetAcceleration (CurrentAcceleration);
		// Steering
		CurrentSteering = Mathf.Lerp (CurrentSteering, Input.GetAxis ("Horizontal"), Time.deltaTime*4);
		controller.SetSteering (CurrentSteering);
		// Shift Gears
		AutomaticShift ();
	}

	void ManualShift () {
		if (Input.GetKeyDown(KeyCode.Minus)){controller.ShiftDown();}
		if (Input.GetKeyDown(KeyCode.Equals)){controller.ShiftUp();}
	}

	void AutomaticShift() {
		// Car is in neutral
		if (controller.CurrentGear == controller.NeutralGear) {
			float RPM = controller.GetRPM ();
			if (RPM > (controller.BitingRPM+controller.MaxRPM)*0.3f){
				if (Input.GetKey(KeyCode.R)){
					controller.ShiftDown();
				} else {
					controller.ShiftUp();
				}
			}
		}
		// Car is in reverse
		else if (controller.CurrentGear < controller.NeutralGear) {
			float RPM = controller.GetRPM ();
			if (RPM > (controller.BitingRPM+controller.MaxRPM)*0.75f) {
				controller.ShiftDown();
			} else if (RPM < (controller.BitingRPM+controller.MaxRPM)*0.25f){
				controller.ShiftUp();
			}
		}
		// Car is driving
		else {
			float RPM = controller.GetRPM ();
			if (RPM > (controller.BitingRPM+controller.MaxRPM)*0.6f) {
				controller.ShiftUp();
			} else if (RPM < (controller.BitingRPM+controller.MaxRPM)*0.25f){
				controller.ShiftDown();
			}
		}
	}

	void OnGUI(){
		GUI.Label (new Rect(10f, Screen.height-40f, Screen.width, 20f), string.Format("RPM: {0:0.00}, Speed: {1:0.00} MPH", controller.GetRPM(), rigidbody.velocity.magnitude*2.2369f));
	}
}
