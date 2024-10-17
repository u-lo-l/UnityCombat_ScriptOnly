using UnityEngine;

public class Damage_SMB : StateMachineBehaviour
{
	private bool isCompletelyFinished;
	private Player player;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (player == null)
			player = animator.GetComponent<Player>();
		isCompletelyFinished = false;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isCompletelyFinished == false && stateInfo.normalizedTime > 0.7f)
		{
			player.ResetToHold();
			isCompletelyFinished = true;
		}
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isCompletelyFinished == false)
		{
			player.ResetToHold();
		}
	}
}

