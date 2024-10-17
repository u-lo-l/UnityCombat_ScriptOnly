using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerMovementInputHandler))]
[RequireComponent(typeof(ForceReceiver))]
[RequireComponent(typeof(EnvironmentChecker))]
[RequireComponent(typeof(WeaponHandler))]
public partial class Player : Character
{
	public PlayerMovementInputHandler MovementInputHandler {get; private set;}
	public PlayerCombatInputHandler CombatInputHandler {get; private set;}
	public CharacterController CharacterController {get; private set;}
	public ForceReceiver ForceReceiver {get; private set;}
	[SerializeField] private WeaponHandler weaponHandler;
	public MeshTrail MeshTrail {get; private set;}
	public PlayerMovementStateMachine movementStateMachine;
	public PlayerCombatStateMachine combatStateMachine;
	public BowHandIIKHandler bowHandIIKHandler;
	public bool IsSprinting => movementStateMachine.GetCurrentState() == PlayerMovementState.State.Sprint;
	public bool IsDodging => movementStateMachine.GetCurrentState() == PlayerMovementState.State.Dodge;
	[SerializeField] TargeterComponent targeterComponent;

	public event Action OnPlayerDeath;
#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();

		MovementInputHandler = GetComponent<PlayerMovementInputHandler>();
		SubscribeTargetEvent();
		CombatInputHandler = GetComponent<PlayerCombatInputHandler>();
		CharacterController = GetComponent<CharacterController>();
		Height = CharacterController.height;
		ForceReceiver = GetComponent<ForceReceiver>();
		weaponHandler = GetComponent<WeaponHandler>();
		bowHandIIKHandler = GetComponent<BowHandIIKHandler>();
		bowHandIIKHandler.enabled = false;

		MeshTrail = GetComponentInChildren<MeshTrail>();
		tag = "Player";
		Debug.Assert(targeterComponent != null, "TargeterComponent Not Found");
	}
	private void OnEnable()
	{
		movementStateMachine = new(this, MovementInputHandler);
		combatStateMachine = new(this, weaponHandler, CombatInputHandler);
		movementStateMachine.ChangeState(movementStateMachine.IdlingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
	}
	protected override void Start()
	{
		base.Start();
		AddListener();
		ShouldSelfDestroy = false;
		FrameStopManager.Instance.RegisterUnslowable(this);
		SwitchToFreeLookMode();
		PlayLocomotion();
	}
	private void Update()
	{
		if(CharacterStatus.IsDead == true)
		{
// #if UNITY_EDITOR
// 			EditorApplication.isPlaying = false;
// #endif
			return;
		}
		if (CharacterStatus.IsDead == false && IsDead == true)
		{
			// Somethins wrong

		}
		movementStateMachine?.Tick();
		combatStateMachine?.Tick();
	}

	private void FixedUpdate()
	{
		if(CharacterStatus.IsDead == true)
		{
			return;
		}
		movementStateMachine?.FixedTick();
		combatStateMachine?.FixedTick();
	}
	private void OnAnimatorMove()
	{
		if (Animator.deltaPosition.sqrMagnitude >= 0.000001f)
		{
			Vector3 deltaPosition =
				Vector3.Dot(Animator.deltaPosition, EnvironmentChecker.FixedForward) * EnvironmentChecker.FixedForward +
				Vector3.Dot(Animator.deltaPosition, transform.right) * transform.right;
			movementStateMachine?.DeltaMove(deltaPosition);
		}
	}
	protected override void OnDisable()
	{
		base.OnDisable();
		FrameStopManager.Instance.UnregisterUnslowable(this);
		RemoveListener();
	}

#endregion

#region Event Handler
	private void AddListener()
	{
		MovementInputHandler.OnJump -= this.OnJump;
		MovementInputHandler.OnRunToggle -= this.OnRunToggle;
		MovementInputHandler.OnDoSprint -= this.OnDoSprint;
		MovementInputHandler.OnStopSprint -= this.OnStopSprint;
		MovementInputHandler.OnDodge -= this.OnDodge;

		CombatInputHandler.OnEquip -= this.OnDoEquip;
		CombatInputHandler.OnAttack -= this.OnDoAttack;
		CombatInputHandler.OnAttackCanceled -= this.OnCancelAttack;
		CombatInputHandler.OnSpell -= this.OnDoSpell;

		weaponHandler.OnWeaponChanged -= this.OnWeaponChanged;

		MovementInputHandler.OnJump += this.OnJump;
		MovementInputHandler.OnRunToggle += this.OnRunToggle;
		MovementInputHandler.OnDoSprint += this.OnDoSprint;
		MovementInputHandler.OnStopSprint += this.OnStopSprint;
		MovementInputHandler.OnDodge += this.OnDodge;

		CombatInputHandler.OnEquip += this.OnDoEquip;
		CombatInputHandler.OnAttack += this.OnDoAttack;
		CombatInputHandler.OnAttackCanceled += this.OnCancelAttack;
		CombatInputHandler.OnSpell += this.OnDoSpell;

		weaponHandler.OnWeaponChanged += this.OnWeaponChanged;
	}
	private void RemoveListener()
	{
		MovementInputHandler.OnJump -= this.OnJump;
		MovementInputHandler.OnRunToggle -= this.OnRunToggle;
		MovementInputHandler.OnDoSprint -= this.OnDoSprint;
		MovementInputHandler.OnStopSprint -= this.OnStopSprint;
		MovementInputHandler.OnDodge -= this.OnDodge;

		CombatInputHandler.OnEquip -= this.OnDoEquip;
		CombatInputHandler.OnAttack -= this.OnDoAttack;
		CombatInputHandler.OnAttackCanceled -= this.OnCancelAttack;
		CombatInputHandler.OnSpell -= this.OnDoSpell;

		weaponHandler.OnWeaponChanged -= this.OnWeaponChanged;
	}
	#endregion
#region Custom Physics
	protected override IEnumerator ApplyImpulse(Vector3 force, float time)
	{
		this.ForceReceiver.AddImpulse(force, time);
		yield break;
	}
	protected override IEnumerator WaitDuringAirborne()
	{
		while (EnvironmentChecker.IsGrounded == false)
		{
			yield return null;
		}
	}
#endregion

	public void RespawnPlayer(Transform transform)
	{
		LayerFadeOut(Animator, 1, 0);
		LayerFadeOut(Animator, 2, 0);
		LayerFadeOut(Animator, 3, 0);
		LayerFadeOut(Animator, 4, 0);
		LayerFadeOut(Animator, 5, 0);
		Animator.Play("Empty");
		CharacterStatus.ResetCharacterStatus();
		this.transform.SetPositionAndRotation(transform.position, transform.rotation);
		collider.enabled = true;
		MovementInputHandler.enabled = true;
		CombatInputHandler.enabled = true;
		bowHandIIKHandler.enabled = false;
		SwitchToFreeLookMode();
		movementStateMachine.ChangeState(movementStateMachine.IdlingState);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		PlayLocomotion();
	}
	protected override void OnDamage(ActionData attackData)
	{
		if (combatStateMachine.GetCurrentState() == PlayerCombatState.State.Damage)
			return;

		if (CharacterStatus.IsDead == true)
		{
			Die();
		}
		else
		{
			combatStateMachine.ChangeState(combatStateMachine.DamagingState);
		}
	}
	protected override void Die(int delay = 0)
	{
		print("DIE");
		collider.enabled = false;
		MovementInputHandler.enabled = false;
		CombatInputHandler.enabled = false;
		LayerFadeOut(Animator, 1, 0);
		LayerFadeOut(Animator, 2, 0);
		LayerFadeOut(Animator, 3, 0);
		LayerFadeOut(Animator, 4, 0);
		LayerFadeOut(Animator, 5, 0);
		Animator.SetTrigger("DieTrigger");
		OnPlayerDeath?.Invoke();
	}
}

