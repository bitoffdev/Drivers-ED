using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour {

	public Waypoint[] nextpoints;

	/*
	void OnDrawGizmos(){
		if (nextpoints!=null){
			Gizmos.color = Color.green;
			foreach (Waypoint pt in nextpoints) {
				Gizmos.DrawLine (transform.position, pt.transform.position);
			}
		}
	}*/
}
