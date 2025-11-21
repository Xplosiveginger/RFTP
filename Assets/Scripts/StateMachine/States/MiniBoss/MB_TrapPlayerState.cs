using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_TrapPlayerState : EntityState
{
    MiniBoss miniboss;
    Vector3[] corners;
    int currentCorner = 0;
    bool attackFinished = false;

    public MB_TrapPlayerState(MiniBoss miniBoss, StateMachine stateMachine, Animator animator, string animBoolName) : base(stateMachine, animator, animBoolName)
    {
        miniboss = miniBoss;
    }

    public override void Enter()
    {
        base.Enter();
        currentCorner = 0;
        attackFinished = false;
        miniboss.ai.enabled = false;
        miniboss.ai.agent.speed = miniboss.chargeSpeed;

        FindRectangleCorners();
    }

    public override void Update()
    {
        base.Update();

        if (miniboss.obstacleDetected)
            stateMachine.ChangeState(miniboss.followPlayerState);

        attackFinished = TrapPlayer();
        if (attackFinished)
            stateMachine.ChangeState(miniboss.followPlayerState);
    }

    public override void Exit()
    {
        base.Exit();
        miniboss.toxicTrail.Stop();
        miniboss.ai.agent.speed = miniboss.ai.moveSpeed;
        miniboss.ai.enabled = true;
    }

    private void FindRectangleCorners()
    {
        Vector3 targetPos = miniboss.Player.transform.position;

        corners = new Vector3[5];
        corners[0] = targetPos + new Vector3(-miniboss.trapRectangleSize.x / 2, miniboss.trapRectangleSize.y / 2);
        corners[1] = targetPos + new Vector3(miniboss.trapRectangleSize.x / 2, miniboss.trapRectangleSize.y / 2);
        corners[2] = targetPos + new Vector3(miniboss.trapRectangleSize.x / 2, -miniboss.trapRectangleSize.y / 2);
        corners[3] = targetPos + new Vector3(-miniboss.trapRectangleSize.x / 2, -miniboss.trapRectangleSize.y / 2);
        corners[4] = targetPos + new Vector3(-miniboss.trapRectangleSize.x / 2, miniboss.trapRectangleSize.y / 2);
    }

    private bool TrapPlayer()
    {
        if (!miniboss.ai.agent.pathPending && miniboss.ai.agent.remainingDistance <= miniboss.ai.agent.stoppingDistance)
        {
            if(currentCorner == 0)
                miniboss.toxicTrail.Play();
            currentCorner++;
        }

        if(currentCorner >= 5)
        {
            return true;
        }

        Vector3 target = corners[currentCorner];
        miniboss.ai.agent.SetDestination(target);

        return false;
    }
}