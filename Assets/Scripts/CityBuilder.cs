using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class CityBuilder : MonoBehaviour {

	[Header("Settings")]
	public bool GenerateOnload;

	public int BlocksX;
	public int BlocksZ;

	public float BlockSizeX;
	public float BlockSizeZ;

	public float RoadWidth;

	[Header("Assets")]
	public GameObject[] Buildings;
	public Material BlockMaterial;
	public Material RoadMaterial;
	public Material IntersectionMaterial;
	public GameObject IntersectionPrefab;
	public GameObject CpuCar;

	float CitySizeX, CitySizeZ;
	
	void Start () {
		if (GenerateOnload){
			BuildCity ();
		}
	}

	[ContextMenu("Destroy City")]
	void DestroyCity (){
		//Kill children
		//while (transform.childCount>0) {
		for (int i=0;i<transform.childCount;i++){
			GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}

	[ContextMenu("Build Now")]
	void BuildCity (){
		DestroyCity ();
		//Calculate values
		CitySizeX = BlocksX * (BlockSizeX + RoadWidth) + RoadWidth;
		CitySizeZ = BlocksZ * (BlockSizeZ + RoadWidth) + RoadWidth;
		// ===== Generate Blocks
		for (int x=0;x<BlocksX;x++){
			for (int z=0;z<BlocksZ;z++){
				// Calculate minimum point of the block
				Vector3 minPoint = new Vector3(-CitySizeX/2f + x*(BlockSizeX + RoadWidth) + RoadWidth, 0f, -CitySizeZ/2f + z*(BlockSizeZ + RoadWidth) + RoadWidth);
				// CREATE THE BLOCK GROUND
				Vector3[] points = new Vector3[4];
				points[0] = minPoint + new Vector3();
				points[1] = minPoint + new Vector3(0f,0f,BlockSizeZ);
				points[2] = minPoint + new Vector3(BlockSizeX,0f,BlockSizeZ);
				points[3] = minPoint + new Vector3(BlockSizeX,0f,0f);
				NewQuad (points, BlockMaterial, transform, true);
				// SCATTER BUILDINGS WITHIN THE BLOCK
				int tries = 0;
				List<GameObject> newobjects = new List<GameObject> ();
				while (tries<40){
					GameObject newobj = PlaceBuilding(Buildings[Random.Range (0, Buildings.Length)], minPoint);
					newobj.transform.SetParent(transform);
					if ( newobjects.FindIndex(p=>p.renderer.bounds.Intersects(newobj.renderer.bounds)) == -1){
						newobjects.Add(newobj);
					} else {
						GameObject.DestroyImmediate(newobj);
					}
					tries+=1;
				}
			}
		}
		GenerateRoads ();
	}

	#region MAKE BUILDINGS
	GameObject PlaceBuilding (GameObject building, Vector3 minPoint){
		//Load the bounds
		Bounds bounds = building.GetComponent<MeshFilter>().sharedMesh.bounds;
		//Multiply by scale
		bounds.center = Vector3.Scale(bounds.center, building.transform.localScale);
		bounds.size = Vector3.Scale (bounds.size, building.transform.localScale);
		//Calculate the range of points that the building can be placed in
		float minX = minPoint.x - bounds.min.x;
		float minZ = minPoint.z - bounds.min.z;
		float maxX = minPoint.x + BlockSizeX - bounds.max.x;
		float maxZ = minPoint.z + BlockSizeZ - bounds.max.z;
		if (minX > maxX || minZ > maxZ) throw new System.ArgumentException("The building will not fit in the block.");
		Vector3 buildingpos = new Vector3(Random.Range (minX, maxX),-bounds.min.y,Random.Range (minZ, maxZ));
		return Instantiate(building, buildingpos, Quaternion.identity) as GameObject;
	}
	#endregion

	#region MAKE GROUND
	void GenerateRoads(){
		GameObject RoadsParent = new GameObject ();
		RoadsParent.transform.parent = transform;
		RoadsParent.name = "Roads";
		// ===== Generate Roads
		for (int x=0;x<=BlocksX;x++){
			for (int z=0;z<=BlocksZ;z++){
				Vector3 StartPoint = new Vector3(-CitySizeX/2f + x*(BlockSizeX + RoadWidth), 0f, -CitySizeZ/2f + z*(BlockSizeZ + RoadWidth));
				// CREATE INTERSECTION
				Vector3[] IntersectionPoints = new Vector3[4];
				IntersectionPoints[0] = StartPoint + new Vector3(0f, 0f, 0f);
				IntersectionPoints[1] = StartPoint + new Vector3(0f, 0f, RoadWidth);
				IntersectionPoints[2] = StartPoint + new Vector3(RoadWidth, 0f, RoadWidth);
				IntersectionPoints[3] = StartPoint + new Vector3(RoadWidth, 0f, 0f);
				NewQuad (IntersectionPoints, IntersectionMaterial, RoadsParent.transform, true);
				// CREATE ROAD 1
				if (z!=BlocksZ){
					Vector3[] Road1Points = new Vector3[4];
					Road1Points[0] = StartPoint + new Vector3(0f, 0f, RoadWidth);
					Road1Points[1] = StartPoint + new Vector3(0f,0f,BlockSizeZ+RoadWidth);
					Road1Points[2] = StartPoint + new Vector3(RoadWidth,0f,BlockSizeZ+RoadWidth);
					Road1Points[3] = StartPoint + new Vector3(RoadWidth,0f,0f+RoadWidth);
					NewQuad (Road1Points, RoadMaterial, RoadsParent.transform, true);
				}
				// CREATE ROAD 2
				if (x!=BlocksX){
					Vector3[] Road2Points = new Vector3[4];
					Road2Points[0] = StartPoint + new Vector3(0f+RoadWidth, 0f, RoadWidth);
					Road2Points[1] = StartPoint + new Vector3(BlockSizeX+RoadWidth,0f,0f+RoadWidth);
					Road2Points[2] = StartPoint + new Vector3(BlockSizeX+RoadWidth,0f,0f);
					Road2Points[3] = StartPoint + new Vector3(0f+RoadWidth,0f,0f);
					NewQuad (Road2Points, RoadMaterial, RoadsParent.transform, true);
				}
			}
		}
	}

	static GameObject NewQuad(Vector3[] points, Material material, Transform parent){
		return NewQuad (points, material, parent, false);
	}

	static GameObject NewQuad(Vector3[] points, Material material, Transform parent, bool AddBoxCollider){
		GameObject obj = new GameObject();
		MeshFilter filter = obj.AddComponent<MeshFilter> ();
		filter.sharedMesh = ProceduralQuad(points);
		MeshRenderer objrenderer = obj.AddComponent<MeshRenderer>();
		objrenderer.material = material;
		obj.transform.SetParent(parent);
		if (AddBoxCollider) {
			BoxCollider objcol = obj.AddComponent<BoxCollider> ();
			objcol.size = new Vector3(objcol.size.x, 0.1f, objcol.size.z);
		}
		return obj;
	}

	static Mesh ProceduralQuad(Vector3[] points){
		//Check parameters
		if (points.Length != 4) {
			throw new System.ArgumentOutOfRangeException("points");
		}
		//Create the quad
		Mesh m = new Mesh ();
		m.name = "Block";
		m.vertices = points;
		m.uv = new Vector2[4]{new Vector2 (0f, 0f), new Vector2 (0f, 1f), new Vector2 (1f, 1f), new Vector2 (1f, 0f)};
		m.triangles = new int[6]{0,1,2,0,2,3};
		m.RecalculateBounds ();
		m.RecalculateNormals ();
		return m;
	}
	#endregion

	#region MAKE VEHICLES
	/*
	void GenerateWaypoints(){
		// Create the intersections
		List<GameObject> waypoints;
		for (int x=0;x<=BlocksX;x++){
			for (int z=0;z<=BlocksZ;z++){
				Vector3 StartPoint = new Vector3(-CitySizeX/2f + x*(BlockSizeX + RoadWidth), 0f, -CitySizeZ/2f + z*(BlockSizeZ + RoadWidth));
				waypoints.Add(Instantiate(IntersectionPrefab, StartPoint + new Vector3(RoadWidth/2f, 0f, RoadWidth/2f), Quaternion.identity));
			}
		}
		// Add some CPU cars
		if (BlocksX>1&&BlockSizeZ>1){
		int start = 0;//Random.Range (0, waypoints.Count);
			List<int> indices = new List<int>();
		int current = -1;
		int dirX = 1;
		int dirZ = 0;

		while (current!=start){

		}
		Instantiate (CpuCar, waypoints[start].position, Quaternion.identity);
		Navigation n = CpuCar.GetComponent<Navigation> ();
		n.points = new Transform[4];
		if ()
		}
	}
	*/
	#endregion
}
