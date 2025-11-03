using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_circTrapPlayerState : EntityState
{
    MiniBoss miniboss;
    bool attackFinished = false;
    private Vector3 circleCenter;
    float radius;
    float angle;
    float fullRotation;

    public MB_circTrapPlayerState(MiniBoss miniBoss, StateMachine stateMachine, Animator animator, string animBoolName) : base(stateMachine, animator, animBoolName)
    {
        miniboss = miniBoss;
    }

    public override void Enter()
    {
        base.Enter();
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

        TrapPlayer();
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
        circleCenter = miniboss.Player.transform.position;
        radius = miniboss.trapCircleRadius;

        angle = 0f;
        fullRotation = 0f;
    }

    private void TrapPlayer()
    {
        angle += miniboss.circleAngularSpeed * Time.deltaTime;  // deg/sec
        fullRotation += miniboss.circleAngularSpeed * Time.deltaTime;

        if (fullRotation >= 360f)
        {
            attackFinished = true;
            return;
        }

        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        Vector3 targetPosition = circleCenter + offset;

        miniboss.ai.agent.SetDestination(targetPosition);
        miniboss.toxicTrail.Play();
    }
}