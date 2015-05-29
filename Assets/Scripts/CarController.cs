using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {
	//Input
	public WheelCollider[] FrontWheels;
	float AccelerationInput;
	float BrakeInput;
	float SteerInput;
	//Settings
	float[] gearRatios = {-4.23f, 0f, 4.23f, 2.47f, 1.67f, 1.23f, 1.00f, 0.79f};
	float MinRPM = 200f;
	float DownshiftRPM = 600f;
	float PeakRPM = 2000f;
	float MaxRPM = 5000f;
	float minTorque = 10f;
	float maxTorque = 40f;
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

	void Update ()
	{
		UpdateEngine();
		ShiftGears ();
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
		// Calculate average wheel RPM
		float wheelRPM = 0f;
		foreach (WheelCollider w in FrontWheels) {
			wheelRPM += w.rpm;
		}
		wheelRPM /= FrontWheels.Length;

		CurrentBrakeTorque = Mathf.Max (-AccelerationInput, 0f) * 80f;
		if (CurrentGear != NeutralGear) {
			// Engine Shaft RPM
			float EngineShaftRPM = wheelRPM * gearRatios[CurrentGear];
			// Lerp the Engine RPM toward its shaft RPM
			CurrentEngineRPM = Mathf.MoveTowards(CurrentEngineRPM, EngineShaftRPM, (MaxRPM - MinRPM) * Time.deltaTime);
			// Calculate wheel torque to apply
			CurrentWheelTorque = GetEngineTorqueFromRPM(CurrentEngineRPM) * Mathf.Max (AccelerationInput, 0f) * gearRatios[CurrentGear];
		} else {
			// Idle Engine
			float TargetRPM = Mathf.Lerp(MinRPM, MaxRPM, AccelerationInput);
			CurrentEngineRPM += (TargetRPM - CurrentEngineRPM) * Time.deltaTime;
		}
	}

	void ShiftGears(){
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
	}
	
	
	float GetEngineTorqueFromRPM(float RPM)
	{
		if (RPM < MinRPM) {
			return 0f; //Car stalled
		} else if (RPM < PeakRPM) {
			// Ramp up to the peak RPM value
			float difference = RPM - MinRPM;
			float x = difference / (PeakRPM - MinRPM);
			
			return Mathf.Lerp(minTorque, maxTorque, x);
		} else {
			// Ramp down from peak to max
			float difference = RPM - PeakRPM;
			float x = difference / MinRPM;
			return Mathf.Lerp(maxTorque, minTorque, x);
		}
	}
}
