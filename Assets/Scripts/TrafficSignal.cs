using UnityEngine;
using System.Collections;

public class TrafficSignal : MonoBehaviour {
	public enum SignalType {Green, Yellow, Red}

	public Light Green;
	public Light Yellow;
	public Light Red;

	void Start () {
	}
	
	IEnumerator MyDelayMethod(float delay, SignalType nextSig)
	{
		yield return new WaitForSeconds(delay);
		SetSignalNow (nextSig);
	}
	public void SetSignal(SignalType sig, float delay){
		//Don't use timer if there is no delay
		if (delay <= 0f) {
			SetSignalNow(sig);
			return;
		}
		//Use yellow light when switching to red and using a delay
		if (sig==SignalType.Red){
			SetSignalNow (SignalType.Yellow);
		}
		//Use timer if there is a delay
		StartCoroutine(MyDelayMethod(delay, sig));
	}
	void SetSignalNow(SignalType sig){
		if(sig==SignalType.Green){
			Green.enabled = true;
		} else {
			Green.enabled = false;
		}
		if (sig==SignalType.Yellow) {
			Yellow.enabled = true;
		} else {
			Yellow.enabled = false;
		}
		if (sig==SignalType.Red) {
			Red.enabled = true;
		} else {
			Red.enabled = false;
		}
	}
	/*
	/// <summary>
	/// Sets the signal.
	/// </summary>
	/// <param name="sig">Sig - the signal to change to</param>
	/// <param name="timer">Timer - the time it takes to get to the new state.
	/// If the light is going from green to red, the light will be yellow for this time.</param>
	void SetSignal(SignalType sig) {
		//Green
		if (sig != SignalType.Green) {

		}
		Green.enabled 
	}

	IEnumerator StartTimer () 
	{
		yield return new WaitForSeconds(Timer);

	}
	
	// Update is called once per frame
	void Update () {
	
	}*/
}
