using System;
using UnityEngine;
using static PlayerMovementState;
public partial class PlayerMovementStateMachine : StateMachine
{
#region Caching States
	public event Action OnDodgeSucceed;
	// public event Action OnDodgeAble;
	public Player player {get; private set;}
	public PlayerIdlingState IdlingState {get; private set;}
	public PlayerWalkingState WalkingState {get; private set;}
	public PlayerRunningState RunningState {get; private set;}
	public PlayerSprintingState SprintingState {get; private set;}
	public PlayerJumpingState JumpingState {get; private set;}
	public PlayerDodgingState DodgingState {get; private set;}
	public PlayerAirborneState AirborneState {get; private set;}
	public PlayerLandingState LandingState {get; private set;}
	public PlayerMovementInputHandler MovementInputHandler {get; private set;}
	public PlayerMovementStateMachine(Player player, PlayerMovementInputHandler inputHandler)
	{
		this.player = player;
		this.MovementInputHandler = inputHandler;
		IdlingState = new(this);
		WalkingState = new(this);
		RunningState = new(this);
		SprintingState = new(this);
		JumpingState = new(this);
		DodgingState = new(this);
		AirborneState = new(this);
		LandingState = new(this);
	}
#endregion
	private float currentSpeed = 0;
	public ref float CurrentSpeed_ref => ref currentSpeed;
	public bool CanMove = true;
	public bool ShouldRun = true;
	public bool ShouldSprint = false;
	public readonly float DodgeRecoverTime = 2f;
	private const float MaxDodgeEnergy = 100f;
	public float RemainDodgeEnergy { get; private set; } = MaxDodgeEnergy;
	private const float dodgeEnergyCost = 50f;
	public int JumpCount { get; private set; } = 2;
	public sealed override void Tick()
	{
		UpdateDodgeEnergy();
		base.Tick();
	}
	public void DeltaMove(Vector3 deltaPosition)
	{
		if (player.CharacterController.enabled == false)
			return ;
		player.CharacterController.Move(deltaPosition);
	}
	public void Move(Vector3 velocity)
	{
		if (player.CharacterController.enabled == false)
			return ;
		Move(velocity, Vector3.zero);
	}
	public void Move(Vector3 velocity, Vector3 impact)
	{
		if (player.CharacterController.enabled == false)
			return ;
		player.ForceReceiver.AddForce(impact);
		Vector3 finalVelocity = velocity + player.ForceReceiver.Movement;

		player.CharacterController.Move(finalVelocity * Time.unscaledDeltaTime);
	}

	public State GetCurrentState()
	{
		PlayerMovementState movementState = currentState as PlayerMovementState;
		return movementState.CurrentState;
	}
	public void TryJump()
	{
		if (CanJump() == false)
		{
			return;
		}
		ChangeState(JumpingState);
	}
	public void TryDodge()
	{
		if (CanDodge() == false)
			return ;

		RemainDodgeEnergy -= dodgeEnergyCost;
		OnDodgeSucceed?.Invoke();
		ChangeState(DodgingState);
	}
	public void ChangeState(PlayerMovementState newState)
	{
		base.ChangeState(newState);
	}
	public bool CanJump()
	{
		if (JumpCount == 0)
			return false;
		if (player.EnvironmentChecker.IsGrounded == false)
			return false;
		State movementState = GetCurrentState();
		if (movementState == State.Jump)
			return false;
		if (movementState == State.Air)
			return false;
		if (movementState == State.Dodge)
			return false;
		return true;
	}
	public bool CanDodge()
	{
		if (RemainDodgeEnergy < dodgeEnergyCost)
			return false;
		if (player.EnvironmentChecker.IsGrounded == false)
			return false;
		State movementState = GetCurrentState();
		if (movementState == State.Jump)
			return false;
		if (movementState == State.Air)
			return false;
		if (movementState == State.Sprint)
			return false;
		if (movementState == State.Dodge)
			return false;
		return true;
	}
	private void UpdateDodgeEnergy()
	{
		if (RemainDodgeEnergy >= MaxDodgeEnergy)
		{
			return ;
		}
		else
		{
			RemainDodgeEnergy += dodgeEnergyCost / DodgeRecoverTime * Time.deltaTime;
		}
	}

	public void RecoverDodgeEnergy(float amount = 25f)
	{
		RemainDodgeEnergy = Mathf.Min(RemainDodgeEnergy + amount, MaxDodgeEnergy);
	}
}

#region FreeLook or Targeting Mode
public partial class PlayerMovementStateMachine
{
	public enum MovementMode { FreeLook, Targeting };
	public MovementMode MoveMode = MovementMode.FreeLook;
	public void SwitchToFreeLookMode() => MoveMode = MovementMode.FreeLook;
	public void SwitchToTargetingMode() => MoveMode = MovementMode.Targeting;
	public bool IsFreeLookMode() => MoveMode == MovementMode.FreeLook;
	public bool IsTargetingMode() => MoveMode == MovementMode.Targeting;
}
#endregion