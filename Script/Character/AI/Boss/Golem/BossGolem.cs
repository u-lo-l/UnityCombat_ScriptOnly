using System;
using UnityEngine;

public partial class BossGolem : EnemyDynamic
{
	[SerializeField] private GameObject hpBar;
	[field : SerializeField] public bool IsSleeping { private get; set; } = true;
	[SerializeField] private SequenceTrigger bossWakeTrigger;
	[SerializeField] private Sequence bossDieSequence;
	[SerializeField] public Transform BossInitialTransform;
	protected override bool IsDead => IsSleeping == false && (CharacterStatus == null || CharacterStatus.IsDead);
	protected override void Awake()
	{
		base.Awake();

		hpBar?.SetActive(false);
		collider.enabled = false;
		this.NavMeshAgent.enabled = false;
		this.Detector.enabled = false;
		this.EnvironmentChecker.enabled = false;
		this.CharacterStatus.enabled = false;
	}

	protected override void Start()
	{
		base.Start();
		destroyDelay = 10;
		movementStateMachine = new GolemMovementStateMachine(this, NavMeshAgent);
		combatStateMachine = new GolemCombatStateMachine(this, weaponHandler);
		playerDistanceOffset = 0.9f;

		SequenceManager.Instance.RegisterSequencePlayEvent(0, WakeUp);
		SequenceManager.Instance.RegisterSequencePlayEvent(1, DeadStartSequence);
		SequenceManager.Instance.RegisterSequenceFinishEvent(1, DeadFinishSequence);
		Sleep();
	}

	protected override void LateUpdate()
	{
		if(IsSleeping == true)
		{
			return ;
		}
		else if (GetTargetTransform() == null)
		{
			Sleep();
			combatStateMachine.ChangeState(combatStateMachine.HoldingState);
			movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		}
		else
		{
			base.LateUpdate();
		}
	}
	protected override void OnAnimatorMove()
	{
		float forwardFactor = Vector3.Dot(Animator.deltaPosition, EnvironmentChecker.FixedForward);

		if (Animator.GetBool("ShouldFollowPlayer") == true && GetTargetTransform() != null)
		{
			Vector3 dir = (GetTargetTransform().position - transform.position).normalized;
			dir.y = 0;
			if (dir != Vector3.zero)
			{
				print("Look Towards Target");
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 50);
				float distanceDiff = GetTargetDistance().Value - combatStateMachine.WeaponHandler.WeaponRange;
				if (distanceDiff > 1 && forwardFactor > 0)
				{
					forwardFactor += Mathf.Min(distanceDiff, 2) * Time.deltaTime;
				}
			}
		}
		if (GetTargetDistance() < playerDistanceOffset)
			forwardFactor = Mathf.Min(0, forwardFactor);
		float modifier = Animator.GetFloat("positionModifier") + 1f;
		Vector3 deltaPosition =
				forwardFactor * EnvironmentChecker.FixedForward +
				Vector3.Dot(Animator.deltaPosition, transform.right) * transform.right;
		NavMeshAgent.nextPosition += deltaPosition * modifier;
	}

	private void OnAnimatorIK(int layerIndex)
	{
		if (GetTargetTransform() == null)
		{
			return ;
		}
		float amount = Animator.GetFloat("spineModifier");
		if (GetTargetDistance() < 2)
		{
			HumanBodyBones spine1 = HumanBodyBones.Spine;
			HumanBodyBones spine2 = HumanBodyBones.Chest;

			Vector3 spine1Rotation = Animator.GetBoneTransform(spine1).localEulerAngles;
			Vector3 spine2Rotation = Animator.GetBoneTransform(spine2).localEulerAngles;

			if (amount != 0)
			{
				spine1Rotation.x += amount;
				spine2Rotation.x += amount;

				Animator.SetBoneLocalRotation(spine1, Quaternion.Euler(spine1Rotation));
				Animator.SetBoneLocalRotation(spine2, Quaternion.Euler(spine2Rotation));
			}
		}
	}

	public override void PlayLocomotion()
	{
		if (movementStateMachine == null || IsSleeping == true)
			return ;
		int LocomotionHash = AnimatorHash.BossGolem.Loco;
		if (Animator.GetLayerWeight(AnimatorHash.Enemy.ActionLayer) > 0f)
			Animator.Play(LocomotionHash, AnimatorHash.Enemy.ActionLayer);
		Animator.CrossFadeInFixedTime(LocomotionHash, 0.1f);
	}

	protected override void OnDamage(ActionData attackData)
	{
		if (CharacterStatus.IsDead == true)
		{
			this.OnDead();
			return ;
		}

		if (combatStateMachine.GetCurrentState() == AICombatState.State.Damage)
			return;
		switch(attackData.CrowdControl)
		{
			case CrowdControl.None :
				if (UnityEngine.Random.Range(0, 10) == 0)
					OnNormalDamage(attackData);
				break;
			case CrowdControl.Down :
				OnNormalDamage(attackData);
				break;
			case CrowdControl.Airborne :
				OnDownDamage(attackData);
				break;
		}
	}

	protected override void OnDead()
	{
		base.OnDead();
		bossDieSequence.PlayDirector();
	}

	public void EndAttack()
	{
		// if (combatStateMachine.GetCurrentState() != AICombatState.State.Attack)
		// 	return ;
		weaponHandler.OnDeactiveSpecificWeapon(4);
		for(int i = 0; i < 5; i++)
		{
			weaponHandler.OnColliderDisable(i);
		}
		weaponHandler.OnNextComboDisable();
	}

	#region Boss Sequence
	public event Action OnBossActivate;
	public void Sleep()
	{
		IsSleeping = true;
		this.NavMeshAgent.enabled = false;
		this.Detector.enabled = false;
		this.EnvironmentChecker.enabled = false;
		this.CharacterStatus.enabled = false;
		this.FootIKHandler.enabled = false;
		Animator.Play("SitOnChair");
		bossWakeTrigger.ReStart();
		hpBar?.SetActive(false);
		transform.SetPositionAndRotation(BossInitialTransform.position, BossInitialTransform.rotation);
	}
	public void  WakeUp()
	{
		if (IsSleeping == false)
		{
			return ;
		}
		this.NavMeshAgent.enabled = true;
		this.Detector.enabled = true;
		this.EnvironmentChecker.enabled = true;
		this.CharacterStatus.enabled = true;
		this.FootIKHandler.enabled = true;
		Animator.SetTrigger(AnimatorHash.BossGolem.WakeUpTrigger);
	}
	public void ActivateBoss() // AnimEvent
	{
		OnBossActivate?.Invoke();
		collider.enabled = true;
		hpBar?.SetActive(true);
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		weaponHandler.TryEquip((int)WeaponType.Fist);
		weaponHandler.OnDeactiveSpecificWeapon(4);

		IsSleeping = false;
	}
	private void DeadStartSequence()
	{
		Time.timeScale = 0.1f;
	}
	private void DeadFinishSequence()
	{
		Time.timeScale = 1f;
	}
#endregion
	// private void OnDrawGizmos()
	// {
	// 	Matrix4x4 oldMatrix = Gizmos.matrix;
	// 	Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 0.01f, 1));

	// 	Gizmos.color = new Color(0,0,1,0.5f);
	// 	Gizmos.DrawWireSphere(Vector3.zero, 6f);

	// 	Gizmos.color = new Color(1,0,1,0.5f);
	// 	Gizmos.DrawWireSphere(Vector3.zero, 2);
	// 	Gizmos.matrix = oldMatrix;
	// }
}