using UnityEngine;
public class AIStrafeState : AIMovementState
{
	private float strafeTime = 5f;
	private float strafeDirection = 0;
	private float strafeRadius = 0;
	public AIStrafeState(AIMovementStateMachine stateMachine)
	 : base(stateMachine)
	{
		CurrentState = State.Strafe;
	}

	public override void Enter()
	{
		Debug.Log("Enter To Strafe State");
		base.Enter();
		controller.updateRotation = false;
		controller.isStopped = false;
		controller.speed = movementStateMachine.Enemy.CharacterStatus.WalkSpeed + Random.Range(-0.1f, 0.1f);
		controller.stoppingDistance = 0;
		strafeTime = 4f + Random.Range(-1f, 1f);
		strafeDirection = Random.Range(0,2) == 0 ? -1f : 1f;
		float far = movementStateMachine.Enemy.Detector.detectingNearDistance;
		float near = movementStateMachine.Enemy.combatStateMachine.WeaponHandler.WeaponRange + 1;
		strafeRadius = Random.Range(near, far);
	}

	public override void Tick()
	{
		if (strafeTime < 0 || movementStateMachine.Enemy.HasTargetPlayer() == false)
		{
			SwitchToWaitState();
		}
		else
		{
			Transform targetTransform = movementStateMachine.Enemy.GetTargetTransform();
			Transform transform = movementStateMachine.Enemy.transform;

			Vector3 lookDirection = targetTransform.position - transform.position;
			lookDirection.y = 0;
			lookDirection.Normalize();
			if (lookDirection != Vector3.zero)
			{
				Quaternion rotation = Quaternion.LookRotation(lookDirection);
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);

				Vector3 displacement = (movementStateMachine.Enemy.GetTargetDistance().Value - strafeRadius) * transform.forward;
				Vector3 moveDirection = strafeDirection * Vector3.Cross(lookDirection, transform.up) + displacement;
				moveDirection.Normalize();
				controller.SetDestination(transform.position + moveDirection);

				Debug.DrawLine(transform.position, transform.position + moveDirection);
			}

		}
		strafeTime -= Time.deltaTime;
		base.Tick();
	}
	protected override float GetTaretSpeed()
	{
		movementStateMachine.Enemy.CharacterStatus.SpeedModifier = Random.Range(0.8f, 1.2f);
		return movementStateMachine.Enemy.CharacterStatus.WalkSpeed;
	}

	public override void Exit()
	{
		controller.updateRotation = true;
		controller.stoppingDistance = 1.25f;
	}
}