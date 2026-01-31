using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class JellyDeformer : MonoBehaviour
{
	Mesh mesh;

	Vector3[] originalVertices;
	Vector3[] displacedVertices;
	Vector3[] vertexVelocities;

	Rigidbody rb;
	Vector3 lastPosition;

	float restAverageRadius;
	Vector3 slimeCenter;

	[Header("Core Spring")]
	public float stiffness = 15f;
	public float damping = 3f;

	[Header("Idle Motion")]
	public float idleWobbleStrength = 0.05f;

	[Header("Movement Influence")]
	public float movementInfluence = 0.02f;

	[Header("Collision")]
	public float collisionPushStrength = 0.3f;
	public float collisionRadius = 0.5f;

	[Header("Ground Flattening")]
	public float groundFlattenStrength = 0.4f;
	public float groundFlattenRadius = 0.6f;

	[Header("Volume Preservation")]
	public float volumeStrength = 0.6f;

	[Header("Liquid Core")]
	public float coreFollowSpeed = 3f;
	public float corePullStrength = 0.4f;

	void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		rb = GetComponent<Rigidbody>();

		originalVertices = mesh.vertices;
		displacedVertices = new Vector3[originalVertices.Length];
		vertexVelocities = new Vector3[originalVertices.Length];

		slimeCenter = Vector3.zero;

		for (int i = 0; i < originalVertices.Length; i++)
		{
			displacedVertices[i] = originalVertices[i];
			displacedVertices[i] += Random.insideUnitSphere * 0.05f;
			slimeCenter += displacedVertices[i];
		}

		slimeCenter /= displacedVertices.Length;

		float sum = 0f;
		for (int i = 0; i < displacedVertices.Length; i++)
			sum += Vector3.Distance(slimeCenter, displacedVertices[i]);
		restAverageRadius = sum / displacedVertices.Length;

		lastPosition = transform.position;
	}

	void Update()
	{
		Vector3 worldVelocity =
			(transform.position - lastPosition) / Mathf.Max(Time.deltaTime, 0.0001f);
		lastPosition = transform.position;

		Vector3 localVelocity =
			transform.InverseTransformDirection(worldVelocity);

		UpdateSlimeCenter();

		for (int i = 0; i < displacedVertices.Length; i++)
		{
			UpdateVertex(i, localVelocity);
		}

		ApplyVolumePreservation();

		mesh.vertices = displacedVertices;
		mesh.RecalculateNormals();
	}

	void UpdateSlimeCenter()
	{
		Vector3 targetCenter = Vector3.zero;
		for (int i = 0; i < displacedVertices.Length; i++)
			targetCenter += displacedVertices[i];
		targetCenter /= displacedVertices.Length;

		slimeCenter = Vector3.Lerp(
			slimeCenter,
			targetCenter,
			coreFollowSpeed * Time.deltaTime
		);
	}

	void UpdateVertex(int i, Vector3 localVelocity)
	{
		Vector3 velocity = vertexVelocities[i];
		Vector3 displacement = displacedVertices[i] - originalVertices[i];

		// Spring back
		velocity -= displacement * stiffness * Time.deltaTime;

		// Idle liquid motion
		velocity += Random.insideUnitSphere * idleWobbleStrength * Time.deltaTime;

		// Movement lag
		velocity += localVelocity * movementInfluence;

		// Liquid core pull
		Vector3 toCore = slimeCenter - displacedVertices[i];
		velocity += toCore * corePullStrength * Time.deltaTime;

		// Damping
		velocity *= 1f - damping * Time.deltaTime;

		displacedVertices[i] += velocity * Time.deltaTime;
		vertexVelocities[i] = velocity;
	}

	void ApplyVolumePreservation()
	{
		Vector3 center = Vector3.zero;
		for (int i = 0; i < displacedVertices.Length; i++)
			center += displacedVertices[i];
		center /= displacedVertices.Length;

		float currentRadius = 0f;
		for (int i = 0; i < displacedVertices.Length; i++)
			currentRadius += Vector3.Distance(center, displacedVertices[i]);
		currentRadius /= displacedVertices.Length;

		float diff = restAverageRadius - currentRadius;

		for (int i = 0; i < displacedVertices.Length; i++)
		{
			Vector3 dir = (displacedVertices[i] - center).normalized;
			vertexVelocities[i] += dir * diff * volumeStrength * Time.deltaTime;
		}
	}

	void OnCollisionStay(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts)
		{
			Vector3 localPoint =
				transform.InverseTransformPoint(contact.point);

			ApplyCollisionDeformation(localPoint);

			if (Vector3.Dot(contact.normal, Vector3.up) > 0.6f)
				ApplyGroundFlattening(localPoint);
		}
	}

	void ApplyCollisionDeformation(Vector3 localPoint)
	{
		for (int i = 0; i < displacedVertices.Length; i++)
		{
			float dist = Vector3.Distance(displacedVertices[i], localPoint);
			if (dist < collisionRadius)
			{
				float force = (1f - dist / collisionRadius) * collisionPushStrength;
				Vector3 dir = (displacedVertices[i] - localPoint).normalized;
				vertexVelocities[i] += dir * force;
			}
		}
	}

	void ApplyGroundFlattening(Vector3 localPoint)
	{
		for (int i = 0; i < displacedVertices.Length; i++)
		{
			if (displacedVertices[i].y > localPoint.y) continue;

			float dist = Vector3.Distance(displacedVertices[i], localPoint);
			if (dist < groundFlattenRadius)
			{
				float force =
					(1f - dist / groundFlattenRadius) * groundFlattenStrength;
				vertexVelocities[i] += Vector3.up * force;
			}
		}
	}
}
