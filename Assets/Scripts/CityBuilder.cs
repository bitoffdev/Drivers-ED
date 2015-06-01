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
		// Kill children
		while (transform.childCount>0) {
			GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}

	[ContextMenu("Build Now")]
	void BuildCity (){
		DestroyCity ();
		// Calculate values
		CitySizeX = BlocksX * (BlockSizeX + RoadWidth) + RoadWidth;
		CitySizeZ = BlocksZ * (BlockSizeZ + RoadWidth) + RoadWidth;
		// ===== Generate Blocks
		GameObject BuildingsParent = new GameObject ();
		BuildingsParent.transform.parent = transform;
		BuildingsParent.name = "Buildings";
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
				while (tries<200){
					GameObject newobj = PlaceBuilding(Buildings[Random.Range (0, Buildings.Length)], minPoint);
					if (newobj!=null){
						newobj.transform.SetParent(BuildingsParent.transform);
						if ( newobjects.FindIndex(p=>p.renderer.bounds.Intersects(newobj.renderer.bounds)) == -1){
							newobjects.Add(newobj);
						} else {
							GameObject.DestroyImmediate(newobj);
						}
					}
					tries+=1;
				}
			}
		}
		GenerateRoads ();
		GenerateWaypoints ();
	}

	#region MAKE BUILDINGS
	GameObject PlaceBuilding (GameObject building, Vector3 minPoint){
		//Load the bounds
		Bounds bounds = building.GetComponent<MeshFilter>().sharedMesh.bounds;
		bounds.center = Vector3.Scale (bounds.center, building.transform.localScale);
		bounds.size = Vector3.Scale (bounds.size, building.transform.localScale);
		//Calculate the range of points that the building can be placed in
		float minX = minPoint.x - bounds.min.x;
		float minZ = minPoint.z - bounds.min.z;
		float maxX = minPoint.x + BlockSizeX - bounds.max.x;
		float maxZ = minPoint.z + BlockSizeZ - bounds.max.z;
		//if (minX > maxX || minZ > maxZ) throw new System.ArgumentException("The building will not fit in the block.");
		if (minX > maxX || minZ > maxZ) return null;
		// Randomize the bulding's position
		Vector3 buildingpos = new Vector3(Random.Range (minX, maxX),-bounds.min.y,Random.Range (minZ, maxZ));
		// Snap the building to one of the sides
		if (Random.Range (0, 2) == 0) {
			buildingpos.x = Random.Range (0, 2)==0 ? minX : maxX;
		} else {
			buildingpos.z = Random.Range (0, 2)==0 ? minZ : maxZ;
		}
		// Return the building
		return Instantiate(building, buildingpos, Quaternion.identity) as GameObject;
	}
	#endregion

	void GenerateWaypoints() {
		GameObject WaypointsParent = new GameObject ();
		WaypointsParent.transform.parent = transform;
		WaypointsParent.name = "Waypoints";
		Waypoint[] pts = new Waypoint[(BlocksX+1)*(BlocksZ+1)*4];
		// ===== Generate Waypoints
		for (int x=0;x<=BlocksX;x++){
			for (int z=0;z<=BlocksZ;z++){
				Vector3 StartPoint = new Vector3(-CitySizeX/2f + x*(BlockSizeX + RoadWidth), 0f, -CitySizeZ/2f + z*(BlockSizeZ + RoadWidth));
				int counter = (x*(BlocksZ+1)+z)*4;
				// Create the waypoints
				pts[counter] = MakeWaypoint(StartPoint + new Vector3(RoadWidth*0.25f, 0f, RoadWidth*0.25f), WaypointsParent.transform);
				pts[counter+1] = MakeWaypoint(StartPoint + new Vector3(RoadWidth*0.75f, 0f, RoadWidth*0.25f), WaypointsParent.transform);
				pts[counter+2] = MakeWaypoint(StartPoint + new Vector3(RoadWidth*0.75f, 0f, RoadWidth*0.75f), WaypointsParent.transform);
				pts[counter+3] = MakeWaypoint(StartPoint + new Vector3(RoadWidth*0.25f, 0f, RoadWidth*0.75f), WaypointsParent.transform);
				// Name the waypoints
				pts[counter].gameObject.name = string.Format("X{0}Z{1}-BL", x, z);
				pts[counter+1].gameObject.name = string.Format("X{0}Z{1}-BR", x, z);
				pts[counter+2].gameObject.name = string.Format("X{0}Z{1}-TR", x, z);
				pts[counter+3].gameObject.name = string.Format("X{0}Z{1}-TL", x, z);
			}
		}
		// ===== Create Paths
		for (int x=0;x<=BlocksX;x++){
			for (int z=0;z<=BlocksZ;z++){
				int counter = (x*(BlocksZ+1)+z)*4;
				List<Waypoint> next1 = new List<Waypoint>();
				List<Waypoint> next2 = new List<Waypoint>();
				List<Waypoint> next3 = new List<Waypoint>();
				List<Waypoint> next4 = new List<Waypoint>();
				if (z>0){
					next1.Add (pts[counter-1]);
					next3.Add (pts[counter-1]);
					next4.Add (pts[counter-1]);
				}
				if (z<BlocksZ){
					next1.Add (pts[counter+5]);
					next2.Add (pts[counter+5]);
					next3.Add (pts[counter+5]);
				}
				if (x>0){
					next2.Add (pts[counter-4*(BlocksZ+1)+2]);
					next3.Add (pts[counter-4*(BlocksZ+1)+2]);
					next4.Add (pts[counter-4*(BlocksZ+1)+2]);
				}
				if (x<BlocksX){
					next1.Add (pts[counter+4*(BlocksZ+1)]);
					next2.Add (pts[counter+4*(BlocksZ+1)]);
					next4.Add (pts[counter+4*(BlocksZ+1)]);
				}
				pts[counter].nextpoints = next1.ToArray();
				pts[counter+1].nextpoints = next2.ToArray();
				pts[counter+2].nextpoints = next3.ToArray ();
				pts[counter+3].nextpoints = next4.ToArray ();
			}
		}
	}

	Waypoint MakeWaypoint(Vector3 pos, Transform parent){
		GameObject pt = new GameObject();
		pt.transform.position = pos;
		pt.transform.parent = parent;
		return pt.AddComponent<Waypoint> ();
	}

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
					/*Vector3[] Road1Points = new Vector3[4];
					Road1Points[0] = StartPoint + new Vector3(0f, 0f, RoadWidth);
					Road1Points[1] = StartPoint + new Vector3(0f,0f,BlockSizeZ+RoadWidth);
					Road1Points[2] = StartPoint + new Vector3(RoadWidth,0f,BlockSizeZ+RoadWidth);
					Road1Points[3] = StartPoint + new Vector3(RoadWidth,0f,0f+RoadWidth);
					NewQuad (Road1Points, RoadMaterial, RoadsParent.transform, true);*/
					BuildRect(StartPoint + new Vector3(0f, 0f, RoadWidth), StartPoint + new Vector3(RoadWidth, 0f, BlockSizeZ+RoadWidth), true, RoadMaterial, RoadsParent.transform, true);
				}
				// CREATE ROAD 2
				if (x!=BlocksX){
					/*Vector3[] Road2Points = new Vector3[4];
					Road2Points[0] = StartPoint + new Vector3(0f+RoadWidth, 0f, RoadWidth);
					Road2Points[1] = StartPoint + new Vector3(BlockSizeX+RoadWidth,0f,0f+RoadWidth);
					Road2Points[2] = StartPoint + new Vector3(BlockSizeX+RoadWidth,0f,0f);
					Road2Points[3] = StartPoint + new Vector3(0f+RoadWidth,0f,0f);
					NewQuad (Road2Points, RoadMaterial, RoadsParent.transform, true);*/
					BuildRect(StartPoint + new Vector3(RoadWidth, 0f, 0f), StartPoint + new Vector3(BlockSizeX+RoadWidth, 0f, RoadWidth), false, RoadMaterial, RoadsParent.transform, true);
				}
			}
		}
	}

	static GameObject BuildRect(Vector3 minPoint, Vector3 maxPoint, bool flipUVs, Material mat, Transform parent, bool AddBoxCollider){
		// Create Object
		GameObject obj = BuildRect (minPoint, maxPoint, flipUVs);
		// Add Renderer
		MeshRenderer objrenderer = obj.AddComponent<MeshRenderer>();
		objrenderer.material = mat;
		obj.transform.SetParent(parent);
		// Add Box Collider
		if (AddBoxCollider) {
			BoxCollider objcol = obj.AddComponent<BoxCollider> ();
			objcol.size = new Vector3(objcol.size.x, 0.1f, objcol.size.z);
		}
		//Return the object
		return obj;
	}

	static GameObject BuildRect(Vector3 minPoint, Vector3 maxPoint, bool flipUVs){
		//Create the quad
		Mesh m = new Mesh ();
		m.name = "Block";
		Vector3[] verts = new Vector3[4];
		verts [0] = new Vector3 (minPoint.x, 0f, minPoint.z);
		verts [1] = new Vector3 (minPoint.x, 0f, maxPoint.z);
		verts [2] = new Vector3 (maxPoint.x, 0f, maxPoint.z);
		verts [3] = new Vector3 (maxPoint.x, 0f, minPoint.z);
		m.vertices = verts;
		// Calculate UVs to be proportional
		float unit = Mathf.Min (maxPoint.x - minPoint.x, maxPoint.z - minPoint.z);
		float maxU = (maxPoint.x - minPoint.x)/unit;
		float maxV = (maxPoint.z - minPoint.z)/unit;
		if (flipUVs) {
			m.uv = new Vector2[4]{new Vector2 (0f, 0f), new Vector2 (maxV, 0f), new Vector2 (maxV, maxU), new Vector2 (0f, maxU)};
		} else {
			m.uv = new Vector2[4]{new Vector2 (0f, 0f), new Vector2 (0f, maxV), new Vector2 (maxU, maxV), new Vector2 (maxU, 0f)};
		}
		m.triangles = new int[6]{0,1,2,0,2,3};
		m.RecalculateBounds ();
		m.RecalculateNormals ();
		// Create the GameObject and attach the mesh
		GameObject obj = new GameObject();
		MeshFilter filter = obj.AddComponent<MeshFilter> ();
		filter.sharedMesh = m;
		return obj;
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
