using System.Collections.Generic;
using UnityEngine;
using static PlayerCombatInputHandler;

public abstract class Melee : Weapon
{
	[SerializeField] protected Collider[] colliders = null;
	[SerializeField] protected GameObject[] weaponTrails_Fast = null;
	[SerializeField] protected GameObject[] weaponTrails_Strong= null;
	[SerializeField] protected GameObject[] weaponTrails_Additional= null;
	private List<IDamagable> hitList;
#region Monobehaviour
	protected override void Awake()
	{
		base.Awake();
		SetWeaponColliderOnAwake();
		hitList = new();
	}

	protected override void Start()
	{
		base.Start();
		DisableAllColliers();
		SetWeaponGameObjectsOnStart();
		SetWeaponTrailsOnStart();
	}
	protected virtual void OnTriggerEnter(Collider other)
	{
		if (other.TryGetDamagable(out IDamagable damagable, Owner, gameObject, AllyLayerMask, hitList) == false)
			return ;

		print("melee hit");
		ActionData actionData = GetActionData(AttackingType);
		Debug.Assert(actionData != null, "[Melee] : acionData not found");

		hitList.Add(damagable);

		Player player = damagable as Player;
		if (player != null && player.IsDodging)
		{
			FrameStopManager.Instance.SlowEnemies();
			return ;
		}

		if (impulseSource!= null && (player != null || Owner is Player))
		{
			base.MakeCamereShake(actionData.ImpulseData);
		}

		Collider enabledCollider = null;
		foreach(Collider c in colliders)
		{
			if (c.enabled == true)
			{
				enabledCollider = c;
				break;
			}
		}

		Vector3 hitPoint = enabledCollider.ClosestPoint(other.transform.position);
		DamageProcessor.ApplyDamage(damagable, this, actionData, hitPoint, Vector3.zero);

		AttackSucceed(actionData.GuageAmount);
	}
	private void OnTriggerExit(Collider other)
	{
		// Do nothing;
	}
	protected virtual void OnDisable()
	{
		DisableAllColliers();
		foreach(GameObject obj in weaponObjs)
		{
			MeleeTrigger meleeTrigger = obj.GetComponent<MeleeTrigger>();
			if (meleeTrigger == null)
				continue;
			meleeTrigger.OnTriggerIn -= OnTriggerEnter;
			meleeTrigger.OnTriggerOut -= OnTriggerExit;
		}
	}
	private void SetWeaponColliderOnAwake()
	{
		int weaponElementCount = Stat.ElementCount;
		colliders = new Collider[weaponElementCount];
		for (int i = 0 ; i < weaponElementCount ; i++)
		{
			colliders[i] = weaponObjs[i].GetComponent<Collider>();
		}
		hitList = new();
		SetAllTriggerOn();
	}
	private void SetWeaponTrailsOnStart()
	{
		weaponTrails_Fast = new GameObject[Stat.ElementCount];
		weaponTrails_Strong = new GameObject[Stat.ElementCount];
		weaponTrails_Additional = new GameObject[Stat.ElementCount];
		TrailSetting(ref weaponTrails_Fast, Stat.FastActionCount, Stat.FastActionData);
		TrailSetting(ref weaponTrails_Strong, Stat.StrongActionCount, Stat.StrongActionData);
		int size = Stat.AdditionalActionSize;
		for(int i = 0 ; i < size ; i++)
		{
			ActionData[] temp = Stat.AdditionalActionData[i].ActionData;
			TrailSetting(ref weaponTrails_Additional, temp.Length, temp);
		}

		void TrailSetting(ref GameObject[] trails, int actionCount, ActionData[] actionData)
		{
			for(int i = 0 ; i < actionCount ; i++)
			{
				ActionData data = actionData[i];
				if (data.TrailParticle == null || trails[data.Elementindex] != null)
					continue;

				trails[data.Elementindex] = Instantiate(data.TrailParticle, weaponObjs[data.Elementindex].transform);
				trails[data.Elementindex].transform.SetLocalPositionAndRotation(data.TrailPositionOffset, data.TrailRotationOffset);
				trails[data.Elementindex].SetActive(false);
			}
		}
	}
	private void SetWeaponGameObjectsOnStart()
	{
		foreach(GameObject obj in weaponObjs)
		{
			if (obj.TryGetComponent<MeleeTrigger>(out var meleeTrigger) == true)
			{
				meleeTrigger.OnTriggerIn -= OnTriggerEnter;
				meleeTrigger.OnTriggerOut -= OnTriggerExit;

				meleeTrigger.OnTriggerIn += OnTriggerEnter;
				meleeTrigger.OnTriggerOut += OnTriggerExit;

				meleeTrigger.SetOwner(Owner);
			}
		}
	}
	private ActionData GetActionData(AttackType AttackingType)
	{
		return AttackingType switch
		{
			AttackType.FastAttack => Stat.FastActionData[ActionIndex],
			AttackType.StrongAttack => Stat.StrongActionData[ActionIndex],
			AttackType.FastSkill => Stat.WeaponSkillData[0].AttackData,
			AttackType.StrongSkill => Stat.WeaponSkillData[1].AttackData,
			_ => Stat.AdditionalActionData[AdditionalIndex].ActionData[ActionIndex],
		};
	}
#endregion
	public override void FastAttack(int attackIndex)
	{
		base.FastAttack(attackIndex);
	}
	public override void StrongAttack(int attackIndex)
	{
		base.StrongAttack(attackIndex);
	}

#region Collider Trigger
	private void SetAllTriggerOn()
	{
		if (colliders == null)
			return ;
		foreach(Collider c in colliders)
			c.isTrigger = true;
	}
	public void DisableAllColliers()
	{
		if (colliders == null)
			return ;
		foreach(Collider c in colliders)
			c.enabled = false;
	}
	public void EnableCollider(int index = 0)
	{
		Debug.Assert(index < colliders.Length, $"index out of range of colliderList {index} < {colliders.Length}");
		hitList.Clear();
		colliders[index].enabled = true;

		GameObject[] weaponTrails;
		switch (AttackingType)
		{
			case AttackType.FastAttack :
			case AttackType.FastSkill :
				weaponTrails = weaponTrails_Fast;
			break;
			case AttackType.StrongAttack :
			case AttackType.StrongSkill:
				weaponTrails = weaponTrails_Strong;
			break;
			default :
				weaponTrails = weaponTrails_Additional;
			break;
		}

		if (weaponTrails[index] != null)
			weaponTrails[index].SetActive(true);
	}
	public void DisableCollider(int index = 0)
	{
		Debug.Assert(index < colliders.Length, "index out of range of colliderList");
		hitList.Clear();
		GameObject[] weaponTrails;
		switch (AttackingType)
		{
			case AttackType.FastAttack :
			case AttackType.FastSkill :
				weaponTrails = weaponTrails_Fast;
			break;
			case AttackType.StrongAttack :
			case AttackType.StrongSkill:
				weaponTrails = weaponTrails_Strong;
			break;
			default :
				weaponTrails = weaponTrails_Additional;
			break;
		}

		if (index < 0)
		{
			DisableAllColliers();
			foreach(GameObject trailObj in weaponTrails)
			{
				if (trailObj == null)
					continue;
				trailObj.SetActive(false);
			}
			return ;
		}
		colliders[index].enabled = false;
		weaponTrails[index]?.SetActive(false);
	}
#endregion
}
