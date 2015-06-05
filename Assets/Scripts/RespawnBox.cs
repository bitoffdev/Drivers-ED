using UnityEngine;
using System.Collections;

/// <summary>
/// Respawner - respawns attached object if it exits the attached boxcollider trigger
/// </summary>
public class RespawnBox : MonoBehaviour {
	public Transform obj;
	Vector3 StartPoint;
	public Bounds box;

	void Start () {
		StartPoint = obj.position;
	}

	void Update() {
		if (!box.Contains (obj.position)) {
			obj.rotation = Quaternion.identity;
			obj.position = StartPoint;
		}
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube (box.center, box.size);
	}
}
