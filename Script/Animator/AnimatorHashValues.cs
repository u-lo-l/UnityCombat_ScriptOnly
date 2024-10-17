using UnityEngine;

namespace AnimatorHash
{
	public static class Player
	{

#region Layer
		public static readonly int BaseLayer = 0;
		public static readonly int ActionLayer = 1;
		public static readonly int UpperActionLayer = 2;
		public static readonly int EquipLayer = 3;
		public static readonly int DamagedLayer = 4;
#endregion

#region Parameters
		public static readonly int SpeedZ = Animator.StringToHash("SpeedZ");
		public static readonly int SpeedX = Animator.StringToHash("SpeedX");
		public static readonly int DodgeTrigger = Animator.StringToHash("DodgeTrigger_F");
		public static readonly int FastActionTrigger = Animator.StringToHash("FastActionTrigger");
		public static readonly int FastActionIndex = Animator.StringToHash("FastActionIndex");
		public static readonly int StrongActionTrigger = Animator.StringToHash("StrongActionTrigger");
		public static readonly int StrongActionCanMoveTrigger = Animator.StringToHash("StrongActionTrigger_CanMove");
		public static readonly int IsFreeLookState = Animator.StringToHash("IsFreeLookState");
#endregion

#region Locomotion
		public static readonly int UnarmedLoco = Animator.StringToHash("Unarmed_Locomotion");
		public static readonly int FistLoco = Animator.StringToHash("Fist_Locomotion");
		public static readonly int SwordLoco = Animator.StringToHash("Sword_Locomotion");
		public static readonly int WarriorLoco = Animator.StringToHash("Warrior_Locomotion");
		public static readonly int TwoHandLoco = Animator.StringToHash("Twohand_Locomotion");
		public static readonly int GreatSwordLoco = Animator.StringToHash("GreatSword_Locomotion");
		public static readonly int DualLoco = Animator.StringToHash("Dual_Locomotion");
		public static readonly int StaffLoco = Animator.StringToHash("Staff_Locomotion");
		public static readonly int BowLoco = Animator.StringToHash("Bow_Locomotion");
#endregion

#region Movement
		public static readonly int Jump = Animator.StringToHash("Jump");
		public static readonly int Dodge = Animator.StringToHash("Dodge");
#endregion

#region Equipment
		public static readonly int Unequip = Animator.StringToHash("Unequip");
		public static readonly int EquipFist = Animator.StringToHash("Fist_Equip");
		public static readonly int EquipSword = Animator.StringToHash("Sword_Equip");
		public static readonly int EquipWarrior = Animator.StringToHash("Warrior_Equip");
		public static readonly int EquipTwohand = Animator.StringToHash("Twohand_Equip");
		public static readonly int EquipGreatSword = Animator.StringToHash("GreatSword_Equip");
		public static readonly int EquipDual = Animator.StringToHash("Dual_Equip");
		public static readonly int EquipStaff = Animator.StringToHash("Staff_Equip");
		public static readonly int EquipBow = Animator.StringToHash("Bow_Equip");

		private static readonly int Unarmed = Animator.StringToHash("Unarmed");
		private static readonly int Fist = Animator.StringToHash("Fist");
		private static readonly int Sword = Animator.StringToHash("Sword");
		private static readonly int Warrior = Animator.StringToHash("Warrior");
		private static readonly int Twohand = Animator.StringToHash("Twohand");
		private static readonly int GreatSword = Animator.StringToHash("GreatSword");
		private static readonly int Dual = Animator.StringToHash("Dual");
		private static readonly int Staff = Animator.StringToHash("Staff");
		private static readonly int Bow = Animator.StringToHash("Bow");
		public static readonly int[] ArmedStateHashed = new int[] {
			Unarmed, Fist, Sword, Warrior, Twohand, GreatSword, Dual, Staff, Bow
		};
#endregion
	}

	public static class Enemy
	{
#region Layer
		public static readonly int BaseLayer = 0;
		public static readonly int ActionLayer = 1;
		public static readonly int EquipLayer = 2;
		public static readonly int DamagedLayer = 3;
		public static readonly int DownLayer = 4;
		public static readonly int GuardingLayer = 5;
#endregion
#region Locomotion
		public static readonly int Loco = Animator.StringToHash("Locomotion");
#endregion
#region Clip
		public static readonly int Unequip = Animator.StringToHash("Unequip");
		public static readonly int Down = Animator.StringToHash("Down");
		public static readonly int Airborne = Animator.StringToHash("Airborne");
		public static readonly int Equip = Animator.StringToHash("Equip");
#endregion
#region Parameters
		public static readonly int SpeedX = Animator.StringToHash("SpeedX");
		public static readonly int SpeedZ = Animator.StringToHash("SpeedZ");
		public static readonly int DodgeTrigger = Animator.StringToHash("DodgeTrigger");
		public static readonly int ActionTrigger = Animator.StringToHash("ActionTrigger");
		public static readonly int ActionIndex = Animator.StringToHash("ActionIndex");
		public static readonly int DieTrigger = Animator.StringToHash("DieTrigger");
		public static readonly int GetUpTrigger = Animator.StringToHash("GetUpTrigger");
		public static readonly int WeaponIndex = Animator.StringToHash("WeaponIndex");
#endregion
	}

	public static class Boss
	{
#region Layer
		public static readonly int BaseLayer = 0;
		public static readonly int ActionLayer = 1;
		public static readonly int EquipLayer = 2;
		public static readonly int DamagedLayer = 3;
#endregion
#region Locomotion
		public static readonly int Loco = Animator.StringToHash("Locomotion");
#endregion
#region Parameters
		public static readonly int Speed = Animator.StringToHash("Speed");
		public static readonly int RotateAngle = Animator.StringToHash("RotateAngle");
		public static readonly int DieTrigger = Animator.StringToHash("DieTrigger");
		public static readonly int ActionTrigger = Animator.StringToHash("ActionTrigger");
		public static readonly int ActionIndex = Animator.StringToHash("ActionIndex");
		public static readonly int WeaponIndex = Animator.StringToHash("WeaponIndex");
		public static readonly int WakeUpTrigger = Animator.StringToHash("WakeUpTrigger");
		public static readonly int RotateTrigger = Animator.StringToHash("RotateTrigger");

#endregion
	}

	public static class BossGolem
	{
		public static readonly int BaseLayer = 0;
		public static readonly int ActionLayer = 1;
		public static readonly int EquipLayer = 2;
		public static readonly int DamagedLayer = 3;
	#region Locomotion
		public static readonly int Loco = Animator.StringToHash("Locomotion");
		public static readonly int Equip = Animator.StringToHash("Equip");
		public static readonly int Unequip = Animator.StringToHash("Unequip");
#endregion
#region Parameters
		public static readonly int SpeedZ = Animator.StringToHash("SpeedZ");
		public static readonly int SpeedX = Animator.StringToHash("SpeedX");
		public static readonly int RotateAngle = Animator.StringToHash("RotateAngle");
		public static readonly int DieTrigger = Animator.StringToHash("DieTrigger");
		public static readonly int ActionTrigger = Animator.StringToHash("ActionTrigger");
		public static readonly int ActionIndex = Animator.StringToHash("ActionIndex");
		public static readonly int WeaponIndex = Animator.StringToHash("WeaponIndex");
		public static readonly int WakeUpTrigger = Animator.StringToHash("StandUpTrigger");
		public static readonly int RotateTrigger = Animator.StringToHash("RotateTrigger");
#endregion
	}
	public static class Damaged
	{
		public static readonly int Damage_High_Front = Animator.StringToHash("Damage_High_Front");
		public static readonly int Damage_High_Rear = Animator.StringToHash("Damage_High_Rear");
		public static readonly int Damage_High_Right = Animator.StringToHash("Damage_High_Right");
		public static readonly int Damage_High_Left = Animator.StringToHash("Damage_High_Left");
		public static readonly int Damage_Middle_Front = Animator.StringToHash("Damage_Middle_Front");
		public static readonly int Damage_Middle_Rear = Animator.StringToHash("Damage_Middle_Rear");
		public static readonly int Damage_Middle_Left = Animator.StringToHash("Damage_Middle_Left");
		public static readonly int Damage_Middle_Right = Animator.StringToHash("Damage_Middle_Right");
		public static readonly int Damage_Low_Left = Animator.StringToHash("Damage_Low_Left");
		public static readonly int Damage_Low_Right = Animator.StringToHash("Damage_Low_Right");
	}
}


