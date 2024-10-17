using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnvironmentChecker))]
public class ForceReceiver : MonoBehaviour
{
	private EnvironmentChecker environment;
	private Vector3 gravity = Physics.gravity;
	[SerializeField] private float drag = 1f;
	private const float DragScaler = 1.2f;
	private float verticalSpeed;
	public Vector3 Impact {get; private set;} = Vector3.zero;
	public Vector3 Movement => new Vector3(0f, verticalSpeed, 0f) + Impact;
	public bool IsMovingDown => verticalSpeed < gravity.y * Time.deltaTime;

	private void Awake()
	{
		environment = GetComponent<EnvironmentChecker>();
	}
	private void Update()
	{
		if (environment.IsGrounded == true && verticalSpeed <= 0.0f)
		{
			verticalSpeed = 0;
		}
		else
		{
			verticalSpeed += gravity.y * Time.unscaledDeltaTime;
		}
	}
	private void FixedUpdate()
	{
		float interpolateValue = Time.fixedUnscaledDeltaTime * (drag + 1) * DragScaler;
		Impact = Vector3.Lerp(Impact, Vector3.zero, interpolateValue);
	}

	public void AddForce(Vector3 force)
	{
		Impact += force;
	}
	public void Jump(float jumpForce)
	{
		verticalSpeed += jumpForce;
	}

	public void AddImpulse(Vector3 force, float time)
	{
		StartCoroutine(ApplyImpluse(force, time, null));
	}

	private readonly WaitForFixedUpdate waitForFixedUpdate = new();
	private IEnumerator ApplyImpluse(Vector3 force, float timeInMilliSeconds, AnimationCurve powerCurve = null)
	{
		float elapsedTime = 0;
		while(elapsedTime < timeInMilliSeconds)
		{
			float scaler = powerCurve == null ? 1 : powerCurve.Evaluate(elapsedTime / timeInMilliSeconds);
			Impact += scaler * Time.fixedDeltaTime * force;
			elapsedTime += Time.fixedDeltaTime * 1000;
			yield return waitForFixedUpdate;
		}
	}
}
