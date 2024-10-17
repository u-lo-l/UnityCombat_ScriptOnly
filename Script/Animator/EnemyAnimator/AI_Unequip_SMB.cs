using UnityEngine;
public class AI_Unequip_SMB : StateMachineBehaviour
{
	private EnemyDynamic enemy;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// Debug.Log("Unequip SMB Enter");
		if (enemy == null)
			enemy = animator.GetComponent<EnemyDynamic>();
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// Debug.Log("Unequip SMB Exit");
		enemy.ResetToHold();
	}
}