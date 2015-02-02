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

	private float CurrentThrottle = 0f;
	private float CurrentSteerAngle = 0f;

	public Vector3 MassCenterOffset = Vector3.zero;

	void Start () {
		rigidbody.centerOfMass += MassCenterOffset;
	}
	/// <summary>
	/// Sets the throttle.
	/// </summary>
	/// <param name="throttle">Should be between -1 and 1</param>
	public void SetThrottle(float throttle){
		CurrentThrottle = Mathf.Clamp(throttle, -1f, 1f);
	}
	/// <summary>
	/// Sets the steering angle.
	/// </summary>
	/// <param name="angle">Angle</param>
	public void SetSteerAngle(float angle){
		CurrentSteerAngle = angle;
	}
	void Update () {
		// Compute the engine RPM based on the average RPM of the two wheels, then call the shift gear function
		EngineRPM = (FrontLeftWheel.rpm + FrontRightWheel.rpm)/2f * GearRatio[CurrentGear];
		ShiftGears();
		
		// Set the audio pitch to the percentage of RPM to the maximum RPM plus one, this makes the sound play
		// up to twice it's pitch, where it will suddenly drop when it switches gears.
		audio.pitch = Mathf.Min (Mathf.Abs(EngineRPM / MaxEngineRPM) + 1.0f, 2f);

		// Apply values to the Wheel controllers
		FrontLeftWheel.motorTorque = EngineTorque / GearRatio [CurrentGear] * CurrentThrottle;
		FrontRightWheel.motorTorque = EngineTorque / GearRatio[CurrentGear] * CurrentThrottle;
		FrontLeftWheel.steerAngle = CurrentSteerAngle;
		FrontRightWheel.steerAngle = CurrentSteerAngle;
	}
	void ShiftGears(){
		// This funciton shifts the gears of the vehcile, it loops through all the gears, checking which will make
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