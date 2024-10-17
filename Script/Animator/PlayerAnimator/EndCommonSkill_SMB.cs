using UnityEngine;

public class EndCommonSkill_SMB : StateMachineBehaviour
{
	private WeaponHandler weaponHandler;
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (weaponHandler == null)
			weaponHandler = animator.GetComponent<Player>().combatStateMachine.WeaponHandler;
		int armedIndex = (int)weaponHandler.ArmedType;
		int amredAnimationHash = AnimatorHash.Player.ArmedStateHashed[armedIndex];
		animator.CrossFadeInFixedTime(amredAnimationHash, 0.1f, layerIndex);
	}
}
