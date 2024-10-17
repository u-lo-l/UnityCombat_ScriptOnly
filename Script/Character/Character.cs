using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterStatusComponent))]
[RequireComponent(typeof(EnvironmentChecker))]
[RequireComponent(typeof(AudioSource))]
public abstract partial class Character : MonoBehaviour, IDamagable, IFrameStoppable
{
	protected GameObject lookAt;
	public Animator Animator { get; protected set; }
	public AudioSource AudioSource { get; protected set; }
	public EnvironmentChecker EnvironmentChecker {get; private set;}
	[field : SerializeField] public float Height {get; protected set;}
	public bool ShouldSelfDestroy {protected get; set;} = true;
	protected virtual bool IsDead => CharacterStatus == null || CharacterStatus.IsDead;
	protected new Collider collider;
	public CharacterStatusComponent CharacterStatus {get; protected set;}
	[HideInInspector, SerializeField] protected List<Material> skinMaterials;
	[HideInInspector, SerializeField] protected List<Color> skinColors;
#region MonoBehaviour
	protected virtual void Awake()
	{
		collider = GetComponent<Collider>();
		Animator = GetComponent<Animator>();
		AudioSource = GetComponent<AudioSource>();
		CharacterStatus = GetComponent<CharacterStatusComponent>();
		EnvironmentChecker = GetComponent<EnvironmentChecker>();

		SkinnedMeshRenderer[] skins = GetComponentsInChildren<SkinnedMeshRenderer>();
		if (skins == null || skins?.Length == 0)
			return ;
		for(int i = 0 ; i < skins.Length ; i++)
		{
			skinMaterials.AddRange(skins[i].materials);
		}
		int count = skinMaterials.Count;
		skinColors = new(count);
		for(int i = 0 ; i < count ; i++)
		{
			skinColors.Add(skinMaterials[i].color);
		}
	}
	protected virtual void Start()
	{
		FrameStopManager.Instance.RegisterStoppable(this);
		MakeLookAtTransform();
	}
	protected virtual void OnDisable()
	{
		FrameStopManager.Instance.UnregisterStoppable(this);
	}
	protected virtual void OnDestroy()
	{
		int count = skinMaterials.Count;
		for(int i = count - 1 ; i >= 0 ; i--)
		{
			Destroy(skinMaterials[i]);
		}
	}
	private void MakeLookAtTransform()
	{
		Transform temp = transform.Find("LookAt");
		if (temp == null)
		{
			lookAt = new GameObject("LookAt");
			lookAt.transform.SetParent(this.transform);
		}
		else
		{
			lookAt = temp.gameObject;
		}
		lookAt.transform.localPosition = new Vector3(0, Height * 0.8f, 0);
	}
	#endregion
}

#region Speed Part
public abstract partial class Character
{
	private float prevAnimSpeed;
	void IFrameStoppable.Freeze(float speed)
	{
		if (Animator == null)
			return ;
		prevAnimSpeed = Animator.speed;
		Animator.speed = speed;
	}
	void IFrameStoppable.Release()
	{
		if (Animator == null)
			return ;
		Animator.speed = prevAnimSpeed;
		prevAnimSpeed = 1;
	}
	public virtual void FastMode() { }
	public virtual void RelaseFastMode() { }
}
#endregion



