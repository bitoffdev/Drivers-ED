using UnityEngine;
using System.Collections;

// 2014 Cadillac CTS
public class CarController : MonoBehaviour {
	//Input
	public WheelCollider[] FrontWheels;
	float AccelerationInput;
	float BrakeInput;
	float SteerInput;
	// Gears
	float[] gearRatios = {-4.06f, 0f, 4.06f, 2.37f, 1.55f, 1.16f, 0.85f, 0.67f}; // Engine to transmission revolution ratio
	float DifferentialGearRatio = 3.45f; // Transmission to tire revolution ratio (also called final drive ratio)
	//Settings
	public float BitingRPM = 800f;
	public float PeakRPM = 3000f;
	public float MaxRPM = 6000f;
	//float[] gearRatios = {-4.23f, 0f, 4.23f, 2.47f, 1.67f, 1.23f, 1.00f, 0.79f};
	//float MinRPM = 200f;
	//float DownshiftRPM = 600f;
	//float PeakRPM = 2000f;
	//float MaxRPM = 5000f;
	float minTorque = 10f;
	float maxTorque = 45f;
	//Data
	public int NeutralGear = 1;
	public int CurrentGear = 1;
	float CurrentEngineRPM = 0f;
	float CurrentWheelTorque;
	float CurrentBrakeTorque;
	/// <summary>
	/// Sets the acceleration input. The value should be between -1 and 1. Negative values apply brake.
	/// </summary>
	public void SetAcceleration(float val){
		AccelerationInput = val;
	}
	/// <summary>
	/// Sets the steering input. The value should be between -1 and 1.
	/// </summary>
	/// <param name="val">Value.</param>
	public void SetSteering(float val){
		SteerInput = val;
	}
	public float GetRPM(){
		return CurrentEngineRPM;
	}
	public void ShiftUp(){
		CurrentGear = Mathf.Min (CurrentGear+1, gearRatios.Length-1);
	}
	public void ShiftDown(){
		CurrentGear = Mathf.Max (CurrentGear-1, 0);
	}

	void Update ()
	{
		UpdateEngine();
		//ShiftGears ();
		UpdateCar();

		//audio.pitch = Mathf.Min (Mathf.Abs(CurrentEngineRPM / MaxRPM), 2f);
	}

	void UpdateCar (){
		foreach (WheelCollider w in FrontWheels) {
			w.motorTorque = CurrentWheelTorque;
			w.steerAngle = Mathf.LerpAngle(SteerInput*30, SteerInput*5, rigidbody.velocity.magnitude / 30f);
			w.brakeTorque = CurrentBrakeTorque;
		}
	}
	
	
	void UpdateEngine()
	{
		CurrentWheelTorque = 0f;

		// ================= Calculate average wheel RPM
		float wheelRPM = 0f;
		foreach (WheelCollider w in FrontWheels) {
			wheelRPM += w.rpm;
		}
		wheelRPM /= FrontWheels.Length;

		// ================= Calculate brake
		CurrentBrakeTorque = Mathf.Max (-AccelerationInput, 0f) * 80f;

		if (CurrentGear != NeutralGear) {
			// Engine Shaft RPM
			float EngineShaftRPM = wheelRPM * gearRatios[CurrentGear] * DifferentialGearRatio;
			// Lerp the Engine RPM toward its shaft RPM
			CurrentEngineRPM = Mathf.MoveTowards(CurrentEngineRPM, EngineShaftRPM, (MaxRPM - BitingRPM) * Time.deltaTime);
			// Calculate wheel torque to apply
			CurrentWheelTorque = GetEngineTorqueFromRPM(CurrentEngineRPM) * Mathf.Max (AccelerationInput, 0f) * gearRatios[CurrentGear];
		} else {
			// Idle Engine
			float TargetRPM = Mathf.Lerp(BitingRPM, MaxRPM, AccelerationInput);
			CurrentEngineRPM += (TargetRPM - CurrentEngineRPM) * Time.deltaTime;
		}
	}

	/*void ShiftGears(){
		if (CurrentGear == NeutralGear && CurrentEngineRPM > 400f) {
			if (AccelerationInput<0f){
				CurrentGear--;
			} else {
				CurrentGear++;
			}
		} else if (CurrentGear < gearRatios.Length - 1 && CurrentEngineRPM > PeakRPM && AccelerationInput > 0.2f){
			CurrentGear++;
		} else if (CurrentGear > NeutralGear && CurrentEngineRPM < DownshiftRPM){
			CurrentGear--;
		}
	}*/
	
	
	float GetEngineTorqueFromRPM(float RPM)
	{
		// Car stalled
		if (RPM < BitingRPM) {
			return 0f;
		}
		else if (RPM < PeakRPM) {
			// Ramp up to the peak RPM value
			float difference = RPM - BitingRPM;
			float x = difference / (PeakRPM - BitingRPM);
			
			return Mathf.Lerp(minTorque, maxTorque, x);
		} else {
			// Ramp down from peak to max
			float difference = RPM - PeakRPM;
			float x = difference / BitingRPM;
			return Mathf.Lerp(maxTorque, minTorque, x);
		}
	}
}
