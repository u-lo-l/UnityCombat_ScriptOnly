using UnityEngine;

public class ReturnToStop_SMB : StateMachineBehaviour
{
	private Player player;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
		{
			player = animator.GetComponent<Player>();
		}
		player.ResetToIdle();
	}
}
