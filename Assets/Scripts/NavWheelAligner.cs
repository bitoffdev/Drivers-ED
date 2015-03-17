using UnityEngine;

/// <summary>
/// This script aligns a wheel's position to represent the suspension and match the
/// cooresponding WheelCollider. This script is meant to be used with any car that
/// is powered by a NavMeshAgent. It will use the NavMeshAgent's velocity to rotate
/// the wheels at the appropriate speed.
/// </summary>
public class NavWheelAligner : MonoBehaviour {
	public NavMeshAgent ParentNavAgent;
	public WheelCollider CorrespondingCollider;

	float circumference = 0.5f;
	
	void Start () {
		circumference = 2f * CorrespondingCollider.radius * Mathf.PI;
	}
	
	void Update () {
		// ===== Update Suspension =====
		RaycastHit hit;
		Vector3 ColliderCenterPoint = CorrespondingCollider.transform.TransformPoint(CorrespondingCollider.center);
		if (Physics.Raycast(ColliderCenterPoint, -CorrespondingCollider.transform.up, out hit, CorrespondingCollider.suspensionDistance + CorrespondingCollider.radius)) {
			transform.position = hit.point + (CorrespondingCollider.transform.up * CorrespondingCollider.radius);
		} else {
			transform.position = ColliderCenterPoint - (CorrespondingCollider.transform.up * CorrespondingCollider.suspensionDistance);
		}
		
		// =====Update Rotation =====
		float rps = ParentNavAgent.velocity.magnitude / circumference;
		transform.Rotate(Time.deltaTime * 360f * rps, 0f, 0f);
	}
}
