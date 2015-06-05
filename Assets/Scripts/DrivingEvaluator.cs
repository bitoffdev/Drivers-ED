using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CarController))]
public class DrivingEvaluator : MonoBehaviour
{
	// UI text to display notifications for the player
	public Text instructiontext;
	// Instructions - directs the player where to go
	public Waypoint TargetWaypoint;
	int CompletedWaypoints = 0;
	float StartTime;
	// Violation variables - checks if the player obeys the traffic laws
	public enum Violation {Speeding, RedLight, Aggressive, Accident, DroveOffRoad, Lane}
	List<Violation> violations = new List<Violation>();
	// Status variables
	int onroad = 0;
	float SpeedLimit = 50f;

	void Start() {
		StartTime = Time.time;
	}

	// MAIN METHODS
	public void AddViolation(Violation v){
		if (violations.Count==0 || violations[violations.Count-1]!=v){
			violations.Add(v);
		}
	}

	public void DisplayLastViolation (){
		if (violations.Count == 0){instructiontext.text = "";return;}

		switch (violations [violations.Count - 1]) {
		case Violation.Accident:
			instructiontext.text = "You caused an accident!";
			break;
		case Violation.DroveOffRoad:
			instructiontext.text = "You drove off the Road!";
			break;
		case Violation.RedLight:
			instructiontext.text = "You drove through a red light!";
			break;
		case Violation.Speeding:
			instructiontext.text = "You were speeding!";
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
			instructiontext.text += " Turn right.";
		} else if (turnAngle < 305f && turnAngle > 235f) {
			instructiontext.text += " Turn left.";
		} else if (turnAngle >= 125f && turnAngle <= 235f) {
			instructiontext.text += " Make a U-turn.";
		} else {
			instructiontext.text += string.Format(" Continue for {0:F0} m.", heading.magnitude);
		}
		// Check if the car is speeding
		if (rigidbody.velocity.magnitude * 2.2369f > SpeedLimit) {
			AddViolation(Violation.Speeding);
		}
		// Check if the car has reached the next waypoint
		if (Vector3.Distance (transform.position, TargetWaypoint.transform.position) < 20f) {
			CompletedWaypoints++;
			if (CompletedWaypoints==5){
				instructiontext.text = string.Format("Drive Completed in {0:00}:{1:00}.", (Time.time-StartTime)/60, (Time.time-StartTime)%60);
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
			if (onroad==0){
				AddViolation(Violation.DroveOffRoad);
			}
		}
	}
}
