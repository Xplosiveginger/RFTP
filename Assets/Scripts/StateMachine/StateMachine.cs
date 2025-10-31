public class StateMachine
{
    public EntityState CurrentState { get; private set; }

    public void Initilize(EntityState state)
    {
        CurrentState = state;
        CurrentState.Enter();
    }

    public void ChangeState(EntityState newstate)
    {
        CurrentState.Exit();
        CurrentState = newstate;
        CurrentState.Enter();
    }

    public void UpdateActiveState()
    {
        CurrentState.Update();
    }
}