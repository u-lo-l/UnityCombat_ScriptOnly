using UnityEngine;

public class Golem_ReturnToHold_SMB : StateMachineBehaviour
{
	private BossGolem enemy;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemy == null)
			enemy = animator.GetComponent<BossGolem>();
		animator.SetTrigger("CancelTrigger");
		enemy.EndAttack();
		enemy.ResetToHold();
	}
}
