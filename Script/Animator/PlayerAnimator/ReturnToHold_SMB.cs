using UnityEngine;

public class ReturnToHold_SMB : StateMachineBehaviour
{
	private Player player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animator.GetLayerWeight(layerIndex) < 0.5f)
		{
			return ;
		}
		if (player == null)
		{
			player = animator.GetComponent<Player>();
		}
		player.combatStateMachine.WeaponHandler.EndAttack();
		player.ResetToHold();
	}
}
