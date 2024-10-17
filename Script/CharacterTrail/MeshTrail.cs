using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
	[HideInInspector] public bool IsActive;
	[Header(" - Time setting")]
	[SerializeField]private float meshRefreshRate = 0.001f;
	[SerializeField]private int meshDurationFrame = 10;
	private float lastTrailTime;
	[SerializeField] private SkinnedMeshRenderer[] skinnedMeshRenderers;
	[Header(" - Shader setting")]
	[SerializeField] private Material trailMaterial;
	private const int trailCount = 4;
	private GameObject[] trailObjectPool;
	private IEnumerator[] coroutines;
	private Coroutine[] trailCoroutines;
	private WaitForFixedUpdate waitForFixedUpdate;
	private int objIndex = 0;
	private int skinnedMeshCount;
	private void Awake()
	{
		// skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
	}
	private void Start()
	{
		GameObject trailObjectParent = new() { name = "GhostEffect" };
		trailObjectPool = new GameObject[trailCount];
		coroutines = new IEnumerator[trailCount];
		trailCoroutines = new Coroutine[trailCount];
		waitForFixedUpdate = new WaitForFixedUpdate();
		for (int i = 0; i < trailObjectPool.Length; i++)
		{
			trailObjectPool[i] = new() { name = $"{gameObject.name}:GhostEffect{i}" };
			MeshFilter meshFilter = trailObjectPool[i].AddComponent<MeshFilter>();
			MeshRenderer meshRenderer = trailObjectPool[i].AddComponent<MeshRenderer>();

			meshFilter.mesh = new();

			foreach(SkinnedMeshRenderer sr in skinnedMeshRenderers)
			{
				Material[] materials = new Material[sr.materials.Length];
				for (int j = 0; j < materials.Length; j++)
				{
					materials[j] = trailMaterial;
				}
				meshRenderer.materials = materials;
			}
			trailObjectPool[i].transform.SetParent(trailObjectParent.transform);
			trailObjectPool[i].SetActive(false);
			coroutines[i] = TrailMeshLifeTime(trailObjectPool[i]);
		}
	}

	private void Update()
	{
		if (IsActive == true)
		{
			TrailMesh();
		}
	}
	private void TrailMesh()
	{
		if (Time.realtimeSinceStartup - lastTrailTime < meshRefreshRate / Time.timeScale)
			return ;
		foreach(SkinnedMeshRenderer sr in skinnedMeshRenderers)
		{
			Mesh mesh = new();
			sr.BakeMesh(mesh);
			trailObjectPool[objIndex].GetComponent<MeshFilter>().mesh = mesh;
		}
		coroutines[objIndex] = TrailMeshLifeTime(trailObjectPool[objIndex]);
		if (trailObjectPool[objIndex].activeSelf == true)
		{
			trailObjectPool[objIndex].SetActive(false);
			StopCoroutine(coroutines[objIndex]);
		}
		StartCoroutine(coroutines[objIndex]);
		lastTrailTime = Time.realtimeSinceStartup;
		objIndex = (objIndex + 1) % trailCount;
	}

	private IEnumerator TrailMeshLifeTime(GameObject trailObj)
	{
		trailObj.transform.SetPositionAndRotation(transform.root.transform.position + transform.root.forward * -0.1f, transform.root.rotation);
		trailObj.SetActive(true);
		Material[] mats = trailObj.GetComponent<MeshRenderer>().materials;
		for (int k = 0 ; k < mats.Length ; k++)
		{
			Color color = mats[k].color;
			color.a = 1f;
			mats[k].color = color;
		}
		for (int i = 0 ; i < meshDurationFrame ; i++)
		{
			for (int k = 0 ; k < mats.Length ; k++)
			{
				Color color = mats[k].color;
				color.a *= 0.5f;
				mats[k].color = color;
			}
			yield return waitForFixedUpdate;
		}
		trailObj.SetActive(false);
		yield break;
	}
}