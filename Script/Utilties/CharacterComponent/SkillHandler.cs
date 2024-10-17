using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(WeaponHandler))]
public class SkillHandler : MonoBehaviour
{
	public Transform SkillFactory;
	[SerializeField] private Character holder;
	[SerializeField] public ActionData CommomSkill;
	private bool isOwnedByPlayer;
	private readonly Dictionary<WeaponType, WeaponSkill> fastSkillTable = new();
	[field : SerializeField] public float FastSkillGauge { get; private set; } = 0;
	private readonly Dictionary<WeaponType, WeaponSkill> strongSkillTable = new();
	public float StrongSkillGauge { get; private set; } = 0;

	public event Action OnFastGuageChanged;
	public event Action OnStrongGuageChanged;
#region MonoBehaviour
	private void Awake()
	{
		if (transform.TryFindByName("SkillFactory", out SkillFactory) == false)
		{
			SkillFactory = new GameObject("SkillFactory").transform;
			SkillFactory.SetParent(transform);
		}
		if (TryGetComponent<Character>(out holder) == false)
		{
			Debug.Assert(false, "[Skill Component] holder Needed");
		}
		isOwnedByPlayer = holder is Player;
	}
#endregion
	public void RegisterWeaponSkill(WeaponType weaponType, WeaponSkill fastSkill, WeaponSkill strongSkill)
	{
		// Debug.Log($"[Skill Handler] skill registed : {weaponType} \\<{fastSkill}> \\<{strongSkill}>");
		fastSkillTable[weaponType] = fastSkill;
		strongSkillTable[weaponType] = strongSkill;
	}
	public void StackFastGauge(float amount)
	{
		FastSkillGauge = Mathf.Min(FastSkillGauge + amount, 1);
		OnFastGuageChanged?.Invoke();
	}
	public void StackStrongGauge(float amount)
	{
		StrongSkillGauge = Mathf.Min(StrongSkillGauge + amount, 1);
		OnStrongGuageChanged?.Invoke();
	}
	public bool CanFastSkill(WeaponType weaponType)
	{
		if (weaponType == WeaponType.Unarmed)
		{
			print("You are not armed yet");
			return false;
		}
		if (FastSkillGauge < 1)
		{
			print($"Not Enough Fast Skill Guage [{FastSkillGauge*100:F1}%]");
			return false;
		}
		return true;
	}
	public void ResetFastSkill()
	{
		FastSkillGauge = 0;
		OnFastGuageChanged?.Invoke();
	}
	public bool CanStrongSkill(WeaponType weaponType)
	{
		if (weaponType == WeaponType.Unarmed)
		{
			print("You are not armed yet");
			return false;
		}
		if (StrongSkillGauge < 1)
		{
			print($"Not Enough Strong Skill Guage [{StrongSkillGauge*100:F1}%]");
			return false;
		}
		return true;
	}
	public void ResetStrongSkill()
	{
		StrongSkillGauge = 0;
		OnStrongGuageChanged?.Invoke();
	}
	public bool CanCommonSkill(WeaponType weaponType)
	{
		print($"CommonSkill Cooldown {remainCoolTime:F2}s left");
		if (canCommonSkill == false)
		{
			print("Commonskill false");
			return false;
		}
		else if (weaponType == WeaponType.Unarmed)
		{
			print("UnArmed");
			return false;
		}
		else
		{
			print("Commonskill true");
			return true;
		}
	}
	private bool canCommonSkill = true;
	private float remainCoolTime = 0;
	public float RemainCoolTimeRate => remainCoolTime / CommonSkillCoolTime;
	private const float CommonSkillCoolTime = 120;
	public IEnumerator CommonSkillCoolDown()
	{
		canCommonSkill = false;
		remainCoolTime = CommonSkillCoolTime;
		while (remainCoolTime > 0)
		{
			remainCoolTime -= Time.deltaTime;
			yield return null;
		}
		remainCoolTime = 0;
		canCommonSkill = true;
	}
#region AnimationEvent
	public void OnFastSkillExecute(int weaponTypeIndex)
	{
		WeaponSkill skillData = fastSkillTable[(WeaponType)weaponTypeIndex];
		if (skillData == null)
			return ;

		if (isOwnedByPlayer == true)
		{
			Player player= holder as Player;
			if (player.IsTargetingMode() == true)
			{
				Vector3 position = player.TargetTransform.position;
				skillData.Execute(player, player.combatStateMachine.WeaponHandler.CurrentWeapon, null, position);
			}
			else
			{
				skillData.Execute(player, player.combatStateMachine.WeaponHandler.CurrentWeapon);
			}
		}
		else
		{
			EnemyDynamic ai = holder as EnemyDynamic;
			skillData.Execute(ai, ai.combatStateMachine.WeaponHandler.CurrentWeapon);
		}
	}
	public void OnFastSkillFinish(int weaponTypeIndex)
	{
		WeaponSkill skillData = fastSkillTable[(WeaponType)weaponTypeIndex];
		if (skillData == null)
			return ;

		if (isOwnedByPlayer == true)
		{
			Player player= holder as Player;
			skillData.Finish(player, player.combatStateMachine.WeaponHandler.CurrentWeapon);
		}

	}

	public void OnStrongSkillExecute(int weaponTypeIndex)
	{
		WeaponSkill skillData = strongSkillTable[(WeaponType)weaponTypeIndex];
		if (skillData == null)
			return ;

		if (isOwnedByPlayer == true)
		{
			Player player= holder as Player;
			skillData.Execute(player, player.combatStateMachine.WeaponHandler.CurrentWeapon);
		}
	}
	public void OnStrongSkillFinish(int weaponTypeIndex)
	{
		WeaponSkill skillData = strongSkillTable[(WeaponType)weaponTypeIndex];
		if (skillData == null)
			return ;

		if (isOwnedByPlayer == true)
		{
			Player player= holder as Player;
			skillData.Finish(player, player.combatStateMachine.WeaponHandler.CurrentWeapon);
		}
	}
	public void OnCommonSkillExecute()
	{
		ISpell spell = CommomSkill.GetSpecialActionSpell();
		spell.Execute(holder as Player, null, CommomSkill, null);
	}
	public void OnCommonSkillFinish()
	{

	}
#endregion
}
