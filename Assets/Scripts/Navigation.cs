
using UnityEngine;
using System.Collections;


public class Navigation : MonoBehaviour {
	
	public Transform[] points;
	private int destPoint = 0;
	private NavMeshAgent agent;
	
	
	void Start () {
		agent = GetComponent<NavMeshAgent>();
		
		GotoNextPoint();
	}
	
	
	void GotoNextPoint() {
		// Returns if no points have been set up
		if (points.Length == 0)
			return;
		
		// Set the agent to go to the currently selected destination.
		agent.destination = points[destPoint].position;
		
		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % points.Length;
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
		if (agent.remainingDistance < 0.5f)
			GotoNextPoint();
	}
}