using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(WeaponHandler))]
public class PlayerCombatInputHandler : MonoBehaviour
{
	[SerializeField] private bool externalInputBlocked = false;
	[SerializeField] private EscManuUI escManuUI;
	private PlayerInput playerInput;
	private InputActionMap actionMap;
	public event Action<int> OnEquip;
	public event Action<AttackType> OnAttack;
	public event Action<AttackType> OnAttackCanceled;

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		actionMap = playerInput.actions.FindActionMap("Player");
		SetInputAction();
	}
	private void Start()
	{
		externalInputBlocked = false;
		SequenceManager.Instance?.RegisterSequencePlayEvent(-1, ReleaseControl);
		SequenceManager.Instance?.RegisterSequenceFinishEvent(-1, GainControl);
		if (escManuUI != null)
		{
			escManuUI.OnMenuActive += ReleaseControl;
			escManuUI.OnMenuDeactive += GainControl;
		}
	}

#region Input Action
	public void ReleaseControl()
	{
		// print("[PlayerCombatInput] : Release Control");
		externalInputBlocked = true;
	}
	public void GainControl()
	{
		// print("[PlayerCombatInput] : Gain Control");
		externalInputBlocked = false;
	}
	private void SetInputAction()
	{
		actionMap.FindAction("Fist", true).started += OnDoEquip;
		actionMap.FindAction("Sword", true).started += OnDoEquip;
		actionMap.FindAction("Warrior", true).started += OnDoEquip;
		actionMap.FindAction("Twohand", true).started += OnDoEquip;
		actionMap.FindAction("GreatSword", true).started += OnDoEquip;
		actionMap.FindAction("Dual", true).started += OnDoEquip;
		actionMap.FindAction("Staff", true).started += OnDoEquip;
		actionMap.FindAction("Bow", true).started += OnDoEquip;

		actionMap.FindAction("FastAttack").started += OnDoFastAttack;
		actionMap.FindAction("StrongAttack").started += OnDoStrongAttack;
		actionMap.FindAction("StrongAttack").canceled += OnDoStrongAttackCanceled;

		actionMap.FindAction("Spell01", true).started += OnDoFastWeaponSkill;
		actionMap.FindAction("Spell02", true).started += OnDoStrongWeaponSkill;
		actionMap.FindAction("Spell03", true).started += OnDoCommonSkill;
	}

	private void OnDoEquip(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		int weaponIndex = int.Parse(context.control.name);
		OnEquip?.Invoke(weaponIndex);
	}
	private void OnDoFastAttack(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnAttack?.Invoke(AttackType.FastAttack);
	}
	private void OnDoStrongAttack(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnAttack?.Invoke(AttackType.StrongAttack);
	}

	private void OnDoStrongAttackCanceled(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnAttackCanceled?.Invoke(AttackType.StrongAttack);
	}
#endregion

#region Spell
	public event Action<int> OnSpell;

	private void OnDoFastWeaponSkill(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnSpell.Invoke(0);
	}

	private void OnDoStrongWeaponSkill(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnSpell.Invoke(1);
	}

	private void OnDoCommonSkill(InputAction.CallbackContext context)
	{
		if (externalInputBlocked == true)
			return ;
		OnSpell.Invoke(2);
	}
#endregion
}
