using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBoss : BaseEnemy
{
    public GameObject Player { get; private set; }
    public Animator animator { get; private set; }
    HealthSystem playerHealth;
    [Space]
    [Header("Setup")]
    public float chargeSpeed = 5f;
    public float circleAngularSpeed = 5f;
    public Rigidbody2D rb;
    public Vector2 trapRectangleSize = new Vector2(5, 3);
    public float trapCircleRadius = 5f;
    public float cooldown = 5f;

    public EnemyAI ai;
    public Transform obstacleCheckTransform;
    public Vector2 obstacleCheckSize;
    public LayerMask obstacleLayerMask;
    public ParticleSystem toxicTrail;
    public float decisionInterval = 5f;
    private float lastDecisionTime;

    public float trapCooldown = 5f;
    public float chargeCooldown = 5f;

    [HideInInspector] public float lastTrapTime;
    [HideInInspector] public float lastChargeTime;

    StateMachine statemachine;
    [field: SerializeField, ] public bool playerInRange { get; set; }
     public bool charging = false;
     public bool trap = false;
     public bool obstacleDetected = false;
    [SerializeField, ] private bool onCooldown = false;

    public EntityState currState;

    public MB_FollowPlayerState followPlayerState { get; private set; }
    public ChargedAttack_State chargedAttackState { get; private set; }
    public MB_TrapPlayerState trapPlayerState { get; private set; }
    public MB_pentTrapPlayerState pentTrapPlayerState { get; private set; }
    public MB_circTrapPlayerState circTrapPlayerState { get; private set; }

    private void Awake()
    {
        statemachine = new StateMachine();
        ai = GetComponent<EnemyAI>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");

        followPlayerState = new MB_FollowPlayerState(this, statemachine, animator, "followPlayer");
        chargedAttackState = new ChargedAttack_State(this, statemachine, animator, "chargedAttack");
        trapPlayerState = new MB_TrapPlayerState(this, statemachine, animator, "trapPlayer");
        pentTrapPlayerState = new MB_pentTrapPlayerState(this, statemachine, animator, "trapPlayer");
        circTrapPlayerState = new MB_circTrapPlayerState(this, statemachine, animator, "trapPlayer");
    }

    private void Start()
    {
        statemachine.Initilize(followPlayerState);
    }

    private void Update()
    {
        FacePlayer();

        obstacleDetected = Physics2D.OverlapBox(obstacleCheckTransform.position, obstacleCheckSize, 0, obstacleLayerMask);

        statemachine.UpdateActiveState();

        if (Time.time - lastDecisionTime >= decisionInterval)
        {
            PickNextState();
            lastDecisionTime = Time.time;
        }
        

        currState = statemachine.CurrentState;

        DebugCurrState();
    }

    protected override void DoDamageToPlayerOnCollision(Collision2D player)
    {
        this.Player = player.gameObject;
        playerHealth = this.Player.GetComponent<HealthSystem>();

        playerHealth.Damage((int)damageOnContact);
    }

    private void DebugCurrState()
    {
        if (currState == followPlayerState)
            Debug.Log("Follow Player State");
        else if (currState == chargedAttackState)
            Debug.Log("Charged Attack State");
        else if (currState == trapPlayerState)
            Debug.Log("Trap Player State");
    }

    void FacePlayer()
    {
        if (Player.transform.position.x < transform.position.x)
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void PickNextState()
    {
        if (!playerInRange)
            return;

        bool trapReady = Time.time - lastTrapTime >= trapCooldown;
        bool chargeReady = Time.time - lastChargeTime >= chargeCooldown;

        int choice = 2;
        if (trapReady && chargeReady)
        {
            choice = Random.Range(0, 3); // 0 = trap, 1 = charge, 2 = follow
        }
        else if (trapReady)
        {
            choice = Random.Range(0, 2) == 0 ? 0 : 2;
        }
        else if (chargeReady)
        {
            choice = Random.Range(0, 2) == 0 ? 1 : 2;
        }

        switch (choice)
        {
            case 0:
                charging = false;
                trap = true;
                lastTrapTime = Time.time;
                break;
            case 1:
                trap = false;
                charging = true;
                lastChargeTime = Time.time;
                break;
            case 2:
                charging = false;
                trap = false;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(obstacleCheckTransform.position, obstacleCheckSize);
    }
}
