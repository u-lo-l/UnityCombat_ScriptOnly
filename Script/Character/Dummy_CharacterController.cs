using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ForceReceiver))]
[RequireComponent(typeof(CharacterController))]
public partial class Dummy_CharacterController : Character, ITargetable
{
	public ForceReceiver ForceReceiver;
	public CharacterController CharacterController;
	protected override void Awake()
	{
		base.Awake();
		CharacterController = GetComponent<CharacterController>();
		ForceReceiver = GetComponent<ForceReceiver>();
		tag = "Enemy";
	}
	protected override void Start()
	{
		Height = CharacterController.height;
		base.Start();
	}
	protected void Update()
	{
		CharacterController.Move(ForceReceiver.Movement * Time.deltaTime);
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
	}

	protected override void OnDamage(ActionData actionData)
	{
		print("OnDamage CC DUMMY");
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
		Ontargetdie?.Invoke();

		Die(delay);
	}

	protected override IEnumerator WaitDuringAirborne()
	{
		while (EnvironmentChecker.IsGrounded == false)
		{
			yield return null;
		}
	}

#region Custom Physics
	private readonly WaitForFixedUpdate waitForFixedUpdate = new();
	protected override IEnumerator ApplyImpulse(Vector3 force, float time)
	{
		this.ForceReceiver.AddImpulse(force, time);
		yield break;
	}
#endregion
}

#region Targeting System
public partial class Dummy_CharacterController
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
