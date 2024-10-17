using TMPro;
using UnityEngine;

public class AIStatusUI : MonoBehaviour
{
	private EnemyDynamic enemy;
	[SerializeField] private TMP_Text movementState;
	[SerializeField] private TMP_Text combatState;
	[SerializeField] private TMP_Text weaponState;
	private void Awake()
	{
		bool condition = movementState != null || combatState != null || weaponState != null;
		
		Debug.Assert(condition, "[AIStatus] Text not found");
		enemy = transform.root.GetComponent<EnemyDynamic>();
		Debug.Assert(enemy != null, "[AIStatus] enemy not found");
		movementState.text = "Movement : ";
		combatState.text = "Combat : ";
		enemy.OnCombatStateChanged += (state) => {
			combatState.text = $"Combat : {state}";
		};
		enemy.OnMovementStateChanged += (state) => {
			movementState.text = $"Movement : {state}";
		};
		enemy.OnWeaponStateChanged += (state) => {
			weaponState.text = $"Weapon : {state}";
		};
	}
}
