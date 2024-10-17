

using UnityEngine;

public abstract class StateMachine
{
	public IState currentState { get; protected set; } = null;
	public virtual void Tick()
	{
		currentState?.Tick();
	}

	public virtual void FixedTick()
	{
		currentState?.FixedTick();
	}
	protected void ChangeState(IState newState)
	{
		if (currentState is AIDeadState)
		{
			Debug.Log("IS Already Dead");
			return ;
		}
		if (currentState == newState)
			return ;
		currentState?.Exit();
		currentState = newState;
		currentState.Enter();
	}
}
