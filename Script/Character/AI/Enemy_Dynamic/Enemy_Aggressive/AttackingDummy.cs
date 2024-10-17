using System.Collections;
using UnityEngine;

public class AttackingDummy : EnemyDynamic
{
	protected override void Start()
	{
		base.Start();
		movementStateMachine = new AggressiveAIMovementStateMachine(this, NavMeshAgent);
		combatStateMachine = new AggressiveAICombatStateMachine(this, weaponHandler);
		combatStateMachine.ChangeState(combatStateMachine.HoldingState);
		StartCoroutine(RandomIdle());
	}

	private float currIdleValue = 0;
	private IEnumerator RandomIdle()
	{
		float duration;
		float elapsedTime;

		do
		{
			duration = Random.Range(3f, 6f);
			float randomIdle = Random.Range(0f,1f);
			currIdleValue = Mathf.Lerp(currIdleValue, randomIdle, Time.deltaTime * 50);
			Animator.SetFloat("RandomIdle", currIdleValue);
			elapsedTime = 0;
			while (elapsedTime < duration)
			{
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		} while (IsDead == false);
		print($"{name} : Is DEAD");
		yield break;
	}

	protected override void LateUpdate()
	{
		if (CheckEnable() == false)
			return ;

		combatStateMachine?.Tick();
		movementStateMachine?.Tick();
		UpdateHasPath();
	}

	public override void ForceStop(float Duration = 2f)
	{
		return ;
	}

	public override void FaceToTarget()
	{
		if (GetTargetTransform() == null)
		{
			return;
		}
		Vector3 dir = GetTargetTransform().position - transform.position;
		dir.y = 0;
		transform.rotation = Quaternion.LookRotation(dir.normalized);
	}
	public override void PlayLocomotion()
	{
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
}

