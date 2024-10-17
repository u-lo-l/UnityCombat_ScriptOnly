using UnityEngine;
using static GetLayerMask;

/// <summary>
/// 커다란 칼을 소환하여 휘두르며 양 옆으로 충격파 발생
/// </summary>
[CreateAssetMenu(fileName = "GreatSword Strong Skill", menuName = "Spell/Player/Strong/GreatSword", order = 9)]
public class StrongGreatSwordSkill : WeaponSkill
{
	[SerializeField, Range(0.01f, 2f)] private float enlargementTime = 1f;
	[SerializeField] private AnimationCurve enlargementCurve;
	[SerializeField, Range(0.01f, 2f)] private float recoverTime = 1f;
	[SerializeField] private AnimationCurve recoverCurve;
	[SerializeField] private AudioClip FinishSound;
	public LayerMask AllyLayerMask {private get; set;}
	private const float Scaler = 5f;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		GreatSword greatSword = weapon as GreatSword;

		greatSword.EnlargeGreatSword(enlargementTime, Scaler, enlargementCurve);

		base.PlayAttackSound(player);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		GreatSword greatSword = weapon as GreatSword;
		greatSword.ResizeGreatSword(recoverTime, recoverCurve);

		PlayClip(player, FinishSound);

		if(greatSword.TryGetCapsulcollider(out CapsuleCollider collider) == true)
		{
			ApplyAftereffect(player, greatSword, collider);
		}
	}
	private void ApplyAftereffect(Character owner, Weapon weapon, CapsuleCollider collider)
	{
		Vector3 worldCenter = collider.transform.TransformPoint(collider.center);
		Vector3 direction = collider.transform.rotation * Vector3.up;
		float radius = collider.radius * Scaler;
		float length = collider.height * Scaler - radius * 2;

		Vector3 startPoint = worldCenter - direction * length / 2;

		RaycastHit[] targets = Physics.SphereCastAll(startPoint, radius * 2, direction, length, GetEnemyLayer);

		foreach(var victim in targets)
		{
			Debug.Log(victim.collider.gameObject.name);
			if (victim.collider.TryGetDamagable(out IDamagable damagable, owner, weapon.gameObject, AllyLayerMask) == true)
			{
				PlayHitSound(owner);
				DamageProcessor.ApplyDamage(damagable, null, AttackData, Vector3.zero, Vector3.zero);
			}
		}
	}
}