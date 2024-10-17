using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Staff Fast Skill", menuName = "Spell/Player/Fast/Staff", order = 12)]
public class FastStaffSkill : WeaponSkill_Throwing
{
	[SerializeField, Range(20, 200)] float startingSpeed;
	private const float maxDistance = 10;
	private const float ReadyTime = 0.3f;
	private readonly WaitForSeconds waitForArrows = new(ReadyTime);
	private const float ShootInterval = 0.015f;
	private readonly WaitForSeconds waitForShoot = new(ShootInterval);
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Staff] Fast Skill Executed by Player");
		player.StartCoroutine(ShootArrows(player, weapon, GetLayerMask.GetEnemyLayerMask));
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Staff] Fast Skill Executed by Player");
		// throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		Debug.Log("[Staff] Fast Skill Finishedr");
		// throw new System.NotImplementedException();
	}

	private IEnumerator ShootArrows(Character owner, Weapon weapon, LayerMask targetLayerMask)
	{
		Vector3 targetPosition = GetTargetPosition(owner, targetLayerMask);
		MagicArrow[] magicArrows = ReadyArrows(owner, targetPosition, weapon, targetLayerMask, 60);
		yield return waitForArrows;
		foreach(MagicArrow arrow in magicArrows)
		{
			arrow.Shoot();
			PlayAttackSound(owner);
			yield return waitForShoot;
		}
	}

	private Vector3 GetTargetPosition(Character owner, LayerMask targetLayerMask)
	{
		float radius = owner.Height;
		Vector3 center = owner.transform.position + 0.5f * owner.Height * Vector3.up;
		Vector3 direction = owner.EnvironmentChecker.FixedForward;
		if (Physics.SphereCast(center, radius, direction, out RaycastHit hit, maxDistance, targetLayerMask) == true)
		{
			return hit.collider.ClosestPoint(center);
		}
		else
		{
			return center + maxDistance * direction;
		}
	}
	private MagicArrow[] ReadyArrows(Character owner, Vector3 targetPosition, Weapon weapon, LayerMask targetLayerMask, float angle)
	{
		MagicArrow[] magicArrows = new MagicArrow[projectileCount];
		angle *= 0.5f;
		float angleStep = projectileCount <= 1 ? 0 : angle * 2 / (projectileCount - 1);
		for (int i = 0 ; i < projectileCount ; i++, angle -= angleStep)
		{
			Vector3 muzzlePosition = owner.transform.rotation * Quaternion.Euler(0, 0, angle) * Vector3.up;
			muzzlePosition *= owner.Height + 0.5f;
			muzzlePosition += owner.transform.position;

			Vector3 direction = targetPosition - muzzlePosition;
			Quaternion muzzleRotation = Quaternion.LookRotation(direction);
			magicArrows[i] = CreateProjectile<MagicArrow>(muzzlePosition, muzzleRotation, owner, weapon, targetLayerMask);
		}
		return magicArrows;
	}

	protected override void SetMuzzle(Character holder, out Vector3 position, out Quaternion rotation)
	{
		position = Vector3.zero;
		rotation = Quaternion.identity;
	}
}