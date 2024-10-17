using UnityEngine;

public class AI_Equip_SMB : StateMachineBehaviour
{
	private bool isCompletelyFinished;
	private EnemyDynamic enemy;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// Debug.Log("AI_Equip SMB Enter");
		if (enemy == null)
			enemy = animator.GetComponent<EnemyDynamic>();
		isCompletelyFinished = false;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isCompletelyFinished == false && stateInfo.normalizedTime > 0.7f)
		{
			// Debug.Log("AI_Equip SMB Completely Exit");
			enemy.ResetToHold();
			isCompletelyFinished = true;
		}
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (isCompletelyFinished == false)
		{
			// Debug.Log("AI_Equip SMB UnCompletely Exit");
			// enemy.ResetToHold();
		}
	}
}