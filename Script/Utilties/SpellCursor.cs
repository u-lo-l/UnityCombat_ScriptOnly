using System.Drawing;
using UnityEngine;

public class SpellCursor : MonoBehaviour
{
	Camera mainCamera;
	Vector3 position = Vector3.zero;
	Vector3 normal = Vector3.zero;
	bool isExist = false;

	public float TraceDistance { private get;  set; } = 500;
	private LayerMask mask;
	private void Awake()
	{
		GameObject sceneObject = GameObject.Find("Scene");
		if (sceneObject == null)
		{
			sceneObject = new() { name = "Scene" };
		}
		Transform sceneTransform = sceneObject.transform;
		transform.SetParent(sceneTransform, false);
		mask = 1 << LayerMask.NameToLayer("Obstacle");
	}

	private void Start()
	{
		mainCamera = Camera.main;
	}

	private void FixedUpdate()
	{
		if (mainCamera.GetCusrorWorldLocation(out position, out normal, TraceDistance, mask) == false)
		{
			isExist = false;
			return ;
		}

		if (Vector3.Dot(Vector3.up, normal) < 0.9)
		{
			isExist = false;
			return ;
		}
		
		Quaternion rotationOffset = Quaternion.FromToRotation(Vector3.down, Vector3.forward);
		Vector3 localUpFromWorld = rotationOffset * Vector3.up;

		transform.localPosition = position + normal * 0.5f;
		transform.localRotation = Quaternion.FromToRotation(localUpFromWorld, normal);
		isExist = true;
	}

	public bool GetHitPosition(out Vector3? hitPosition)
	{
		hitPosition = position;
		// Debug.Log($"[SpellCursor] : {isExist} : {hitPosition.Value}");
		return isExist;
	}
	public bool GetHitPosition(out Vector3 hitPosition)
	{
		// Debug.Log($"[SpellCursor] : {isExist}");
		hitPosition = position;
		return isExist;
	}
}
