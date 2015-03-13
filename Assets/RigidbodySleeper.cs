using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodySleeper : MonoBehaviour {
	void Awake () {
		rigidbody.Sleep ();
	}
}
