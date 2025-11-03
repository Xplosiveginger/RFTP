using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargedAttack_State : EntityState
{
    MiniBoss miniBoss;
    Vector3 target;
    Vector3 dir;

    public ChargedAttack_State(MiniBoss miniBoss, StateMachine stateMachine, Animator animator, string animBoolName) : base(stateMachine, animator, animBoolName)
    {
        this.miniBoss = miniBoss;
    }

    public override void Enter()
    {
        base.Enter();
        miniBoss.ai.enabled = false;

        dir = FindDirectionToPlayer();
        miniBoss.rb.velocity = miniBoss.chargeSpeed * dir;
    }

    public override void Update()
    {
        base.Update();

        Debug.Log(Vector3.Distance(miniBoss.transform.position, target));

        if(miniBoss.obstacleDetected)
            stateMachine.ChangeState(miniBoss.followPlayerState);

        if (Vector3.Distance(miniBoss.transform.position, target) < 3.39f)
            stateMachine.ChangeState(miniBoss.followPlayerState);
    }

    private Vector3 FindDirectionToPlayer()
    {
        target = miniBoss.Player.transform.position;
        Vector3 dirToPlayer = (miniBoss.Player.transform.position - miniBoss.gameObject.transform.position).normalized;
        return dirToPlayer;
    }

    public override void Exit()
    {
        base.Exit();
        miniBoss.rb.velocity = Vector2.zero;
        miniBoss.ai.enabled = true;
    }
}