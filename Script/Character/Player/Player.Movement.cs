using UnityEngine;
public partial class Player
{
	public void ResetToIdle()
	{
		movementStateMachine?.ChangeState(movementStateMachine.IdlingState);
	}
	public void PlayLocomotion()
	{
		WeaponType armedType = weaponHandler.ArmedType;
		int LocomotionHash;
		switch (armedType)
		{
			case WeaponType.Unarmed:
				LocomotionHash = AnimatorHash.Player.UnarmedLoco; break;
			case WeaponType.Fist:
				LocomotionHash = AnimatorHash.Player.FistLoco; break;
			case WeaponType.Sword:
				LocomotionHash = AnimatorHash.Player.SwordLoco; break;
			case WeaponType.Warrior:
				LocomotionHash = AnimatorHash.Player.WarriorLoco; break;
			case WeaponType.TwoHand:
				LocomotionHash = AnimatorHash.Player.TwoHandLoco; break;
			case WeaponType.GreatSword:
				LocomotionHash = AnimatorHash.Player.GreatSwordLoco; break;
			case WeaponType.Dual:
				LocomotionHash = AnimatorHash.Player.DualLoco; break;
			case WeaponType.Staff:
				LocomotionHash = AnimatorHash.Player.StaffLoco; break;
			case WeaponType.Bow:
				LocomotionHash = AnimatorHash.Player.BowLoco; break;
			default :
				return ;
		}
		if (Animator.GetCurrentAnimatorStateInfo(0).IsTag("Locomotion") == true)
		{
			float timeOffset = Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + 0.1f;
			if (timeOffset > 1f)
				timeOffset -= 1f;
			Animator.CrossFade(LocomotionHash, 0.1f, 0, timeOffset);
		}
		else
		{
			Animator.CrossFadeInFixedTime(LocomotionHash, 0.1f, AnimatorHash.Player.BaseLayer);
			Animator.Play(AnimatorHash.Player.ArmedStateHashed[(int)armedType], AnimatorHash.Player.ActionLayer);
		}
	}
	private void OnJump()
	{
		if (combatStateMachine.GetCurrentState() != PlayerCombatState.State.Hold)
			return ;
		movementStateMachine.TryJump();
	}
	private void OnRunToggle()
	{
		movementStateMachine.ShouldRun = !movementStateMachine.ShouldRun;
	}
	private void OnDoSprint()
	{
		if(movementStateMachine.ShouldSprint == false)
			movementStateMachine.ShouldSprint = true;
	}
	private void OnStopSprint()
	{
		movementStateMachine.ShouldSprint = false;
	}
	private void OnDodge()
	{
		if (combatStateMachine.GetCurrentState() != PlayerCombatState.State.Hold)
			return ;
		if (weaponHandler.ArmedType == WeaponType.Unarmed)
			return ;
		movementStateMachine.TryDodge();
	}

	public override void FastMode()
	{
		// print("JUST EVADE");
		MeshTrail.IsActive = true;
		CharacterStatus.RecoverByLostHp(0.25f);
		movementStateMachine.RecoverDodgeEnergy(25f);
		this.gameObject.layer = GetLayerMask.GetGhoastLayer;
	}
	public override void RelaseFastMode()
	{
		// print("JUST EVADE END");
		MeshTrail.IsActive = false;
		this.gameObject.layer = GetLayerMask.GetPlayerLayer;
	}
}