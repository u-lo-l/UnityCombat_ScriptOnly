using UnityEngine;

public class AI_ReturnToHold_SMB : StateMachineBehaviour
{
	private EnemyDynamic enemy;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemy == null)
			enemy = animator.GetComponent<EnemyDynamic>();
		if (enemy != null)
			enemy.ResetToHold();
	}
}
