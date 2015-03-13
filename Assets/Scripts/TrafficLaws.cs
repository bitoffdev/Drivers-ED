using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CarController))]
public class TrafficLaws : MonoBehaviour {
	public enum Status {Okay, SpeedViolation, RoadViolation, LaneViolation, CollisionViolation}
	public GameObject ground;
	public Status status =  Status.Okay;
	float CurrentSpeedLimit = 200f;
	CarController controller;
	int onRoad = 0;
	
	public string StatusString(){
		if (status == Status.SpeedViolation){
			return "Speeding";
		} else if (status == Status.RoadViolation){
			return "Drove off road";
		} else if (status == Status.LaneViolation){
			return "Drove in wrong lane";
		} else if (status == Status.CollisionViolation){
			return "You crashed!";
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
		if (col.gameObject.layer==LayerMask.NameToLayer("Roads")){
			onRoad += 1;
		} else if (col.gameObject.tag!="Ground"){
			status = Status.CollisionViolation;
		}
		/*if (col.gameObject.layer != LayerMask.NameToLayer ("Roads")) {
			if (col.gameObject.tag=="Ground"){
				status = Status.RoadViolation;
			}
			status = Status.CollisionViolation;
		}*/
		//if (col.gameObject == ground) {
		//	status = Status.RoadViolation;
		//}
	}
		void OnCollisionExit (Collision col) {
			if (col.gameObject.layer==LayerMask.NameToLayer("Roads")){
				onRoad -= 1;
				if (onRoad==0){
					status = Status.RoadViolation;
				}
			}
		}
}