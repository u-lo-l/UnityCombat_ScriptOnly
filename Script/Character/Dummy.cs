using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public partial class Dummy : Character, ITargetable
{
	private new Rigidbody rigidbody;
	private CapsuleCollider capsuleCollider;
	protected override void Awake()
	{
		base.Awake();
		rigidbody = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		tag = "Enemy";
	}
	protected override void Start()
	{
		Height = capsuleCollider.height;
		base.Start();
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnDamage(ActionData actionData)
	{
		if (CharacterStatus.IsDead == true)
		{
			this.OnDead(5);
			return ;
		}
		OnNormalDamage(actionData);
	}

	protected void OnNormalDamage(ActionData attackData)
	{
		Animator.Play("pushed");
	}

	protected void OnDead(int delay)
	{
		Animator.Play("died");
		rigidbody.isKinematic = false;
		rigidbody.useGravity = false;
		rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		capsuleCollider.enabled = false;
		Ontargetdie?.Invoke();

		Die(delay);
	}

	protected override IEnumerator WaitDuringAirborne()
	{
		rigidbody.isKinematic = false;
		while (EnvironmentChecker.IsGrounded == false)
		{
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
			yield return null;
		}
		rigidbody.isKinematic = true;
	}

#region Custom Physics
	private readonly WaitForFixedUpdate waitForFixedUpdate = new();
	protected override IEnumerator ApplyImpulse(Vector3 force, float time)
	{
		print($"Apply Impulse : {force} for {time}ms");
		rigidbody.isKinematic = false;
		Animator.applyRootMotion = false;
		float elapsedTime = 0;
		while (elapsedTime < time)
		{
			rigidbody.AddForce(force);
			elapsedTime += Time.fixedDeltaTime * 1000;
			yield return waitForFixedUpdate;
		}
		while (true)
		{
			yield return null;
			if (rigidbody.velocity.sqrMagnitude < 0.0001f)
				break;
		}
		rigidbody.isKinematic = true;
		Animator.applyRootMotion = true;
	}
#endregion
}

#region IDamagable
public partial class Dummy
{

}
#endregion


#region Targeting System
public partial class Dummy
{
	public event Action Ontargetdie;
	[SerializeField] GameObject targetIndicator;
	Transform ITargetable.GetTransform()
	{
		return lookAt == null ? null : this.lookAt.transform;
	}
	bool ITargetable.CanTarget()
	{
		return IsDead == false;
	}
	void ITargetable.BeTarget()
	{
		if (targetIndicator != null)
			targetIndicator.SetActive(true);
	}
	void ITargetable.NotTarget()
	{
		if (targetIndicator != null)
			targetIndicator.SetActive(false);
	}
}
#endregion
