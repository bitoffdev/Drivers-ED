using UnityEngine;
using System.Collections;

public class WheelAligner : MonoBehaviour {
	public WheelCollider CorrespondingCollider;
	private float CurrentRotation = 0f;
	
	void Update () {
		RaycastHit hit;
		// Get Wheel Collider's center
		Vector3 ColliderCenterPoint = CorrespondingCollider.transform.TransformPoint(CorrespondingCollider.center);
		// Cast a ray out from the wheel collider's center the distance of the suspension and set transform position
		if (Physics.Raycast(ColliderCenterPoint, -CorrespondingCollider.transform.up, out hit, CorrespondingCollider.suspensionDistance + CorrespondingCollider.radius)) {
			transform.position = hit.point + (CorrespondingCollider.transform.up * CorrespondingCollider.radius);
		} else {
			transform.position = ColliderCenterPoint - (CorrespondingCollider.transform.up * CorrespondingCollider.suspensionDistance);
		}
		// Set the transform rotation to the collider's rotation rotated by the CurrentRotation
		transform.rotation = CorrespondingCollider.transform.rotation * Quaternion.Euler(CurrentRotation, CorrespondingCollider.steerAngle, 0);
		// Increase the current rotation
		CurrentRotation += CorrespondingCollider.rpm * ( 360/60 ) * Time.deltaTime;
	}
}
