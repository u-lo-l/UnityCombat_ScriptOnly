using UnityEngine;
public interface ISpell
{
	public void Execute(Player player, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null);
	public void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData = null, Vector3? aimPosition = null);
}