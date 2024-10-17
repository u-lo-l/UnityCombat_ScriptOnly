
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Warp", menuName = "Spell/Warp", order = 1)]
public class Spell_Warp : ScriptableObject, ISpell
{
	private const int Trial = 10;
	public void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		if (aimPosition == null)
			return ;
		player.CharacterController.enabled = false;
		player.transform.position = aimPosition.Value;
		player.CharacterController.enabled = true;
	}

	public void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		NavMeshAgent controller = enemy.NavMeshAgent;
		Vector3 warpPosition;
		if (FindRandomWarpPosition(enemy.transform.position, out warpPosition, controller) == true)
		{
			enemy.transform.position = warpPosition;
		}
		else
		{
			Debug.LogWarning("[Enemy Warp] : Fail to Find warp position");
		}
	}

	private bool FindRandomWarpPosition(Vector3 center, out Vector3 position, NavMeshAgent navMeshAgent)
	{
		position = Vector3.zero;
		NavMeshPath path = new();
		for (int i = 0 ; i < Trial ; i++)
		{
			Vector3 random = Random.insideUnitSphere * 10f;
			if (random.sqrMagnitude < 25)
				continue;
			position =  random + center;
			if (navMeshAgent != null)
			{
				if (navMeshAgent.CalculatePath(position, path) == true)
				{
					return true;
				}
			}
			else
			{
				return true;
			}
		}
		return false;
	}
}
