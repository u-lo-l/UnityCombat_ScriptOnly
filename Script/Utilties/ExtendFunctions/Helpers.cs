using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum GroundMaterialType { Default, Sand, Grass, Pebble, Rock, Wood, Leaf };
public static class Extend_TransformHelpers
{
	public static Transform FindByName(this Transform transform, string name)
	{
		Transform[] transforms = transform.GetComponentsInChildren<Transform>();
		foreach(Transform t in transforms)
		{
			if (t.gameObject.name.Equals(name) == true)
			{
				return t;
			}
		}
		return null;
	}

	public static bool TryFindByName(this Transform transform, string name, out Transform result)
	{
		result = null;
		Transform[] transforms = transform.GetComponentsInChildren<Transform>();
		foreach(Transform t in transforms)
		{
			if (t.gameObject.name.Equals(name) == true)
			{
				result = t;
				return true;
			}
		}
		return false;
	}
}

public static class Extend_CameraHelpers
{
	private static RaycastHit hit;
	public static bool GetCusrorWorldLocation(this Camera camera, out Vector3 position, out Vector3 normal, float distance, LayerMask layerMask)
	{
		position = normal = Vector3.zero;

		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, distance, layerMask) == false)
			return false;

		position = hit.point;
		normal = hit.normal;
		return true;
	}

	public static bool GetCusrorWorldLocation(this Camera camera, out Vector3 position, float distance, LayerMask layerMask)
	{
		position = Vector3.zero;

		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, distance, layerMask) == false)
			return false;

		position = hit.point;
		return true;
	}

	public static bool HasCusrorWorldLocation(this Camera camera, float distance, LayerMask layerMask)
	{
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		return Physics.Raycast(ray, out hit, distance, layerMask);
	}
}

public static class Extend_CameraShakeHelper
{
	public static void Shake(this CinemachineImpulseSource source, CameraShakeImpulseData impulseData)
	{
		if (impulseData == null)
		{
			// Debug.LogWarning("ImpulseData Not Found");
		}
		else
		{
			source.m_ImpulseDefinition = impulseData.ImpulseDefinition;
			source.GenerateImpulse(impulseData.ShakeForce);
		}
	}
}

public static class Extend_Collider
{
	public static bool TryGetDamagable(this Collider collider,
										out IDamagable damagable,
										Character weaponOwner,
										GameObject weaponGameObject,
										LayerMask allyLayerMask,
										List<IDamagable> hitList = null)
	{
		damagable = null;
		Debug.Assert(allyLayerMask.value != 0, $"{weaponOwner.gameObject.name} | {weaponGameObject?.gameObject.name} has no ally layer mask");
		if (weaponGameObject != null && collider.gameObject == weaponGameObject)
		{
			return false;
		}
		if (weaponOwner == collider.gameObject)
		{
			return false;
		}
		if (CompareLayerMask(1 << collider.gameObject.layer, allyLayerMask) == true)
		{
			return false;
		}
		if (collider.gameObject.TryGetComponent(out damagable) == false)
		{
			return false;
		}
		if (damagable.IsDead() == true)
		{
			return false;
		}
		if (hitList != null && hitList.Contains(damagable) == true)
		{
			return false;
		}
		return true;

		static bool CompareLayerMask(LayerMask lhs, LayerMask rhs)
		{
			return (lhs.value & rhs.value) != 0;
		}
	}

}
