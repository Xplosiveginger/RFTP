using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_FollowPlayerState : EntityState
{
    MiniBoss miniboss;

    public MB_FollowPlayerState(MiniBoss miniboss, StateMachine stateMachine, Animator animator, string animBoolName) : base(stateMachine, animator, animBoolName)
    {
        this.miniboss = miniboss;
    }

    public override void Update()
    {
        base.Update();

        if (miniboss.charging)
        {
            stateMachine.ChangeState(miniboss.circTrapPlayerState);
        }
    }
}
