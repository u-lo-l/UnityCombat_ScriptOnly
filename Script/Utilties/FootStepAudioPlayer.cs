using System;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAudioPlayer : MonoBehaviour
{
	private readonly float footStepVolume = 0.5f;
	[SerializeField] private Transform FootStepPlayer;
	[SerializeField] private AudioSource AudioSource;
	[SerializeField] private FootStepAudioContainer[] footStepAudioContainers;
	[SerializeField] private LayerMask groundLayerMask;
	private readonly Dictionary<GroundMaterialType, FootStepAudioContainer> footStepAudios = new();
	private const float MaxRayDistance = 1f;
	private GroundMaterialType currentGroundType = GroundMaterialType.Default;
	private Material currentMaterial = null;
	private TerrainLayer currentLayer = null;
	private string filename;
	private void Awake()
	{
		if (transform.TryFindByName("FootStepPlayer", out FootStepPlayer) == false)
		{
			FootStepPlayer = new GameObject("FootStepPlayer", typeof(AudioSource)).transform;
			FootStepPlayer.SetParent(this.transform);
		}
		if (FootStepPlayer.TryGetComponent(out AudioSource) == false)
		{
			FootStepPlayer.gameObject.AddComponent<AudioSource>();
			AudioSource = FootStepPlayer.GetComponent<AudioSource>();
		}
		AudioSource.volume = footStepVolume;
		AudioSource.minDistance = 1f;
		AudioSource.maxDistance = 20;
		AudioSource.rolloffMode = AudioRolloffMode.Linear;
		AudioSource.spatialBlend = 1f;
		groundLayerMask = GetLayerMask.GetObstacleLayerMask;
	}
	private void Start()
	{
		foreach(var container in footStepAudioContainers)
		{
			container.CheckFootStepAudioContainer();
		}
		foreach (var container in footStepAudioContainers)
		{
			if (footStepAudios.TryAdd(container.type, container) == false)
			{
				Debug.LogWarning(container.type);
			}
		}
	}

#region Animation Event
	private void OnFootStepWalk()
	{
		currentGroundType = GetGroundMaterialType();
		if (footStepAudios.TryGetValue(currentGroundType, out FootStepAudioContainer container) == true)
			PlayFootstepClip(container.WalkSound);
	}
	private void OnFootStepRun()
	{
		currentGroundType = GetGroundMaterialType();
		if (footStepAudios.TryGetValue(currentGroundType, out FootStepAudioContainer container) == true)
			PlayFootstepClip(container.RunSound);
	}
	private void OnFootStepJump()
	{
		currentGroundType = GetGroundMaterialType();
		if (footStepAudios.TryGetValue(currentGroundType, out FootStepAudioContainer container) == true)
			PlayFootstepClip(container.JumpSound);
	}
	private void OnFootStepLand()
	{
		currentGroundType = GetGroundMaterialType();
		if (footStepAudios.TryGetValue(currentGroundType, out FootStepAudioContainer container) == true)
			PlayFootstepClip(container.LandSound);
	}
#endregion
	private void PlayFootstepClip(List<AudioClip> clips)
	{
		AudioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
		AudioSource.clip = clips[UnityEngine.Random.Range(0, clips.Count)];
		AudioSource.volume = AudioVolumeManager.FootStepVolume;
		AudioSource.Play();
	}

	private GroundMaterialType GetGroundMaterialType()
	{
		Ray ray = new(transform.position + Vector3.up * 0.5f, Vector3.down);
		if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, MaxRayDistance, groundLayerMask) == true)
		{
			if (hit.collider.TryGetComponent<Renderer>(out Renderer renderer) == true)
			{
				currentLayer = null;
				Material mat = renderer.sharedMaterial;
				if (mat == currentMaterial)
				{
					return currentGroundType;
				}
				currentMaterial = mat;
				filename = mat.name;
			}
			else if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain) == true)
			{
				currentMaterial = null;
				if (terrain.terrainData.terrainLayers.Length == 0)
				{
					return GroundMaterialType.Default;
				}
				TerrainLayer strongestLayer = GetStrongestLayer(terrain);
				if (currentLayer == strongestLayer)
				{
					return currentGroundType;
				}
				filename = strongestLayer.name;
			}
			else
			{
				currentMaterial = null;
				currentLayer = null;
				return GroundMaterialType.Default;
			}

			int underscore = filename.IndexOf('_');
			if (underscore > 0)
			{
				string terrainMaterialTypeName = filename[..underscore];
				if (Enum.TryParse(terrainMaterialTypeName, ignoreCase : true, out GroundMaterialType type) == true)
				{
					return type;
				}
			}
		}
		return GroundMaterialType.Default;
	}

	private	int[] mapUV = new int[2];
	private TerrainLayer GetStrongestLayer(Terrain terrain)
	{
		int layerIndex = 0;
		TerrainData data = terrain.terrainData;
		if (terrain.terrainData.terrainLayers.Length > 1)
		{
			Vector3 characterPosition = transform.position;
			Vector3 terrainPosition = terrain.transform.position;
			mapUV[0] = Mathf.RoundToInt((characterPosition.x - terrainPosition.x) / data.size.x * data.alphamapWidth);
			mapUV[1] = Mathf.RoundToInt((characterPosition.z - terrainPosition.z) / data.size.z * data.alphamapHeight);
			float[,,] splatMatData = data.GetAlphamaps(mapUV[0], mapUV[1], 1, 1);

			float[] cellMix = new float[splatMatData.GetUpperBound(2) + 1];
			for(int i = 0 ; i < cellMix.Length; i++)
			{
				cellMix[i] = splatMatData[0,0,i];
			}
			float strongest = 0;
			for(int i = 0 ; i < cellMix.Length ; i++)
			{
				if (cellMix[i] > strongest)
				{
					layerIndex = i;
					strongest = cellMix[i];
				}
			}
		}
		return terrain.terrainData.terrainLayers[layerIndex];
	}
}
