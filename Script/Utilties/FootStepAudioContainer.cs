using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FootStepAudioContainer", menuName = "FootStepAudioContainer")]
public class FootStepAudioContainer : ScriptableObject
{
	public GroundMaterialType type;
	public List<AudioClip> WalkSound;
	public List<AudioClip> RunSound;
	public List<AudioClip> JumpSound;
	public List<AudioClip> LandSound;
	public void CheckFootStepAudioContainer()
	{
		int[] nullCounts = new int[4];

		CheckUnassignedAudioClip(WalkSound, nullCounts, 0);
		CheckUnassignedAudioClip(RunSound, nullCounts, 1);
		CheckUnassignedAudioClip(JumpSound, nullCounts, 2);
		CheckUnassignedAudioClip(LandSound, nullCounts, 3);

		Debug.Assert(nullCounts[0] == 0, $"[FootStepSound] : {nullCounts[0]} unassigned clip in WalkSound");
		Debug.Assert(nullCounts[1] == 0, $"[FootStepSound] : {nullCounts[1]} unassigned clip in RunSound");
		Debug.Assert(nullCounts[2] == 0, $"[FootStepSound] : {nullCounts[2]} unassigned clip in JumpSound");
		Debug.Assert(nullCounts[3] == 0, $"[FootStepSound] : {nullCounts[3]} unassigned clip in LandSound");
	}

	private void CheckUnassignedAudioClip(List<AudioClip> clips, int[] nullCounts, int index)
	{
		if (clips.Count == 0)
		{
			Debug.Assert(false, $"AudioClips Not Found");
			return ;
		}
		foreach (var clip in clips)
		{
			if (clip == null)
				nullCounts[index]++;
		}
	}
}