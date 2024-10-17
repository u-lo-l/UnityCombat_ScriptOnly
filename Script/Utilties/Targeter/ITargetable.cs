using System;
using UnityEngine;

public interface ITargetable
{
	public event Action Ontargetdie;
	public Transform GetTransform();
	public bool CanTarget();
	public void BeTarget();
	public void NotTarget();
}
