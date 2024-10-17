using UnityEngine;
public abstract class PlayerActionState : PlayerCombatState
{
	protected WeaponHandler weaponHandler;
	private LayerMask enemyLayer;
	protected Animator animator;
	private Collider[] nearbyCharacterColliderBuffer = new Collider[32];

	public PlayerActionState(PlayerCombatStateMachine stateMachine) : base(stateMachine)
	{
		CurrentState = State.Action;
		weaponHandler = stateMachine.WeaponHandler;
		animator = stateMachine.Player.Animator;
		enemyLayer = GetLayerMask.GetEnemyLayerMask;
	}
	public override void Enter()
	{
		weaponHandler.CanNextCombo = false;

		RotateTowardTarget();

		if (weaponHandler.CurrentWeapon != null)
			weaponHandler.CurrentWeapon.ActionIndex = weaponHandler.ActionIndex;

		combatStateMachine.Player.LayerFadeIn(animator, AnimatorHash.Player.ActionLayer, 0f);
	}
	public override void Tick()
	{
		// Do Nothing;
	}
	public override void Exit()
	{
		if (weaponHandler.CanNextCombo == true)
			weaponHandler.IncreaseAttackIndex();
		else
			weaponHandler.ResetAttackIndex();
		weaponHandler.CanNextCombo = false;
		weaponHandler.EndAttack();
	}
	private void RotateTowardTarget()
	{
		if (combatStateMachine.Player.IsTargetingMode() == true)
		{
			Vector3 dir = combatStateMachine.Player.TargetTransform.position - combatStateMachine.Player.transform.position;
			dir.y = 0;
			combatStateMachine.Player.transform.rotation = Quaternion.LookRotation(dir);
			return ;
		}
		float range = weaponHandler.WeaponRange;
		Collider nearestCollider = SearchForNearestEnemy(range);
		if (nearestCollider == null)
		{
			return;
		}
		Vector3 direction = nearestCollider.transform.position - combatStateMachine.Player.transform.position;
		float distance = Mathf.Max(direction.magnitude - range, 0);
		direction.y = 0;
		combatStateMachine.Player.transform.rotation = Quaternion.LookRotation(direction);
		if (distance > 0f && weaponHandler.CurrentWeapon is Melee)
		{
			combatStateMachine.Player.ForceReceiver.AddImpulse(combatStateMachine.Player.EnvironmentChecker.FixedForward * distance * 120, 0.5f);
		}
		return;
	}
	private Collider SearchForNearestEnemy(float attackRange = 1.0f)
	{
		Vector3 forward = combatStateMachine.Player.transform.forward;
		Vector3 playerCenter = combatStateMachine.Player.transform.position;
		Vector3 sphereCenter = playerCenter + forward * (attackRange * 0.8f);
		Collider nearestCollider = null;

		int count = Physics.OverlapSphereNonAlloc(sphereCenter, attackRange * 0.8f, nearbyCharacterColliderBuffer, enemyLayer);
		float minDistance = float.MaxValue;
		for (int i = 0 ; i < count ; i++)
		{
			Vector3 position = nearbyCharacterColliderBuffer[i].transform.position;
			if (position.y < playerCenter.y - 0.5f || position.y > playerCenter.y + 3f)
				continue;
			float distance = (playerCenter - position).sqrMagnitude;
			if (distance < 0.25f)
				continue;
			if (Vector3.Dot(forward, (position - playerCenter).normalized) < Mathf.Cos(80))
				continue;
			if (minDistance > distance)
			{
				minDistance = distance;
				nearestCollider = nearbyCharacterColliderBuffer[i];
			}
		}
		return nearestCollider;
	}
}