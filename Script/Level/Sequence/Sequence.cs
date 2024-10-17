using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class Sequence : MonoBehaviour
{
	protected enum RepeatType { Once, EveryTime };
	[SerializeField] protected RepeatType repeatType = RepeatType.Once;
	protected bool isPlayed = false;
	public event Action OnDirectorPlay;
	public event Action OnDirectorFinish;
	[SerializeField] private PlayableDirector director;
	public void RegisterOnDirectorPlayEvent(Action action)
	{
		OnDirectorPlay += action;
	}

	public void RegisterOnDirectorFinishEvent(Action action)
	{
		OnDirectorFinish += action;
	}
	public virtual void PlayDirector()
	{
		if (repeatType == RepeatType.Once && isPlayed == true)
		{
			return ;
		}
		OnDirectorPlay?.Invoke();
		director.Play();
		StartCoroutine(FinishDirector((float)director.duration));
		isPlayed = true;
	}
	public void FinishDirector()
	{
		OnDirectorFinish?.Invoke();
	}
	private IEnumerator FinishDirector(float time)
	{
		if (director.timeUpdateMode == DirectorUpdateMode.GameTime)
			yield return new WaitForSeconds(time);
		else if (director.timeUpdateMode == DirectorUpdateMode.UnscaledGameTime)
			yield return new WaitForSecondsRealtime(time);
		OnDirectorFinish?.Invoke();
	}
}
