using UnityEngine;
using static PlayerCombatInputHandler;

public class Bow : Melee
{
	BowStatData bowStat;
	[SerializeField] private GameObject arrowPrefab;
	[SerializeField] private Animator bowAnimator;
	[field : SerializeField] public Transform BowIKTransform { get; private set;}
	[field : SerializeField] public Transform StringIKTransform { get; private set; }
	private PlayerCombatInputHandler playerCombatInputHandler;
	public bool IsArrowReady {private get; set;} = false;
	private readonly int readyTriggerHash = Animator.StringToHash("readyTrigger");
	private readonly int shootTriggerHash = Animator.StringToHash("shootTrigger");
	private readonly int cancelTriggerHash = Animator.StringToHash("cancelTrigger");
	protected override void Awake()
	{
		base.Awake();
		EquipAnimationHash = AnimatorHash.Player.EquipBow;
		bowStat = Stat as BowStatData;
		if (weaponObjs[0].TryGetComponent<Animator>(out bowAnimator) == false)
		{
			Debug.Assert(false, "[BOW] : Bow Animator Not Found");
		}
		if (weaponObjs[0].transform.TryFindByName("STRING", out Transform stringTf) == false)
		{
			Debug.Assert(false, "[BOW] : String IK Point not Found");
		}
		StringIKTransform = stringTf;
		if (weaponObjs[0].transform.TryFindByName("Bow_Element", out Transform bowTf) == false)
		{
			Debug.Assert(false, "[BOW] : Bow IK Point not Found");
		}
		BowIKTransform = bowTf;
	}
	protected override void Start()
	{
		base.Start();
	}

	public override bool Equip()
	{
		bowAnimator.SetTrigger(cancelTriggerHash);
		if (Owner is Player)
		{
			if (playerCombatInputHandler == null)
			{
				playerCombatInputHandler = Owner.GetComponent<PlayerCombatInputHandler>();
			}
			if (Owner.TryGetComponent<BowHandIIKHandler>(out var ikHandler) == true)
			{
				ikHandler.StringIKTransform = StringIKTransform;
				ikHandler.BowIKTransform = BowIKTransform;
			}
		}
		playerCombatInputHandler.OnAttack -= Ready;
		playerCombatInputHandler.OnAttack += Ready;

		playerCombatInputHandler.OnAttackCanceled -= Release;
		playerCombatInputHandler.OnAttackCanceled += Release;
		return base.Equip();
	}

	public override void Unequip()
	{
		if (Owner is Player)
		{
			if (Owner.TryGetComponent<BowHandIIKHandler>(out var ikHandler) == true)
			{
				ikHandler.StringIKTransform = null;
			}
		}
		playerCombatInputHandler.OnAttack -= Ready;
		playerCombatInputHandler.OnAttackCanceled -= Release;
		base.Unequip();
		bowAnimator.ResetTrigger(shootTriggerHash);
		bowAnimator.ResetTrigger(readyTriggerHash);
	}

	private GameObject arrowObject;
	public void Ready(AttackType attackType)
	{
		bowAnimator.ResetTrigger(shootTriggerHash);
		bowAnimator.ResetTrigger(cancelTriggerHash);
		if (attackType == AttackType.StrongAttack)
		{
			print("READY ARROW");
			bowAnimator.SetTrigger(readyTriggerHash);
			if (arrowPrefab != null)
			{
				arrowObject = Instantiate<GameObject>(arrowPrefab);
				arrowObject.transform.SetParent(StringIKTransform);
				arrowObject.transform.SetLocalPositionAndRotation(new(0,-0.1134f,0.0097f), Quaternion.Euler(83.212f,0.097f,0.084f));
				arrowObject.transform.localScale = 0.25f * Vector3.one;
			}
		}
		else if (arrowObject != null)
		{
			Destroy(arrowObject);
			arrowObject = null;
		}
	}
	public void Release(AttackType attackType)
	{
		bowAnimator.ResetTrigger(readyTriggerHash);
		if (attackType == AttackType.StrongAttack)
		{
			if (IsArrowReady == true)
			{
				print("SHOOT ARROW");
				Arrow arrow = arrowObject.GetComponent<Arrow>();
				arrow.Owner = Owner;
				arrow.Weapon = this;
				arrow.TargetLayerMask = GetLayerMask.GetEnemyLayerMask;
				arrow.OnProjectileHit += OnArrowHit;
				arrow.Shoot();
				bowAnimator.SetTrigger(shootTriggerHash);
			}
			else
			{
				Destroy(arrowObject);
			}
			arrowObject = null;
		}
		bowAnimator.SetTrigger(cancelTriggerHash);
		IsArrowReady = false;
		attackType = AttackType.None;
	}

	private void OnArrowHit(Collider target, Weapon weapon, Vector3 hitPoint)
	{
		print("HIT");
		if (target.TryGetComponent<IDamagable>(out IDamagable damagable) == true)
		{
			DamageProcessor.ApplyDamage(damagable, weapon, Stat.StrongActionData[0], hitPoint, Vector3.zero);
			base.AttackSucceed(Stat.StrongActionData[0].GuageAmount);
		}
	}
}
