using System;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusic : MonoBehaviour
{
	[SerializeField] private float volumeDampRate = 1f;
	[SerializeField] private bool playOnStart = false;
	[SerializeField] private List<AudioSource> audioSources;
	private AudioSource currentPlayingSource;
	private AudioSource fadingSource;
	private readonly Dictionary<string, AudioSource> musics = new();
	[SerializeField] private Stack<string> musicStack = new();
	[SerializeField] private List<string> temp;
#region Monobehaviour
	private void Awake()
	{
		foreach(AudioSource source in audioSources)
		{
			source.volume = 0.01f;
			musics.Add(source.name, source);
		}
	}
	private void OnEnable()
	{
		musicStack.Clear();
		temp.Clear();
		if (audioSources.Count > 0)
		{
			PushTrack(audioSources[0].name);
			if (playOnStart == true)
			{
				Play();
			}
		}
	}
	private void Update()
	{
		if (currentPlayingSource != null)
		{
			float volume = AudioVolumeManager.BackGroundMusicVolume;
			currentPlayingSource.volume = Mathf.Lerp(currentPlayingSource.volume, volume, volumeDampRate * Time.unscaledDeltaTime);
		}
		if (fadingSource != null)
		{
			fadingSource.volume = Mathf.Lerp(fadingSource.volume, 0, volumeDampRate * Time.unscaledDeltaTime);
			if (Mathf.Approximately(fadingSource.volume, 0) == true)
			{
				fadingSource.Stop();
				fadingSource = null;
			}
		}
	}
#endregion
#region  Main Methode
	public void PushTrack(string name)
	{
		if (musics.ContainsKey(name) == true)
		{
			musicStack.Push(name);
			temp.Add(name);
			SwitchMusic(name);
		}
	}
	public void PopTrack()
	{
		if (musicStack.Count > 1)
		{
			musicStack.Pop();
			temp.RemoveAt(temp.Count - 1);
		}
		SwitchMusic(musicStack.Peek());
	}
#endregion
	private void Play()
	{
		if (currentPlayingSource != null)
			currentPlayingSource.Play();
	}

	private void Stop()
	{
		foreach(AudioSource source in audioSources)
		{
			source.Stop();
		}
	}

	private void SwitchMusic(string name)
	{
		if(musics.TryGetValue(name, out AudioSource source) == true)
		{
			if (fadingSource != null)
			{
				fadingSource.volume = 0;
				fadingSource.Stop();
			}
			fadingSource = currentPlayingSource;
			currentPlayingSource = source;
			if (currentPlayingSource.isPlaying == false)
			{
				currentPlayingSource.Play();
			}
		}
	}
}
