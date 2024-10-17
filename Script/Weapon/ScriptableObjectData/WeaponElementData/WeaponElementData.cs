using System;
using UnityEngine;

public abstract class WeaponElementData : ScriptableObject
{
	[field : SerializeField] public GameObject Prefab { get; private set; }
	[field : SerializeField] public string Name { get; private set; }
	[field : SerializeField] public WeaponSocket SocketType { get; private set; }
	[field : SerializeField] public Vector3 PositionOffset { get; private set; }
	[field : SerializeField] public Quaternion RotationOffset { get; private set; }
}