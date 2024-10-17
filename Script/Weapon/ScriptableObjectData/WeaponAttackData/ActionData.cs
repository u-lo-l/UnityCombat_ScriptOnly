using System;
using UnityEngine;

[Serializable]
public enum CrowdControl { None, Down, Airborne, }
public enum AttackType { None, FastAttack, StrongAttack, FastSkill, StrongSkill, Else };
public abstract class ActionData : ScriptableObject
{
	[field : Header("Name")]
	[field : SerializeField] public string Name { get; private set; }

	[field : Header("Skill Guage")]
	[field : SerializeField, Range(0f, 0.25f)] public float GuageAmount { get; private set; } = 0.15f;


	[field : Header("Movable")]
	[field : SerializeField] public bool AutoRotation { get; private set; } = true;
	[field : SerializeField] public bool CanMove { get; private set; } = false;
	[field : SerializeField] public CrowdControl CrowdControl { get; private set; } = CrowdControl.None;


	[field : Header("Attack Setting")]
	[field : SerializeField] public AttackType ActionType {get; private set;}
	[field : SerializeField, Tooltip("Delay min, max in secs")] public Vector2 Delay { get; private set; }
	[field : SerializeField, Range(5, 1000)] public float Damage { get; private set; }
	[field : SerializeField, Range(0, 5000)] public float ThrustPower { get; private set; }
	[field : SerializeField] public Vector3 ThrustDirection {get; private set;}
	[field : SerializeField] public AnimationCurve ThrustCurve {get; private set;}
	[field : SerializeField] public float ImpactingTime {get; private set;}
	[field : SerializeField, Range(0, 20)] public int StopFrame { get; private set; }


	[field : Header("Camera Shake Effect")]
	[field : SerializeField] public CameraShakeImpulseData ImpulseData;

	[field : Header("Attack Effect")]
	[field : SerializeField] public int Elementindex { get; private set; } = -1;
	[field : SerializeField] public GameObject TrailParticle { get; private set; }
	[field : SerializeField] public Vector3 TrailPositionOffset { get; private set; } = Vector3.zero;
	[field : SerializeField] public Quaternion TrailRotationOffset { get; private set; } = Quaternion.identity;


	[field : Header("Hit Effect Scaler")]
	[field : SerializeField] public Vector3 HitParticleScaler { get; private set; } = Vector3.one;


	[field : Header("SFX")]
	[field : SerializeField] public AudioClip AttackSound { get; private set; }
	[field : SerializeField, Range(0f, 1f)] public float clipStartTimeRate { get; private set; }
	[field : SerializeField, Range(0.3f, 1.25f)] public float DefaultPitch { get; private set; } = 1f;
	[field : SerializeField] public AudioClip HitSound { get; private set; }


	[Header("SpecialAction")]
	[SerializeField] private int specialActionIndex;
	[SerializeField] private ScriptableObject[] Spells;
	private bool HasSpecialAction => Spells != null && Spells.Length > 0;
	public ISpell GetSpecialActionSpell()
	{
		if (HasSpecialAction == false)
			return null;
		else
			return Spells[specialActionIndex] as ISpell;
	}
	public ActionData()
	{
		if (TrailParticle != null && Elementindex < 0)
		{
			Debug.Assert(false, $"[AttackDAta] : You should set element index for trail effect for {this.GetType()}");
		}
		if (Delay.x < 0 ||  Delay.y < 0)
		{
			Debug.Assert(false, $"Delay Can't be negative");
		}
		if (Delay.x > Delay.y)
		{
			Debug.Assert(false, $"Min Delay can't be bigger than Max Delay");
		}
	}
}