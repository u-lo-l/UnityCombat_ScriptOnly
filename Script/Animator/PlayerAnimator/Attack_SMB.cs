using UnityEngine;

public class Attack_SMB : StateMachineBehaviour
{
	private Player player;
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
			player = animator.GetComponent<Player>();
		player.combatStateMachine.WeaponHandler.OnColliderDisable(-1);
	}
}
