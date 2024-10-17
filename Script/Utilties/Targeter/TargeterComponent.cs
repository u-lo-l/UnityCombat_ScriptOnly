using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class TargeterComponent : MonoBehaviour
{
	[SerializeField] private Transform cameraTransform;
	[SerializeField] private Volume globalVolume;
	private DepthOfField dof;
	[field : SerializeField] public bool AutoChangeTarget = false;
	private const float TargetSphereRadius = 15;
	[SerializeField] private List<ITargetable> targets = new();
	private new SphereCollider collider;
	private new Rigidbody rigidbody;
	public event Action OnTargetRemoved;
	private void Awake()
	{
		collider = GetComponent<SphereCollider>();
		rigidbody = GetComponent<Rigidbody>();
		collider.isTrigger = true;
		rigidbody.isKinematic = true;
		if (globalVolume != null)
			globalVolume.profile.TryGet<DepthOfField>(out dof);
	}

	private void Update()
	{
		if (returnedTarget != null)
		{
			float sqrDistance = (returnedTarget.GetTransform().position - transform.position).sqrMagnitude;
			if (sqrDistance > MathF.Pow(TargetSphereRadius + 5,2))
			{
				RemoveMember();
			}
		}
	}
	private Collider[] collierBuffer = new Collider[32];
	private void OnEnable()
	{
		if (dof != null)
			dof.active = true;
		SeachNearbyTargetables(5, 90);
	}

	private void OnDisable()
	{
		if (dof != null)
			dof.active = false;
		if (returnedTarget != null)
		{
			print($"[TargeterComponent] OnDisable");
			returnedTarget.NotTarget();
			returnedTarget = null;
		}
		targets.Clear();
	}

	[SerializeField ] private ITargetable returnedTarget = null;
	internal Transform GetTargetTransform(Transform playerTransform)
	{
		float max = float.NegativeInfinity;
		returnedTarget = null;

		if (SeachNearbyTargetables(2, 270) == true)
		{
			foreach(ITargetable targetable in targets)
			{
				Vector3 dir = targetable.GetTransform().position - playerTransform.position;
				float temp = Vector3.Dot(cameraTransform.forward, dir.normalized);
				if (max < temp)
				{
					max = temp;
					returnedTarget = targetable;
				}
			}
		}

		if (returnedTarget == null && SeachNearbyTargetables(5, 180) == true)
		{
			foreach(ITargetable targetable in targets)
			{
				Vector3 dir = targetable.GetTransform().position - playerTransform.position;
				float temp = Vector3.Dot(cameraTransform.forward, dir.normalized);
				if (max < temp)
				{
					max = temp;
					returnedTarget = targetable;
				}
			}
		}

		if (returnedTarget == null && SeachNearbyTargetables(10, 120) == true)
		{
			foreach(ITargetable targetable in targets)
			{
				Vector3 dir = targetable.GetTransform().position - playerTransform.position;
				float temp = Vector3.Dot(cameraTransform.forward, dir.normalized);
				if (max < temp)
				{
					max = temp;
					returnedTarget = targetable;
				}
			}
		}

		if (returnedTarget == null && SeachNearbyTargetables(15, 90) == true)
		{
			foreach(ITargetable targetable in targets)
			{
				Vector3 dir = targetable.GetTransform().position - playerTransform.position;
				float temp = Vector3.Dot(cameraTransform.forward, dir.normalized);
				if (max < temp)
				{
					max = temp;
					returnedTarget = targetable;
				}
			}
		}

		if (returnedTarget == null)
		{
			return null;
		}

		returnedTarget.BeTarget();
		returnedTarget.Ontargetdie -= RemoveMember;
		returnedTarget.Ontargetdie += RemoveMember;
		return returnedTarget.GetTransform();
	}

	private void RemoveMember()
	{
		print($"[TargeterComponent] Remove Target");
		if (returnedTarget == null)
			return ;
		returnedTarget.Ontargetdie -= RemoveMember;
		returnedTarget.NotTarget();
		returnedTarget = null;
		OnTargetRemoved?.Invoke();
	}

	private bool SeachNearbyTargetables(float radius, float angleInDegrees)
	{
		targets.Clear();
		int count = Physics.OverlapSphereNonAlloc(transform.position, radius, collierBuffer, GetLayerMask.GetEnemyLayerMask);
		for (int i = 0 ; i < count ; i++)
		{
			Vector3 dir = collierBuffer[i].transform.position - transform.position;
			if (Vector3.Dot(dir, cameraTransform.forward) <= Mathf.Acos(angleInDegrees * Mathf.Deg2Rad * 0.5f))
				continue;
			if (collierBuffer[i].TryGetComponent<ITargetable>(out ITargetable targetable))
			{
				if (targetable.CanTarget() == false)
					continue;
				if (targets.Contains(targetable) == false)
					targets.Add(targetable);
			}
		}
		return targets.Count > 0;
	}
}
