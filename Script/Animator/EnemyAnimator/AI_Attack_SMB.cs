using UnityEngine;

public class AI_Attack_SMB : StateMachineBehaviour
{
	private EnemyDynamic enemy;
	// private bool tryNextCombo;
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemy == null)
			enemy = animator.GetComponent<EnemyDynamic>();
		// tryNextCombo = true;
	}
	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		// if (stateInfo.normalizedTime > 0.6f && tryNextCombo == true)
		// {
		// 	if (enemy.CanNextCombo == true)
		// 	{
		// 		Debug.Log($"[Attack SMB] Combo");
		// 		enemy.combatStateMachine.WeaponHandler.OnColliderDisable(0);
		// 		if (UnityEngine.Random.Range(0, 4) == 0)
		// 		{
		// 			tryNextCombo = false;
		// 			return ;
		// 		}
		// 		enemy.TryAttack(-1);
		// 	}
		// }
	}
	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		enemy.combatStateMachine.WeaponHandler.OnColliderDisable(-1);
	}
}
