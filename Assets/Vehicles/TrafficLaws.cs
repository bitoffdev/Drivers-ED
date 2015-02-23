using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class TrafficLaws : MonoBehaviour {
	public enum Status {Okay, SpeedViolation, RoadViolation, LaneViolation}
	public GameObject ground;
	public Status status =  Status.Okay;
	float CurrentSpeedLimit = 200f;
	CarController controller;
	
	public string StatusString(){
		if (status == Status.SpeedViolation){
			return "Speeding";
		} else if (status == Status.RoadViolation){
			return "Drove off road";
		} else if (status == Status.LaneViolation){
			return "Drove in wrong lane";
		}
		return "Okay";
	}

	void Update () {
		if (rigidbody.velocity.magnitude * 2.2369f > CurrentSpeedLimit) {
			status = Status.SpeedViolation;
		}
	}

	void OnTriggerEnter (Collider col) {
		SpeedZone sz = col.GetComponent<SpeedZone> ();
		if (sz!=null) {
			CurrentSpeedLimit = sz.SpeedLimit;
		}
	}
	void OnCollisionEnter (Collision col) {
		if (col.gameObject == ground) {
			status = Status.RoadViolation;
		}
	}
}
