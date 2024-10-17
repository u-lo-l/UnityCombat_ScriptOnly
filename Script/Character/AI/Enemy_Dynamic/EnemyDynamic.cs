using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponHandler))]
[RequireComponent(typeof(Detector))]
public abstract class EnemyDynamic : EnemyBase
{
	public event System.Action<string> OnMovementStateChanged;
	public event System.Action<string> OnCombatStateChanged;
	public event System.Action<string> OnWeaponStateChanged;
	protected WeaponHandler weaponHandler;
	protected SkillHandler skillHandler;
	public AIMovementStateMachine movementStateMachine;
	public AICombatStateMachine combatStateMachine;
	public FootIKHandler FootIKHandler;
	[field : SerializeField] public float WanderRadius { get; private set; } = 10f;
	[HideInInspector] public bool hasPath;
	public bool CanNextCombo => weaponHandler.CanNextCombo;
	[field : SerializeField] public bool CanDown { get; private set; }
	[field : SerializeField] public bool CanAirborne { get; private set; }
	protected float playerDistanceOffset = 0.75f;
	public float AttackRange()
	{
		return weaponHandler.WeaponRange;
	}
	protected override void Awake()
	{
		base.Awake();
		weaponHandler = GetComponent<WeaponHandler>();
		skillHandler = GetComponent<SkillHandler>();
		FootIKHandler = GetComponent<FootIKHandler>();
	}
	protected override void Start()
	{
		base.Start();

		weaponHandler.OnWeaponChanged += this.OnWeaponChanged;
	}
	protected virtual void Update()
	{
		if (IsDead == true)
		{
			Vector3 rayOrigin = transform.position - EnvironmentChecker.FixedForward + transform.up;
			int obstacleLayerMask = GetLayerMask.GetObstacleLayerMask;
			if (Physics.Raycast(rayOrigin, - transform.up, out RaycastHit hit,  2, obstacleLayerMask))
			{
				Vector3 lookDirection = transform.position - hit.point;
				lookDirection.y = 0;
				lookDirection.Normalize();
				if (lookDirection != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(lookDirection);
			}
		}
	}
	protected virtual void LateUpdate()
	{
		if (CheckEnable() == false)
			return ;

		combatStateMachine?.Tick();
		movementStateMachine?.Tick();
		UpdateHasPath();
	}

	protected bool CheckEnable()
	{
		return NavMeshAgent.enabled == true && CharacterStatus.IsDead == false;
	}
	protected void UpdateHasPath()
	{
		hasPath = NavMeshAgent != null && NavMeshAgent.hasPath == true;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		weaponHandler.OnWeaponChanged -= this.OnWeaponChanged;
	}

	protected virtual void OnAnimatorMove()
	{
		float forwardFactor = Vector3.Dot(Animator.deltaPosition, EnvironmentChecker.FixedForward);
		if (GetTargetDistance() < playerDistanceOffset)
			forwardFactor = Mathf.Min(0, forwardFactor);
		if (GetTargetDistance() < playerDistanceOffset)
			return ;
		float modifier = Animator.GetFloat("positionModifier") + 1f;
		Vector3 deltaPosition =
				forwardFactor * EnvironmentChecker.FixedForward +
				Vector3.Dot(Animator.deltaPosition, transform.right) * transform.right;
		transform.position += deltaPosition * modifier;
	}

#region Detector
	public bool HasTargetPlayer()
	{
		return GetTargetTransform() != null;
	}
#endregion

#region MovementStateMachine
	public void InvokeOnMovementStateChanged(string state)
	{
		OnMovementStateChanged?.Invoke(state);
	}

	public virtual void ForceStop(float Duration = 2f)
	{
		if (movementStateMachine == null)
			return ;
		movementStateMachine.StoppingState.Duration = Duration;
		movementStateMachine.Enemy.CanMove = false;
		movementStateMachine.ChangeState(movementStateMachine.StoppingState);
	}

	public virtual void FaceToTarget()
	{
		if (GetTargetTransform() == null)
		{
			return;
		}
		Vector3 dir = GetTargetTransform().position - transform.position;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation(dir.normalized);
	}
	public virtual void PlayLocomotion()
	{
		if (movementStateMachine == null)
			return ;
		WeaponType armedType = weaponHandler.ArmedType;
		int LocomotionHash;
		switch (armedType)
		{
			case WeaponType.Unarmed:
				LocomotionHash = Animator.StringToHash("Unarmed_Locomotion"); break;
			default :
				LocomotionHash = Animator.StringToHash("Armed_Locomotion"); break;
		}
		if (Animator.GetLayerWeight(AnimatorHash.Enemy.ActionLayer) > 0f)
			Animator.Play(LocomotionHash, AnimatorHash.Enemy.ActionLayer);
		if (Animator.GetCurrentAnimatorStateInfo(0).IsTag("Locomotion") == true)
		{
			float timeOffset = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.1f;
			if (timeOffset > 1f)
				timeOffset -= 1f;
			Animator.CrossFade(LocomotionHash, 0.1f, AnimatorHash.Enemy.BaseLayer, timeOffset);
		}
		else
		{
			Animator.CrossFadeInFixedTime(LocomotionHash, 0.1f);
		}
	}
#endregion

#region CombatStateMachine
	public void InvokeOnCombatStateChanged(string state)
	{
		OnCombatStateChanged?.Invoke(state);
	}

	protected void OnWeaponChanged(WeaponType prevType, WeaponType newType)
	{
		OnWeaponStateChanged?.Invoke(newType.ToString());
		if (prevType == newType)
			return;
		PlayLocomotion();
	}
	public void ResetToHold(float holdDelay = 0.1f)
	{
		if (combatStateMachine.GetCurrentState() == AICombatState.State.Down)
		{
			return ;
		}
		if (combatStateMachine.GetCurrentState() == AICombatState.State.GetUp)
		{
			return ;
		}
		if (CharacterStatus.IsDead == false)
		{
			if (combatStateMachine.HoldingState.Duration < holdDelay)
				combatStateMachine.HoldingState.Duration = holdDelay;
			combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		}
	}
	public bool TryEquip()
	{
		return combatStateMachine.TryEquip();
	}
	public bool CanAttack(int attackIndex)
	{
		return combatStateMachine.CanAttack(attackIndex);
	}
	public bool TryAttack(int attackIndex = 0)
	{
		if (attackIndex == -1)
			attackIndex = weaponHandler.ActionIndex + 1;
		return combatStateMachine.TryAttack(attackIndex);;
	}
#endregion

#region Damaged
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
				OnNormalDamage(attackData);
				break;
			case CrowdControl.Down :
				OnDownDamage(attackData);
				break;
			case CrowdControl.Airborne :
				OnAirborneDamage(attackData);
				break;
		}
	}

	protected void OnNormalDamage(ActionData attackData)
	{
		if (combatStateMachine.GetCurrentState() == AICombatState.State.Down)
			combatStateMachine.ChangeState(combatStateMachine.DownedState);
		else
			combatStateMachine.ChangeState(combatStateMachine.DamagingState);
	}
	protected void OnDownDamage(ActionData attackData)
	{
		if (this.CanDown == false)
		{
			OnNormalDamage(attackData);
			return ;
		}
		combatStateMachine.ChangeState(combatStateMachine.DownedState);
	}
	protected void OnAirborneDamage(ActionData attackData)
	{
		if (this.CanAirborne == false)
		{
			OnNormalDamage(attackData);
			return ;
		}
		combatStateMachine.ChangeState(combatStateMachine.AirborneState);
	}
	protected override void OnDead()
	{
		base.OnDead();
		rigidbody.AddForce(Vector3.up);
		AddImpluse(Vector3.up, 0.5f);
		combatStateMachine?.ChangeState(combatStateMachine.DeadState);
		movementStateMachine?.ChangeState(movementStateMachine.StoppingState);
	}

#endregion

#region Foot IK
	public void EnableFootIK()
	{
		if (FootIKHandler != null)
		{
			FootIKHandler.ApplyFootIKPosition = true;
			FootIKHandler.ApplyFootIKRotation = true;
			FootIKHandler.ApplyHipAdjustment = true;
			FootIKHandler.ApplyStairIK = true;
		}
	}
	public void DisableFootIK()
	{
		if (FootIKHandler != null)
		{
			FootIKHandler.ApplyFootIKPosition = false;
			FootIKHandler.ApplyFootIKRotation = false;
			FootIKHandler.ApplyHipAdjustment = false;
			FootIKHandler.ApplyStairIK = false;
		}
	}
#endregion


}