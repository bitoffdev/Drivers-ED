using UnityEngine;

/// <summary>
/// This script aligns a wheel's position to represent the suspension and match the
/// cooresponding WheelCollider. This script is meant to be used with any car that
/// is powered by a NavMeshAgent. It will use the NavMeshAgent's velocity to rotate
/// the wheels at the appropriate speed.
/// </summary>
public class NavWheelAligner : MonoBehaviour {
	public NavMeshAgent ParentNavAgent;
	//public WheelCollider CorrespondingCollider;
	public float wheelRadius;
	public float suspensionDistance;
	Vector3 DefaultPosition;

	float circumference = 0.5f;
	
	void Start () {
		DefaultPosition = transform.localPosition;
		circumference = 2f * wheelRadius * Mathf.PI;
	}
	
	void Update () {
		// ===== Update Suspension =====
		RaycastHit hit;
		Vector3 ColliderCenterPoint = transform.parent.TransformPoint(DefaultPosition);//CorrespondingCollider.transform.TransformPoint(CorrespondingCollider.center);
		if (Physics.Raycast(ColliderCenterPoint, -ParentNavAgent.transform.up, out hit, suspensionDistance + wheelRadius)) {
			transform.position = hit.point + (-ParentNavAgent.transform.up * wheelRadius);
		} else {
			transform.position = ColliderCenterPoint - (ParentNavAgent.transform.up * suspensionDistance);
		}
		
		// =====Update Rotation =====
		float rps = ParentNavAgent.velocity.magnitude / circumference;
		transform.Rotate(Time.deltaTime * 360f * rps, 0f, 0f);
	}

#if UNITY_EDITOR
	void OnDrawGizmosSelected(){
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireDisc(transform.position, transform.right, wheelRadius);
	}
#endif
}
