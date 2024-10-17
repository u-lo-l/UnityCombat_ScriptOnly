using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[DisallowMultipleComponent]
public class Detector : MonoBehaviour
{
	[SerializeField] private float startDelay = 0;
	private bool isActive = false;
	[SerializeField] private Transform headTransform;
	[field : SerializeField] public float detectingFarDistance { get ; private set; }
	[field : SerializeField] public float detectingNearDistance { get ; private set; }
	[SerializeField] private float detectingAngleInDegrees;
	[SerializeField] private float lostTime;
	private RaycastHit raycastHit;
	[field : SerializeField] public GameObject Target {get; private set;} = null;
	[SerializeField] private LayerMask targetLayerMask;
	[SerializeField] private LayerMask allyLayerMask;
	// Player가 여럿일 경우도 있겠지만, 이후에 Player의 아군이 생길 경우, Dictionary<GameObject, float>을 이용해서 감지된 객체들을 관리할 수 있다.
	private const int BufferSize = 3;
	private Collider[] overlappedCollidersBuffer;
	[SerializeField] private float timeSinceLostTarget = 0;
	public event Action OnDetectPlayer;
	public event Action OnLostPlayer;
	private void Awake()
	{
		// Reset();
		overlappedCollidersBuffer = new Collider[BufferSize];
		Debug.Assert(headTransform != null, "[Detector] Head Not Found");
		Debug.Assert(targetLayerMask != 0, "[Detector] targetLayerMask Not Found");
		Debug.Assert(allyLayerMask != 0, "[Detector] allyLayerMask Not Found");
	}
	private IEnumerator Start()
	{
		float time = 0;
		isActive = false;
		while (time < startDelay)
		{
			time += Time.deltaTime;
			yield return null;
		}
		isActive = true;
	}

	private void Reset()
	{
		detectingFarDistance = 10;
		detectingNearDistance = 5;
		detectingAngleInDegrees = 240;
		lostTime = 1;
		overlappedCollidersBuffer = new Collider[BufferSize];
	}
	private void Update()
	{
		if (isActive)
			RemoveLostTarget();
	}

	private void FixedUpdate()
	{
		if (isActive)
			DetectTarget();
	}

	private void DetectTarget()
	{
		int detectedSize;
		if (Target == null)
		{
			detectedSize = Physics.OverlapSphereNonAlloc(transform.position, detectingFarDistance, overlappedCollidersBuffer, targetLayerMask);
		}
		else
		{
			detectedSize = Physics.OverlapSphereNonAlloc(transform.position, detectingFarDistance, overlappedCollidersBuffer, 1 << Target.layer);
		}

		if (detectedSize == 0)
		{
			if (Target != null)
				timeSinceLostTarget += Time.deltaTime;
			return;
		}

		Collider detected = overlappedCollidersBuffer[0];
		Vector3 towardTarget = detected.transform.position - transform.position;


		if (towardTarget.magnitude > detectingNearDistance)
		{
			if (Vector3.Dot(this.transform.forward, towardTarget.normalized) < Mathf.Cos(detectingAngleInDegrees * Mathf.Deg2Rad / 2))
			{
				if (Target != null)
					timeSinceLostTarget += Time.deltaTime;
				return;
			}
		}

		Vector3 origin = headTransform.position;
		Vector3 direction = towardTarget.normalized;
		float maxDistance = towardTarget.magnitude;
		if (Physics.Raycast(origin, direction, out raycastHit, maxDistance, GetLayerMask.GetObstacleLayerMask) == true)
		{
			if ((raycastHit.point - origin).sqrMagnitude < maxDistance * maxDistance)
			{
				if (Target != null)
					timeSinceLostTarget += Time.deltaTime;
				return ;
			}
		}

		if (Target == null)
		{
			Target = detected.gameObject;
			print("New Player Found");
			OnDetectPlayer?.Invoke();
		}
		timeSinceLostTarget = 0;
	}
	private void RemoveLostTarget()
	{
		if (Target == null)
		{
			return ;
		}
		if (timeSinceLostTarget >= lostTime)
		{
			Target = null;
			print("Player Lost");
			OnLostPlayer?.Invoke();
			timeSinceLostTarget = 0;
		}
	}

	private void OnDrawGizmos()
	{
		Matrix4x4 oldMatrix = Gizmos.matrix;
		Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 0.01f, 1));

		Gizmos.color = new Color(0,1,0,0.1f);
		Gizmos.DrawSphere(Vector3.zero, detectingFarDistance);
		Gizmos.color = new Color(1,1,1,0.1f);
		Gizmos.DrawWireSphere(Vector3.zero, detectingFarDistance);

		Gizmos.color = new Color(1,0,0,0.1f);
		Gizmos.DrawSphere(Vector3.zero, detectingNearDistance);
		Gizmos.color = new Color(1,1,1,0.1f);
		Gizmos.DrawWireSphere(Vector3.zero, detectingNearDistance);
		Gizmos.matrix = oldMatrix;
	}
}
