using System.Collections;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 일섬. 진행방향의 적들을 통과하면서 강한 데미지 및 스턴
/// 부딪히면 정지하는 현상이 있어 layer를 Ghoast로 바꿔주고 다시 Player로 바꿔줘야함.
/// </summary>
[CreateAssetMenu(fileName = "Sword Strong Skill", menuName = "Spell/Player/Strong/Sword", order = 3)]
public class StrongSwordSkill : WeaponSkill
{
	private const float SlashDistance = 10f;
	private const float SlashTime = 0.1f;
	public override void Execute(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		player.gameObject.layer = GetLayerMask.GetGhoastLayer;
		player.MeshTrail.IsActive = true;
		(weapon as Sword).OnSlashHits -= OnSlashHits;
		(weapon as Sword).OnSlashHits += OnSlashHits;
		(weapon as Sword).SlashStarts(SlashTime, SlashDistance);
		PlayAttackSound(player);
	}
	public override void Execute(EnemyBase enemy, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		throw new System.NotImplementedException();
	}
	public override void Finish(Player player, Weapon weapon, ActionData attackData, Vector3? aimPosition = null)
	{
		player.MeshTrail.IsActive = false;
		player.gameObject.layer = GetLayerMask.GetPlayerLayer;
		(weapon as Sword).SlashEnds();
		(weapon as Sword).OnSlashHits -= OnSlashHits;
	}

	private void OnSlashHits(RaycastHit[] targets, Sword sword)
	{
		foreach(var victim in targets)
		{
			if (victim.collider.TryGetDamagable(out IDamagable damagable, sword.Owner, sword.gameObject, sword.AllyLayerMask) == true)
			{
				PlayHitSound(sword.Owner);
				Vector3 hitPoint = victim.collider.transform.position;
				DamageProcessor.ApplyDamage(damagable, sword, AttackData, hitPoint, Vector3.zero);
			}
		}
	}

}