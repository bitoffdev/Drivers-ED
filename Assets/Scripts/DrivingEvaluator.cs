using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CarController))]
public class DrivingEvaluator : MonoBehaviour
{
	// UI text to display notifications for the player
	public Text uitext;
	// Instructions - directs the player where to go
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

	void Start () {
		uitext.text = instructions [currentInstruction].text;
	}

	// CHECK IF THE CAR IS SPEEDING
	void Update () {
		DisplayLastViolation ();
		Vector3 heading = instructions [currentInstruction].waypoint.position - transform.position;
		float turnAngle = Vector3.Cross(transform.forward, heading).y;
		if (turnAngle > 55f){
			uitext.text += " Turn right.";
		} else if (turnAngle < -55f) {
			uitext.text += " Turn left.";
		} else {
			uitext.text += string.Format(" Continue for {0:F0} m.", heading.magnitude);
		}

		if (rigidbody.velocity.magnitude * 2.2369f > SpeedLimit) {
			//uitext.text = "You're speeding! " + uitext.text;
			AddViolation(Violation.Speeding);
		}
		if (Vector3.Distance (transform.position, instructions [currentInstruction].waypoint.position) < 10f) {
			if (instructions.Length > currentInstruction+1){
				currentInstruction+=1;
				uitext.text = instructions[currentInstruction].text;
			} else {
				uitext.text = "Drive complete";
				gameObject.GetComponent<CarController> ().enabled = false;
			}
		}
	}

	// CHECK IF THE PLAYER HAS REACHED THE NEXT WAYPOINT
	void OnTriggerEnter (Collider col){
		if (col.transform == instructions[currentInstruction].waypoint){
			if (instructions.Length > currentInstruction+1){
				currentInstruction+=1;
				uitext.text = instructions[currentInstruction].text;
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
