using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {
	public WheelCollider FrontLeftWheel;
	public WheelCollider FrontRightWheel;
	public float[] GearRatio;
	private int CurrentGear = 0;
	public float EngineTorque = 230.0f;
	public float MaxEngineRPM = 3000.0f;
	public float MinEngineRPM = 1000.0f;
	private float EngineRPM = 0.0f;

	void Start () {
		rigidbody.centerOfMass += new Vector3(0f, -.75f, .25f);
	}
	
	// Update is called once per frame
	void Update () {
		// Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
		EngineRPM = (FrontLeftWheel.rpm + FrontRightWheel.rpm)/2f * GearRatio[CurrentGear];
		ShiftGears();
		
		// set the audio pitch to the percentage of RPM to the maximum RPM plus one, this makes the sound play
		// up to twice it's pitch, where it will suddenly drop when it switches gears.
		audio.pitch = Mathf.Abs(EngineRPM / MaxEngineRPM) + 1.0f ;
		// this line is just to ensure that the pitch does not reach a value higher than is desired.
		if (audio.pitch > 2.0f) {
			audio.pitch = 2.0f;
		}
		
		// finally, apply the values to the wheels.    The torque applied is divided by the current gear, and
		// multiplied by the user input variable.
		FrontLeftWheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * Input.GetAxis("Vertical");
		FrontRightWheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * Input.GetAxis("Vertical");
		
		// the steer angle is an arbitrary value multiplied by the user input.
		FrontLeftWheel.steerAngle = 10f * Input.GetAxis("Horizontal");
		FrontRightWheel.steerAngle = 10f * Input.GetAxis("Horizontal");
	}
	void ShiftGears(){
		// this funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
		// the engine RPM fall within the desired range. The gear is then set to this "appropriate" value.
		if ( EngineRPM >= MaxEngineRPM ) {
			int AppropriateGear = CurrentGear;
			
			for (int i = 0; i < GearRatio.Length; i++) {
				if (FrontLeftWheel.rpm * GearRatio[i] < MaxEngineRPM) {
					AppropriateGear = i;
					break;
				}
			}
			
			CurrentGear = AppropriateGear;
		}
		
		if ( EngineRPM <= MinEngineRPM ) {
			int AppropriateGear = CurrentGear;
			
			for (int j=GearRatio.Length-1; j>=0; j--) {
				if ( FrontLeftWheel.rpm * GearRatio[j] > MinEngineRPM ) {
					AppropriateGear = j;
					break;
				}
			}
			
			CurrentGear = AppropriateGear;
		}
	}
}