using UnityEngine;
using System.Collections;


public class Intersection : MonoBehaviour {
	class Path {
		public Transform[] points;
		public Path(params Transform[] pts){
			points = pts;
		}
	}

	public TrafficSignal[] TrafficSignals;
	public Transform[] StopZoneWaypoints;
	public Transform[] IntersectionWaypoints;

	Path[] paths = new Path[2];
	public int currentCycle = 0;

	void Start () {
		//paths [0] = new Path (StopZoneWaypoints[1], IntersectionWaypoints[1], IntersectionWaypoints[0]);
		//paths [1] = new Path ();

		InvokeRepeating("ChangeLight", 0f, 10f);
	}

	void ChangeLight(){
		currentCycle += 1;
		if (currentCycle >= paths.Length) {
			currentCycle = 0;
		}
		SetLights ();
	}

	void SetLights(){
		for (int i=0;i<TrafficSignals.Length;i++){
			TrafficSignal.SignalType sig = (currentCycle==0 && i%2==0) || (currentCycle==1 && i%2!=0) ? TrafficSignal.SignalType.Green : TrafficSignal.SignalType.Red;
			TrafficSignals[i].SetSignal(sig, 3f);
		}
	}

	void OnDrawGizmos () {
		for (int i=0; i<StopZoneWaypoints.Length; i++) {
			Gizmos.color = (currentCycle==0 && i%2==0) || (currentCycle==1 && i%2!=0) ? Color.green : Color.red;
			Gizmos.DrawCube(StopZoneWaypoints[i].position, Vector3.one);
		}
		Gizmos.color = Color.white;
		foreach (Transform pt in IntersectionWaypoints) {
			Gizmos.DrawSphere(pt.position, 0.5f);
		}
	}
	void OnTriggerEnter(Collider col){
		if (col.tag == "Player") {
			Vector3 Direction = (col.transform.position - transform.position).normalized;
			Quaternion Rotation = Quaternion.Inverse(transform.rotation) * Quaternion.LookRotation(Direction);
			int enteringnum = 0;
			if(Rotation.eulerAngles.y>315f||Rotation.eulerAngles.y<45f){
				enteringnum = 1;
			} else if(45f<Rotation.eulerAngles.y&&Rotation.eulerAngles.y<135f){
				enteringnum = 2;
			} else if(135f<Rotation.eulerAngles.y&&Rotation.eulerAngles.y<225f){
				enteringnum = 3;
			} else if(225f<Rotation.eulerAngles.y&&Rotation.eulerAngles.y<315f){
				enteringnum = 4;
			}
			if ((currentCycle==0&&enteringnum%2==0) || (currentCycle==1&&enteringnum%2!=0)){
				TrafficLaws laws = col.gameObject.GetComponentInParent<TrafficLaws>();
				Debug.Log (laws.status);
				laws.SetStatus(TrafficLaws.Status.RedLightViolation);
			}
		}
	}
}
