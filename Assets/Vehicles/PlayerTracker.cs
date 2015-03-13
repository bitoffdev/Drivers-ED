using UnityEngine;
using System.Collections;

public class PlayerTracker : MonoBehaviour {
	public Transform arrow;
	public Transform[] waypoints;

	Transform currentWaypoint;
	int currentIndex = 0;
	Vector3 pointerTarget;

	void Start () {
		currentIndex = 0;
		currentWaypoint = waypoints[currentIndex];
		pointerTarget = new Vector3(currentWaypoint.position.x, 
		                            arrow.position.y, 
		                            currentWaypoint.position.z);
	}

	void Update () {
		Quaternion targetRotation = Quaternion.LookRotation(pointerTarget - arrow.position);
		arrow.rotation = Quaternion.Slerp(arrow.rotation, targetRotation, 3f * Time.deltaTime);

		//arrow.LookAt(pointerTarget);
	}

	void OnTriggerEnter (Collider col){
		if (col.transform == currentWaypoint){
			if (waypoints.Length > currentIndex+1){
				currentIndex+=1;
				currentWaypoint = waypoints[currentIndex];
				pointerTarget = new Vector3(currentWaypoint.position.x, 
				                            arrow.position.y, 
				                            currentWaypoint.position.z);
			}
		}
	}
}
