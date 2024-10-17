using System;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
	public static SequenceManager Instance;
	[SerializeField] private Sequence[] levelSequences;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Debug.Log("[Level Manager] : Already Exist");
			Destroy(gameObject);
		}
	}
	public bool RegisterSequencePlayEvent(int index, Action action)
	{
		if (index >= levelSequences.Length)
			return false;

		if (index < 0)
		{
			// print("[Level Manager] : Registering default Play Event");
			foreach(Sequence seq in levelSequences)
				seq.RegisterOnDirectorPlayEvent(action);
		}
		else
		{
			// print($"[Level Manager] : Registering {index} Play Event");
			levelSequences[index].RegisterOnDirectorPlayEvent(action);
		}
		return true;
	}
	public bool RegisterSequenceFinishEvent(int index, Action action)
	{
		if (index >= levelSequences.Length)
			return false;

		if (index < 0)
		{
			// print("[Level Manager] : Registering default Finish Event");
			foreach(Sequence seq in levelSequences)
				seq.RegisterOnDirectorFinishEvent(action);
		}
		else
		{
			// print($"[Level Manager] : Registering {index} Finish Event");
			levelSequences[index].RegisterOnDirectorFinishEvent(action);
		}
		return true;
	}
}
