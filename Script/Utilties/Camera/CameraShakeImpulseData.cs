using System;
using Cinemachine;
using UnityEngine;

[Serializable, CreateAssetMenu(fileName = "CameraShakeImpulseData", menuName = "CameraShakeImpulseData")]
public class CameraShakeImpulseData : ScriptableObject
{
#region Impulse Source Setting
	[field : Header("ImpulseDefinition Setting")]
	[SerializeField, Range(0, 2f)] private float shakePower = 1;
	[field : SerializeField] public CinemachineImpulseDefinition ImpulseDefinition { get; private set; }
	[SerializeField] private bool x = false;
	[SerializeField] private bool y = false;
	[SerializeField] private bool z = false;
	public Vector3 ShakeForce =>  new Vector3(x ? 1 : 0, y ? 1 : 0, z ? 1 : 0) * shakePower;
#endregion

#region Impulse Listener Setting
	[field : Header("Impulse Listener Setting")]
	[field : SerializeField] public float ListenerAmplitude = 1f;
	[field : SerializeField] public float ListenerFrequency = 1f;
	[field : SerializeField] public float ListenerDuration = 1f;
#endregion
}