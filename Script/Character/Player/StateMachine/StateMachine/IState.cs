using UnityEngine;

public interface IState
{
	public void Enter();
	public void Tick();
	public void FixedTick();
	public void Exit();
}