using UnityEngine;

public class Unequip_SMB : StateMachineBehaviour
{
	private Player player;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// Debug.Log("Unequip SMB Enter");
		if (player == null)
			player = animator.GetComponent<Player>();
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// Debug.Log("Unequip SMB Exit");
		player.ResetToHold();
	}
}
