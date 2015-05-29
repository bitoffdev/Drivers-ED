var minForce = 1.0;
var multiplier = 0.1;
var deformRadius = 1.0;
var maxDeform = 0.0;
var bounceBackSpeed = 0.0;
var bounceBackSleepCap = 0.001;
var onCollision = true;
var onCall = true;
var updateCollider = false;
var updateColliderOnBounce = false;

private var mesh : Mesh;
mesh = GetComponent(MeshFilter).mesh;

if (!GetComponent(MeshCollider)) {updateCollider = false;updateColliderOnBounceBack = false;}

private var permaVerts : Vector3[];
permaVerts = GetComponent(MeshFilter).mesh.vertices;
private var sleep = true;
//--------------------------------------------------------------
function OnCollisionEnter (collision : Collision) {
	if (onCollision && collision.relativeVelocity.magnitude >= minForce) {
		sleep = false;
		var vertices = mesh.vertices;
		tf = transform.worldToLocalMatrix;
		for (var i=0;i<vertices.length;i++) {
			for (var contact in collision.contacts) {
				point = tf.MultiplyPoint(contact.point);
				vec = tf.MultiplyVector(collision.relativeVelocity*UsedMass(collision));
				if ((point-vertices[i]).magnitude < deformRadius) {
					vertices[i] += vec*(deformRadius-(point-vertices[i]).magnitude)/deformRadius*multiplier;
					if (maxDeform > 0 && (vertices[i]-permaVerts[i]).magnitude > maxDeform) {
						vertices[i] = permaVerts[i] + (vertices[i]-permaVerts[i]).normalized*maxDeform;
					}
				}
			}
			
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
  		if (updateCollider) {GetComponent(MeshCollider).sharedMesh = mesh;}
	}
}
//--------------------------------------------------------------
function Deform(point : Vector3, direction : Vector3) {
	if (onCall && direction.magnitude >= minForce) {
		sleep = false;
		var vertices = mesh.vertices;
		tf = transform.worldToLocalMatrix;
		point = tf.MultiplyPoint(point);
		vec = tf.MultiplyVector(direction);
		for (var i=0;i<vertices.length;i++) {
			if ((point-vertices[i]).magnitude <= deformRadius) {
				vertices[i] += vec*(deformRadius-(point-vertices[i]).magnitude)/deformRadius*multiplier;
				if (maxDeform > 0 && (vertices[i]-permaVerts[i]).magnitude > maxDeform) {
					vertices[i] = permaVerts[i] + (vertices[i]-permaVerts[i]).normalized*maxDeform;
				}
			}
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
  		if (updateCollider) {GetComponent(MeshCollider).sharedMesh = mesh;}
	}
}
//--------------------------------------------------------------
function Update () {
	if (!sleep && bounceBackSpeed > 0) {
		sleep = true;
		var vertices = mesh.vertices;
		for (var i=0;i<vertices.length;i++) {
			vertices[i] += (permaVerts[i] - vertices[i])*(Time.deltaTime*bounceBackSpeed);
			if ((permaVerts[i]-vertices[i]).magnitude >= bounceBackSleepCap) {sleep = false;}
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
  		if (updateColliderOnBounce) {GetComponent(MeshCollider).sharedMesh = mesh;}
	}
}
//--------------------------------------------------------------
function UsedMass (collision : Collision) {
	if (collision.rigidbody) {
		if (rigidbody) {
			if (collision.rigidbody.mass > rigidbody.mass) {
				return (collision.rigidbody.mass);
			}
			else {
				return (rigidbody.mass);
			}
		}
		else {
			return (collision.rigidbody.mass);
		}
	}
	else if (rigidbody) {
		return (rigidbody.mass);
	}
	else {return (1);}
}