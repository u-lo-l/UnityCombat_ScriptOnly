using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Detector))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public abstract partial class EnemyBase : Character, ITargetable
{
	public NavMeshAgent NavMeshAgent { get; protected set; }
	[HideInInspector] public new Rigidbody rigidbody;
	public Detector Detector {get; protected set;}
	public event Action OnDie;

	[HideInInspector] public bool CanMove = true;
	protected int destroyDelay = 5;
	protected Coroutine colorChangingCoroutine;

	protected override void Awake()
	{
		base.Awake();
		NavMeshAgent = GetComponent<NavMeshAgent>();
		Height = NavMeshAgent.height;
		rigidbody = GetComponent<Rigidbody>();
		Detector = GetComponent<Detector>();
		tag = "Enemy";
		rigidbody.isKinematic = true;
	}

	protected override void Start()
	{
		base.Start();
	}
	protected override void OnDisable()
	{
		base.OnDisable();
	}
	public Transform GetTargetTransform()
	{
		if (Detector == null)
			return null;
		if (Detector.Target == null)
			return null;
		return Detector.Target.transform;
	}
	public float? GetTargetDistance()
	{
		Transform targetTransform = GetTargetTransform();
		if (targetTransform == null)
			return null;
		return (targetTransform.position - transform.position).magnitude;
	}

	public float GetTargetSqrDistance()
	{
		Transform targetTransform = GetTargetTransform();
		if (targetTransform == null)
			return float.PositiveInfinity;
		return (targetTransform.position - transform.position).sqrMagnitude;
	}
#region Damage Part
	protected virtual void OnDead()
	{
		NavMeshAgent.enabled = true;
		NavMeshAgent.speed = 0;
		Die(destroyDelay);
	}

	protected override void Die(int delay = 0)
	{
		OnDie?.Invoke();
		base.Die(delay);
		Ontargetdie?.Invoke();
	}

	protected IEnumerator ColorChangeOnDeath()
	{
		print("ColorChangeOnDeath");
		for (int i = 0 ; i < 100 ; i++)
			yield return new WaitForEndOfFrame();
		DisableEmissive();
	}
#endregion
#region Custom Physics
	protected Coroutine LaunchingCoroutine = null;
	private readonly WaitForFixedUpdate waitForFixedUpdate = new();
	protected override IEnumerator ApplyImpulse(Vector3 force, float time)
	{
		force.y = 0;
		this.NavMeshAgent.enabled = false;
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.AddForce(force);

		float elapsedTime = 0;
		while (elapsedTime < time)
		{
			if (IsDead == true)
				force += Vector3.up;
			rigidbody.AddForce(force, ForceMode.Force);
			elapsedTime += Time.fixedDeltaTime * 1000;
			yield return waitForFixedUpdate;
		}
		while (true)
		{
			yield return null;
			Vector3 planarVelocity = rigidbody.velocity;
			planarVelocity.y = 0;
			if (planarVelocity.sqrMagnitude < 0.0001f)
				break;
		}
		rigidbody.isKinematic = true;
		NavMeshAgent.enabled = true;
	}
	protected override IEnumerator WaitDuringAirborne()
	{
		while (EnvironmentChecker.IsGrounded == false)
		{
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
			yield return null;
		}
		rigidbody.useGravity = false;
		NavMeshAgent.enabled = true;
		rigidbody.isKinematic = true;
		yield return null;
	}

#endregion
#region CustomSkinColor
	protected void DisableEmissive()
	{
		foreach(var mat in skinMaterials)
		{
			mat.color = Color.gray;
			mat.DisableKeyword("_EMISSION");
		}
	}

	#endregion
}
#region
public partial class EnemyBase
{
	public event Action Ontargetdie;
	[SerializeField] GameObject targetIndicator;
	Transform ITargetable.GetTransform()
	{
		return lookAt == null ? null : this.lookAt.transform;
	}
	public bool CanTarget()
	{
		return IsDead == false;
	}
	public void BeTarget()
	{
		if (targetIndicator != null)
			targetIndicator.SetActive(true);
	}
	public void NotTarget()
	{
		if (targetIndicator != null)
			targetIndicator.SetActive(false);
	}
}
#endregion
