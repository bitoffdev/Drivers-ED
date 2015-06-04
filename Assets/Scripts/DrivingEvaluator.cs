using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CarController))]
public class DrivingEvaluator : MonoBehaviour
{
	// UI text to display notifications for the player
	public Text uitext;
	// Instructions - directs the player where to go
	/*
	[System.Serializable]
	public class Instruction {
		public Transform waypoint;
		public string text;
		public Instruction(Transform _waypoint, string _text){
			waypoint = _waypoint;
			text = _text;
		}
	}
	public Instruction[] instructions;
	int currentInstruction = 0;
	*/
	public Waypoint TargetWaypoint;
	int CompletedWaypoints = 0;
	//float StartTime;
	//int CurrentWaypoint = 0;
	// Violation variables - checks if the player obeys the traffic laws
	public enum Violation {Speeding, RedLight, Aggressive, Accident, DroveOffRoad, Lane}
	List<Violation> violations = new List<Violation>();
	// Status variables
	int onroad = 0;
	float SpeedLimit = 50f;

	// MAIN METHODS
	public void AddViolation(Violation v){
		if (violations.Count==0 || violations[violations.Count-1]!=v){
			violations.Add(v);
		}
		//DisplayLastViolation ();
	}

	public void DisplayLastViolation (){
		if (violations.Count == 0){uitext.text = "";return;}

		switch (violations [violations.Count - 1]) {
		case Violation.Accident:
			uitext.text = "You caused an accident!";
			break;
		case Violation.DroveOffRoad:
			uitext.text = "You drove off the Road!";
			break;
		case Violation.RedLight:
			uitext.text = "You drove through a red light!";
			break;
		case Violation.Speeding:
			uitext.text = "You were speeding!";
			break;
		}
	}

	void Update () {
		// Exit early if the game is completed
		if (CompletedWaypoints >= 5) {
			return;
		}
		// Display the directions
		DisplayLastViolation ();
		Vector3 heading = TargetWaypoint.transform.position - transform.position;
		float turnAngle = Quaternion.FromToRotation (transform.forward, heading).eulerAngles.y;; 
		if (turnAngle > 55f && turnAngle < 125f){
			uitext.text += " Turn right.";
		} else if (turnAngle < 305f && turnAngle > 235f) {
			uitext.text += " Turn left.";
		} else if (turnAngle >= 125f && turnAngle <= 235f) {
			uitext.text += " Make a U-turn.";
		} else {
			uitext.text += string.Format(" Continue for {0:F0} m.", heading.magnitude);
		}
		// Check if the car is speeding
		if (rigidbody.velocity.magnitude * 2.2369f > SpeedLimit) {
			AddViolation(Violation.Speeding);
		}
		// Check if the car has reached the next waypoint
		if (Vector3.Distance (transform.position, TargetWaypoint.transform.position) < 20f) {
			CompletedWaypoints++;
			if (CompletedWaypoints==5){
				uitext.text = string.Format("Drive Completed in {0:00}:{1:00}.", Time.time/60, Time.time%60);
				gameObject.GetComponent<CarController> ().enabled = false;
			} else {
				TargetWaypoint = TargetWaypoint.nextpoints[Random.Range (0, TargetWaypoint.nextpoints.Length)];
			}
		}
	}

	// CHECK IF THE CAR IS ON THE ROAD
	void OnCollisionEnter (Collision col) {
		if (col.gameObject.layer==LayerMask.NameToLayer("Roads")){
			onroad += 1;
		}
	}
	void OnCollisionExit (Collision col) {
		if (col.gameObject.layer==LayerMask.NameToLayer("Roads")){
			onroad -= 1;
			Debug.Log (onroad);
			if (onroad==0){
				AddViolation(Violation.DroveOffRoad);
			}
		}
	}
}
