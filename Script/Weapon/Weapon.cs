using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using static PlayerCombatInputHandler;

public abstract class Weapon : MonoBehaviour
{
	[HideInInspector] public AudioSource audioSource;
	[SerializeField] protected CinemachineImpulseSource impulseSource;
	private Vector3? capturedPosition = null;
	public Character Owner {get; set;}
	[field : SerializeField] public WeaponType Type {get; protected set;}
	[field : SerializeField] public WeaponStatData Stat {get; protected set;}
	[HideInInspector] public int EquipAnimationHash {get; protected set;}
	[SerializeField] protected GameObject[] weaponObjs;
	public Transform WeaponObjTransform(int index) => weaponObjs[index].transform;
	protected GameObject rootObject;
	[SerializeField] protected Animator animator;
	private readonly List<Material> materials = new();
	[HideInInspector] public int ActionIndex { protected get; set; } = 0;
	public int AdditionalIndex { protected get; set; }
	[HideInInspector] public AttackType AttackingType = AttackType.FastAttack;
	public float AttackRange => Stat.Range;
	protected bool IsPossessedByPlayer => Owner is Player;
	protected bool IsPossessedByEnemy => Owner is EnemyBase;
	[field : SerializeField] public LayerMask AllyLayerMask {get; set;}

	private const int HitParticlePoolSize = 4;
	private int fastHitParticleIndex = 0;
	private readonly GameObject[] fastHitParticlePool = new GameObject[HitParticlePoolSize];
	private int strongHitParticleIndex = 0;
	private readonly GameObject[] strongHitParticlePool = new GameObject[HitParticlePoolSize];
	private Transform particlePoolTransform;

#region MonoBehaviour
	protected virtual void Awake()
	{
		rootObject = transform.root.gameObject;
		animator = rootObject.GetComponent<Animator>();
		impulseSource = GetComponentInParent<CinemachineImpulseSource>();
		Type = Stat.Type;

		Debug.Assert(rootObject != null, $"{gameObject.name} : Cannot find rootObject of weapon({Type})");
		Debug.Assert(Stat != null, $"{gameObject.name} : Cannot find weapon stat data ({Type})");
		Debug.Assert(Stat.FastActionCount + Stat.StrongActionCount > 0, $"{gameObject.name} : AttackData Not Found ({Type})");
		Debug.Assert(Stat.ElementCount != 0, $"{gameObject.name} : Weapon element Not Found ({Type})");
		Debug.Assert(animator != null, $"{gameObject.name} : Cannot find animator from ({rootObject.name})");

		int weaponElementCount = Stat.ElementCount;
		weaponObjs = new GameObject[weaponElementCount];
		for (int i = 0 ; i < weaponElementCount ; i++)
		{
			Transform socketTransform = rootObject.transform.FindByName("Socket:" + Stat.WeaponElements[i].SocketType);
			weaponObjs[i] = Instantiate<GameObject>(Stat.WeaponElements[i].Prefab, socketTransform);
			weaponObjs[i].transform.SetLocalPositionAndRotation(Stat.WeaponElements[i].PositionOffset, Stat.WeaponElements[i].RotationOffset);
			weaponObjs[i].name = Stat.WeaponElements[i].Name;
			weaponObjs[i].SetActive(false);
		}
		for (int i = 0 ; i < HitParticlePoolSize ; i++)
		{
			if (Stat.FastHitParticle != null)
			{
				fastHitParticlePool[i] = Instantiate<GameObject>(Stat.FastHitParticle, transform);
				fastHitParticlePool[i].SetActive(false);
			}
			if (Stat.StrongHitParticle != null)
			{
				strongHitParticlePool[i] = Instantiate<GameObject>(Stat.StrongHitParticle, transform);
				strongHitParticlePool[i].SetActive(false);
			}
		}
		GameObject particlePoolObj = GameObject.Find("ParticlePool");
		if (particlePoolObj == null)
		{
			particlePoolObj = new("ParticlePool");
		}
		particlePoolTransform = particlePoolObj.transform;
	}
	protected virtual void Start()
	{
		foreach(GameObject obj in weaponObjs)
		{
			if(obj.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer) == false)
				continue;
			materials.AddRange(meshRenderer.materials);
		}
	}
	private void OnDisable()
	{
		int count = materials.Count;
		Color matColor;
		for(int i = 0 ; i < count ; i ++)
		{
			matColor = materials[i].color;
			matColor.a = 0;
			materials[i].color = matColor;
		}
	}
#endregion

#region Equipment
	public void RegisterOwner(Character owner)
	{
		this.Owner = owner;
	}

	public virtual bool Equip()
	{
		int equipAnimationHash;
		int layer;

		if (Owner is EnemyBoss)
		{
			equipAnimationHash = EquipAnimationHash;
			layer = AnimatorHash.Boss.EquipLayer;
		}
		else if (Owner is BossGolem)
		{
			equipAnimationHash = AnimatorHash.BossGolem.Equip;
			layer = AnimatorHash.BossGolem.EquipLayer;
		}
		else if (IsPossessedByEnemy)
		{
			print("owner is enemy");
			equipAnimationHash = AnimatorHash.Enemy.Equip;
			layer = AnimatorHash.Enemy.EquipLayer;
		}
		else
		{
			print("owner is player");
			equipAnimationHash = EquipAnimationHash;
			layer = AnimatorHash.Player.EquipLayer;
		}
		if (Stat.HasEquipAnimation == false)
		{
			animator.Play(equipAnimationHash, layer);
			return true;
		}
		if (equipAnimationHash == 0)
		{
			return false;
		}
		if (animator == null)
		{
			animator = rootObject.GetComponent<Animator>();
		}
		animator.CrossFadeInFixedTime(equipAnimationHash, 0.1f, layer);
		return true;
	}
	public virtual void ActiveWeapon()
	{
		foreach(GameObject obj in weaponObjs)
		{
			obj.SetActive(true);
		}
	}
	public virtual void ActiveWeapon(int index)
	{
		Debug.Log($"[{Owner.gameObject.name}] : {name}Activate  {weaponObjs[index].name}");
		weaponObjs[index].SetActive(true);
	}
	public virtual void DeactiveWeapon(int index)
	{
		if (weaponObjs[index].activeSelf == false)
			return;
		Debug.Log($"[{Owner.gameObject.name}] : {name}Deactivate  {weaponObjs[index].name}");
		weaponObjs[index].SetActive(false);
	}
	public virtual void Unequip()
	{
		foreach(GameObject obj in weaponObjs)
		{
			obj.SetActive(false);
		}
		int unequipAnimationHash;
		int layer;
		if (Owner is BossGolem)
		{
			unequipAnimationHash = AnimatorHash.BossGolem.Unequip;
			layer = AnimatorHash.BossGolem.EquipLayer;
		}
		else if (IsPossessedByEnemy)
		{
			print("owner is enemy");
			unequipAnimationHash = AnimatorHash.Enemy.Unequip;
			layer = AnimatorHash.Enemy.EquipLayer;
		}
		else
		{
			print("owner is player");
			unequipAnimationHash = AnimatorHash.Player.Unequip;
			layer = AnimatorHash.Player.EquipLayer;
		}
		animator.Play(unequipAnimationHash, layer);
	}
#endregion

#region Action
	public int IncreaseAttackIndex()
	{
		int maxCount = (AttackingType == AttackType.FastAttack) ? Stat.FastActionCount : Stat.StrongActionCount;
		ActionIndex = (ActionIndex + 1) % maxCount;
		return ActionIndex;
	}
	public virtual void FastAttack(int attackIndex)
	{
		this.AttackingType = AttackType.FastAttack;
		this.ActionIndex = attackIndex;
	}
	public virtual void StrongAttack(int attackIndex)
	{
		this.AttackingType = AttackType.StrongAttack;
		this.ActionIndex = attackIndex;
	}
	public void SpecialAction(int attackIndex)
	{
		this.ActionIndex = attackIndex;
		ActionData actionData;
		switch(AttackingType)
		{
			case AttackType.FastAttack :
				actionData = Stat.FastActionData[ActionIndex];
			break;
			case AttackType.StrongAttack :
				actionData = Stat.StrongActionData[ActionIndex];
			break;
			default :
				actionData = null;
			break;
		}

		ISpell spell = actionData.GetSpecialActionSpell();
		if(spell == null)
		{
			return ;
		}

		if (IsPossessedByPlayer == true)
		{
			Player player = Owner as Player;
			spell.Execute(player, this, actionData, capturedPosition);
		}
		else if (IsPossessedByEnemy == true)
		{
			EnemyBase enemy = Owner as EnemyBase;
			spell.Execute(enemy, this, actionData, enemy.Detector.Target?.transform.position);
		}
		else
		{
			Debug.Assert(false, $"[Staff] : Owner Type Not Correct {Owner.GetType()}");
		}
	}
	public virtual void ReleaseSpellCursorPosition()
	{
		capturedPosition = null;
	}
	protected virtual void PlayParticle()
	{

	}
	public void MakeCamereShake(CameraShakeImpulseData impulseData)
	{
		impulseSource.Shake(impulseData);
	}
#endregion
#region Attack Success Action
	public event Action<float> OnFastAttackSucceed;
	public event Action<float> OnStrongAttackSucceed;
	protected void AttackSucceed(float amount)
	{
		if (AttackingType == AttackType.FastAttack)
			OnFastAttackSucceed?.Invoke(amount);
		else if (AttackingType == AttackType.StrongAttack)
			OnStrongAttackSucceed?.Invoke(amount);
	}
#endregion
	public void ActivateHitParicle(IDamagable damagable, Vector3 hitPointOnWorld, ActionData actionData)
	{
		Vector3 dirOnWorld = hitPointOnWorld - damagable.GetTransform().position;
		dirOnWorld = Vector3.ProjectOnPlane(dirOnWorld.normalized, Vector3.up);

		GameObject particle = null;
		switch (actionData.ActionType)
		{
			case AttackType.FastAttack:
			case AttackType.FastSkill:
				particle = fastHitParticlePool[fastHitParticleIndex];
				fastHitParticleIndex = (fastHitParticleIndex + 1) % HitParticlePoolSize;
				break;
			case AttackType.StrongAttack:
			case AttackType.StrongSkill:
				particle = strongHitParticlePool[strongHitParticleIndex];
				strongHitParticleIndex = (strongHitParticleIndex + 1) % HitParticlePoolSize;
				break;
		}
		if (particle == null)
		{
			Debug.LogWarning("No Particle for hit");
			return ;
		}
		particle.transform.SetParent(particlePoolTransform, false);
		particle.transform.SetLocalPositionAndRotation(hitPointOnWorld, Quaternion.LookRotation(dirOnWorld));
		particle.transform.localScale = actionData.HitParticleScaler;
		particle.SetActive(true);
	}
}
