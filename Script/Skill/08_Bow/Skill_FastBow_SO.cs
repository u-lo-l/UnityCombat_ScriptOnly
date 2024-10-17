using UnityEngine;
using static GetLayerMask;

[CreateAssetMenu(fileName = "Bow Fast Skill", menuName = "Spell/Player/Fast/Bow", order = 14)]
public class FastBowSkill : WeaponSkill_Throwing
{
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Bow] Fast Skill Executed by Player");
		player.SwitchToFreeLookMode();
		SetMuzzle(player, out Vector3 position, out Quaternion orientation);
		WindTornado windTornado = CreateProjectile<WindTornado>(muzzlePosition : position,
												  muzzleRotation : orientation,
												  owner : player,
												  weapon : weapon,
												  targetLayerMask : GetEnemyLayerMask);
		SetProjectildAudio(windTornado);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Bow] Fast Skill Executed by Player");
		// throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Bow] Fast Skill Finishedr");
		// throw new System.NotImplementedException();
	}
	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		Vector3 center = holder.transform.position + holder.transform.forward + holder.transform.up;
		rotation = holder.transform.rotation;
		if (Physics.Raycast(center, Vector3.up, out RaycastHit hitInfo, 2, GetObstacleLayerMask) == true)
		{
			position = hitInfo.point;
		}
		else
		{
			position = holder.transform.position + holder.transform.forward;
		}
	}
}