using UnityEngine;

public class Boss_Rotate_SMB : StateMachineBehaviour
{
	private bool isCompletelyFinished;
	private EnemyBoss boss;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (boss == null)
			boss = animator.GetComponent<EnemyBoss>();
		isCompletelyFinished = false;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isCompletelyFinished == false && stateInfo.normalizedTime > 0.7f)
		{
			boss.ResetToStop();
			isCompletelyFinished = true;
		}
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isCompletelyFinished == false)
		{
			boss.ResetToStop();
		}
	}
}
