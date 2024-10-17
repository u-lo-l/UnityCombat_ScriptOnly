using UnityEngine;

public class AIStopState : AIMovementState
{
	public float Duration = 0f;

	public AIStopState(AIMovementStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Stop;
	}

	public override void Enter()
	{
		base.Enter();
		Duration = Mathf.Max(0.5f, Duration);
		if (controller.enabled == true)
		{
			controller.speed = 0;
			controller.isStopped = true;
			controller.updateRotation = false;
			controller.stoppingDistance = controller.speed / 2;
		}
	}
	public override void FixedTick()
	{

	}
	public override void Tick()
	{
		base.Tick();
		if (Duration > 0)
		{
			// Debug.Log($"{movementStateMachine.Enemy.name} | [AIStopState] : Wait for {Duration}secs");
			Duration -= Time.deltaTime;
			return;
		}
		// Force Stop
		if (movementStateMachine.Enemy.CanMove == false)
		{
			// Debug.Log($"{movementStateMachine.Enemy.name} | [AIStopState] : Cant Move");
			return;
		}

		if (movementStateMachine.Enemy.HasTargetPlayer() == false)
		{
			CasePlayerNotFound();
		}
		else
		{
			CasePlayerFound();
		}

	}
	public override void Exit()
	{
		Duration = 0;
		movementStateMachine.Enemy.CanMove = true;
		if (controller != null)
			controller.isStopped = false;
	}

	protected override float GetTaretSpeed()
	{
		movementStateMachine.Enemy.CharacterStatus.SpeedModifier = 1;
		return 0;
	}

	private void CasePlayerNotFound()
	{
		// Debug.Log($"{movementStateMachine.Enemy.name} | [AIStopState] : Switch To Wander");
		SwitchToWanderState();
	}

	/// <summary>
	/// Player가 감지되면 CombatStateMachine에서는 알아서 EquipState로 들어갈 것이다.<br/>
	/// weaponHandler가 weapon을 가지고 있고, AttackDelay가 0이라면 공격이 가능하다.
	/// 공격이 가능하다면 Player
	/// </summary>
	protected virtual void CasePlayerFound()
	{
		if (movementStateMachine.Enemy.CanAttack(0) == true)
		{
			// 공격 범위 밖에 있다면 Follow
			// 공격범위 내에 있다면 Do Nothing
			//	-> Follow에서 Stop으로 State가 전환 된 경우이다.
			// (CombatStateMachine에 의해서 Attack이 수행되면 CanAttack()이 False를 반환할 것이다.)

			float sqrDistance = movementStateMachine.Enemy.GetTargetSqrDistance();
			if (sqrDistance == float.PositiveInfinity)
			{
				SwitchToWanderState();
			}

			float range = movementStateMachine.Enemy.AttackRange();
			if ( sqrDistance < range * range)
			{
				return ; // 여기선 멈춰서 Attack
			}
			else
			{
				SwitchToFollowState();
			}
		}
		else
		{
			if (movementStateMachine.Enemy.GetTargetDistance() < movementStateMachine.Enemy.Detector.detectingNearDistance * 0.7f)
			{
				SwitchToStepBackState();
			}
			else
			{
				SwitchToStrafeState();
			}
		}
	}
};