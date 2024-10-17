using UnityEngine;

[DisallowMultipleComponent]
public class EnvironmentChecker : MonoBehaviour
{
	[field : Header(" - Ground Checking")]
	[field : SerializeField] public bool ShouldGroundCheck{get; private set;} = true;
	[HideInInspector] public bool IsGrounded { get; private set; }
	[SerializeField] private Vector3 groundCheckingOffset;
	[SerializeField] private float groundCheckingRadius;
	[SerializeField] private LayerMask groundLayer;

	[field : Header(" - Normal Checking")]
	[field : SerializeField] public bool ShouldNormalCheck{get; private set;} = true;
	[SerializeField] private float slopeLimit = 45f;
	[field : SerializeField] public Vector3 FixedForward;


	[field : Header(" - StairCase Checking")]
	[SerializeField] private SphereCollider stairCheckCollider;
	[field : SerializeField] public bool IsInStairArea {get; private set;}

	private void Reset()
	{
		groundCheckingOffset = new Vector3(0f, 0.1f, 0.04f);
		groundCheckingRadius = 0.2f;
		FixedForward = transform.forward;
	}
	private void Start()
	{
		Debug.Assert(groundLayer != (LayerMask)0, $"[EnvironmentChecker] : {gameObject.name} groundLayer Needed");
	}

	private void Update()
	{
		Debug.DrawRay(transform.position, FixedForward);
	}

	private void FixedUpdate()
	{
		CheckGrounded();
		CheckForwardGround();
	}

	private void CheckGrounded()
	{
		if (ShouldGroundCheck == false)
		{
			IsGrounded = true;
			return;
		}
		IsGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckingOffset),
										 groundCheckingRadius,
										 groundLayer);
	}
	private void CheckForwardGround()
	{
		FixedForward = transform.forward;
		if (ShouldNormalCheck == false)
		{
			return;
		}

		if (IsGrounded == false)
		{
			return ;
		}

		Vector3 rayOrigin = transform.position + transform.forward * 1f + transform.up * 1f;
		if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hitPoint, 3f, groundLayer) == false)
		{
			return ;
		}
		else if (hitPoint.point.y > transform.position.y + 1)
		{
			return ;
		}
		else
		{
			FixedForward = (hitPoint.point - transform.position).normalized;
			if (Mathf.Abs(Vector3.Dot(FixedForward, Vector3.up)) > Mathf.Cos((90 - slopeLimit) * Mathf.Deg2Rad))
			{
				FixedForward = transform.forward;
			}
		}
	}

// #if UNITY_EDITOR
// 	private void OnDrawGizmosSelected()
// 	{
// 		Gizmos.color = new Color(125f, 0f, 0f, 0.1f);
// 		Gizmos.DrawSphere(transform.TransformPoint(groundCheckingOffset), groundCheckingRadius);
// 		Gizmos.color = Color.white;
// 		Gizmos.DrawLine(transform.position, transform.position + FixedForward * 2);
// 	}
// #endif
}
