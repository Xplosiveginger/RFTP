using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_pentTrapPlayerState : EntityState
{
    MiniBoss miniboss;
    Vector3[] corners;
    int currentCorner = 0;
    bool attackFinished = false;

    public MB_pentTrapPlayerState(MiniBoss miniBoss, StateMachine stateMachine, Animator animator, string animBoolName) : base(stateMachine, animator, animBoolName)
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

        FindPentagonCorners();
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
        miniboss.ai.agent.speed = miniboss.ai.moveSpeed;
        miniboss.ai.enabled = true;
    }

    private void FindPentagonCorners()
    {
        Vector3 targetPos = miniboss.Player.transform.position;

        corners = new Vector3[6];

        for (int i = 0; i < 5; i++)
        {
            float angleDeg = i * 72f;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * miniboss.trapCircleRadius;
            corners[i] = targetPos + offset;
            if (i == 0) { corners[5] = corners[i]; }
        }
    }

    private bool TrapPlayer()
    {
        if (!miniboss.ai.agent.pathPending && miniboss.ai.agent.remainingDistance <= miniboss.ai.agent.stoppingDistance)
        {
            Debug.Log("Reached Location");
            currentCorner++;
            Debug.Log($"Corner no {currentCorner}");
        }

        if (currentCorner >= 6)
        {
            return true;
        }

        Vector3 target = corners[currentCorner];
        miniboss.ai.agent.SetDestination(target);

        return false;
    }
}