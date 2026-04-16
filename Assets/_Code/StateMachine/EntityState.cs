using UnityEngine;

public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected Animator animator;
    protected string animBoolName;

    public EntityState(StateMachine stateMachine, Animator animator, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animator = animator;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        if(animator != null)
            animator.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {

    }

    public virtual void Exit()
    {
        if (animator != null)
            animator.SetBool(animBoolName, false);
    }
}