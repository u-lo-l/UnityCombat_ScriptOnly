using UnityEngine;
public class GolemFlyingKneeState : GolemActionState
{
	public GolemFlyingKneeState(AICombatStateMachine stateMachine) : base(stateMachine)
	{
		triggerHash = Animator.StringToHash("FlyingKneeTrigger");
	}
	public override void Enter()
	{
		base.Enter();
		isTriggered = false;
		isRotated = false;
		combatStateMachine.WeaponHandler.CurrentWeapon.AdditionalIndex = 8;
	}

	public override void Tick()
	{
		if (isRotated == false)
		{
			movementStateMachine.RotateToTarget(0.1f);
			isRotated = true;
			return ;
		}
		if (movementStateMachine.IsRotating == true)
		{
			return ;
		}
		if (isTriggered == false)
		{
			animator.SetTrigger(triggerHash);
			animator.SetBool("ShouldFollowPlayer", true);
			isTriggered = true;
			return ;
		}
	}
	public override void Exit()
	{
		base.Exit();
		animator.SetBool("ShouldFollowPlayer", false);
		combatStateMachine.HoldingState.Duration = Random.Range(0f, 0.1f);
	}
}