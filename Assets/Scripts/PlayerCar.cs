using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class PlayerCar : MonoBehaviour {
	public UnityEngine.UI.Text speedometer;
	public UnityEngine.UI.Text geartext;

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
		//Display Speed
		speedometer.text = string.Format ("{0:F0} MPH", rigidbody.velocity.magnitude * 2.2369f);
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
					DisplayGear();
				} else {
					controller.ShiftUp();
					DisplayGear();
				}
			}
		}
		// Car is in reverse
		else if (controller.CurrentGear < controller.NeutralGear) {
			float RPM = controller.GetRPM ();
			if (RPM > (controller.BitingRPM+controller.MaxRPM)*0.75f) {
				controller.ShiftDown();
				DisplayGear();
			} else if (RPM < (controller.BitingRPM+controller.MaxRPM)*0.25f){
				controller.ShiftUp();
				DisplayGear();
			}
		}
		// Car is driving
		else {
			float RPM = controller.GetRPM ();
			if (RPM > (controller.BitingRPM+controller.MaxRPM)*0.6f) {
				controller.ShiftUp();
				DisplayGear();
			} else if (RPM < (controller.BitingRPM+controller.MaxRPM)*0.25f){
				controller.ShiftDown();
				DisplayGear();
			}
		}
	}

	void DisplayGear () {
		if (geartext) {
			if (controller.CurrentGear==controller.NeutralGear){
				geartext.text = "Neutral";
			} else if (controller.CurrentGear<controller.NeutralGear){
				geartext.text = "Reverse";
			} else {
				geartext.text = "Gear: " + controller.CurrentGear.ToString();
			}
		}
	}

	/*void OnGUI(){
		GUI.Label (new Rect(10f, Screen.height-40f, Screen.width, 20f), string.Format("RPM: {0:0.00}, Speed: {1:0.00} MPH", controller.GetRPM(), rigidbody.velocity.magnitude*2.2369f));
	}*/
}
