using System;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public partial class WeaponHandler : MonoBehaviour
{
	public Transform Armory;
	[SerializeField] public AudioSource AudioSource;
	[SerializeField] private SkillHandler skillHandler;
	[SerializeField] private Character holder;
	[SerializeField] private LayerMask allyLayerMask;
	[SerializeField] private GameObject[] everyWeaponPrefabs;
	private DecalProjector decalProjector;
	[SerializeField] private float characterHeight = 1f;
	public WeaponType ArmedType {get; private set;} = WeaponType.Unarmed;
	public event Action<WeaponType, WeaponType> OnWeaponChanged;
	private readonly Dictionary<WeaponType, Weapon> weaponTable = new();
	[field : SerializeField] public List<WeaponType> PossessingWeaponType { get ; private set; } = new();
	public WeaponType GetFirstWeaponType()
	{
		return (PossessingWeaponType.Count == 0) ? WeaponType.Unarmed : PossessingWeaponType[0];
	}
	public Weapon CurrentWeapon => weaponTable[ArmedType];
	public float WeaponRange => (CurrentWeapon == null) ? 0 : CurrentWeapon.Stat.Range;
	[HideInInspector] public bool CompletelyEquipped = false;
	public bool CanNextCombo = false;
	// [HideInInspector] public bool CanNextCombo = false;
	public int ActionIndex { get; private set; } = 0;
	public bool CanAttack { get; private set; } = true;
	private Coroutine AttackDelayCoroutine;
	private CinemachineImpulseSource impulseSource;

#region MonoBehaviour

	private void BuildArmory()
	{
		if (transform.TryFindByName("Armory", out Armory) == false)
		{
			Armory = new GameObject("Armory", typeof(AudioSource)).transform;
			Armory.SetParent(transform);
		}
		CreateRangeDecal();
	}
	private void CreateRangeDecal()
	{
		if (Armory.TryGetComponent(out decalProjector) == false)
		{
			decalProjector = Armory.AddComponent<DecalProjector>();
		}
		Armory.localPosition = new Vector3(0, characterHeight, 0);
		Armory.localEulerAngles = new Vector3(90, 0, 0);

		decalProjector.material = Resources.Load<Material>("WeaponRangeDecal");
		decalProjector.renderingLayerMask = 2u;
		decalProjector.drawDistance = characterHeight * 2;
		decalProjector.size = new Vector3(0, 0, characterHeight * 2);
		decalProjector.fadeScale = 1f;
		decalProjector.startAngleFade = 0f;
		decalProjector.endAngleFade = 20f;
		decalProjector.fadeFactor = 0.5f;
		decalProjector.pivot = new(0, 0, characterHeight / 2);
	}
	private void Awake()
	{
		BuildArmory();

		if (Armory.TryGetComponent(out AudioSource) == false)
		{
			Armory.gameObject.AddComponent<AudioSource>();
			AudioSource = Armory.GetComponent<AudioSource>();
		}
		AudioSource.minDistance = 1f;
		AudioSource.maxDistance = 20f;
		AudioSource.rolloffMode = AudioRolloffMode.Linear;
		AudioSource.spatialBlend = 1f;

		if (Armory.TryGetComponent(out CinemachineImpulseSource impulseSource) == false)
		{
			impulseSource = Armory.gameObject.AddComponent<CinemachineImpulseSource>();
		}

		for(int i = 0 ; i < (int)WeaponType.Max ; i++)
		{
			weaponTable.Add((WeaponType)i, null);
		}
		foreach(GameObject prefab in everyWeaponPrefabs)
		{
			if (prefab == null)
				continue;
			GameObject obj = Instantiate<GameObject>(prefab, Armory);
			Weapon weapon = obj.GetComponent<Weapon>();
			Debug.Assert(weapon != null, "[WeaponHandler] : Weapon Prefab Not Valid");
			obj.name = weapon.Stat.Name == "" ? weapon.Type.ToString() : weapon.Stat.Name;
			weaponTable[weapon.Type] = weapon;
			weapon.AllyLayerMask = allyLayerMask;
			weapon.RegisterOwner(GetComponent<Character>());
			weapon.audioSource = this.AudioSource;
			weapon.OnFastAttackSucceed += UpdateFastAttackGauge;
			weapon.OnStrongAttackSucceed += UpdateStrongAttackGauge;
			PossessingWeaponType.Add(weapon.Type);
		}
		RegisterWeaponSkills();

		if (holder is Player)
		{
			print($"Show Weapon Table : {gameObject.name}");
			foreach(KeyValuePair<WeaponType, Weapon> pair in weaponTable)
			{
				print($"<{pair.Key} | {(pair.Value == null ? "null" : pair.Value.name)}>");
			}
			print($"End Weapon Table : {gameObject.name}");
		}
	}
#endregion
	public bool ShouldStop() => ShouldStopForAttack() || ShouldStopForSkill();
	// private void OnDrawGizmos()
	// {
	// 	if(Application.isPlaying == false)
	// 		return ;
	// 	if (WeaponRange == 0)
	// 		return ;
	// 	Gizmos.color = new Color(1, 0, 0, 0.2f);
	// 	Gizmos.DrawWireSphere(transform.position, WeaponRange);
	// }
}
