using UnityEngine;
using System.Collections;

public class CpuCar : MonoBehaviour {

	public Waypoint targetWaypoint;
	NavMeshAgent agent;

	void Start () {
		agent = GetComponent<NavMeshAgent>();
		agent.destination = targetWaypoint.transform.position;
	}
	
	/*void OnTriggerEnter() {
		// Causes car to stop when it enters the collider of a
		// cube for three seconds.
		if (GameObject.tag == "stop") {
			WaitForSeconds(3);
		}
	}*/
	
	void Update () {
		// Choose the next destination point when the agent gets
		// close to the current one.
		if (agent.remainingDistance < 1.0f) {
			targetWaypoint = targetWaypoint.nextpoints[Random.Range(0, targetWaypoint.nextpoints.Length)];
			agent.destination = targetWaypoint.transform.position;
		}
	}
}
