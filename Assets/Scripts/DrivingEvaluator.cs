using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(CarController))]
public class DrivingEvaluator : MonoBehaviour
{
	// Violation variables
	public enum Violation {Speeding, RedLight, Aggressive, Accident, DroveOffRoad, Lane}
	List<Violation> violations = new List<Violation>();
	// Status variables
	int onroad = 0;
	float SpeedLimit = 50f;
	// UI Text used to display violations
	public Text uitext;

	// MAIN METHODS
	public void AddViolation(Violation v){
		violations.Add(v);
		DisplayLastViolation ();
	}

	public void DisplayLastViolation (){
		if (violations.Count == 0){return;}

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

	// CHECK IF THE CAR IS SPEEDING
	void Update () {
		if (rigidbody.velocity.magnitude * 2.2369f > SpeedLimit) {
			AddViolation(Violation.Speeding);
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
