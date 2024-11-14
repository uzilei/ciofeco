using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Horizontal Movement Setting")]
    private Rigidbody2D rb;

    [SerializeField] private float walkspeed = 1;

    private float xAxis;
    private float yAxis;
    private float gravity;
    
    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    [Space(5)]
    Animator anim;

    private bool canDash = true;
    private bool dashed;
    public static PlayerControll Instance;

    private bool isAttacking;
    private bool isMoving;
    private int attackCount = 0;
    private float timeSinceAttack = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Ground Check Settings")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [Header("Attacking")]
    [SerializeField] Transform FrontAttackTransform, UpAttackTransform;
    [SerializeField] Vector2 FrontAttackArea, UpAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] int damage; // Damage done to enemy on attack
    [SerializeField] int extraComboDamage;
    [SerializeField] private float comboCooldown = 1.5f;
    [SerializeField] private float comboResetTime = 1.5f;
    [SerializeField] float playerHealth;
    private bool comboCooldownActive = false;
    private enum PlayerState { Idle, Walking, Jumping, Dashing, Attacking }
    private PlayerState pState = PlayerState.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(FrontAttackTransform.position, FrontAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
    }

    void Update()
    {
        GetInputs();

        if (pState == PlayerState.Dashing) return;

        Move();
        Jump();
        Flip();
        StartDash();

        if (Input.GetKeyDown("k") && !isAttacking && !comboCooldownActive)
        {
            Attack();
        }

        // Reset attack combo logic, more efficient if in Attack() method, but more reliable here
        timeSinceAttack += Time.deltaTime;
        if (comboCooldownActive && timeSinceAttack >= comboCooldown) // Reset combo if comboCooldownActive is true and timeSinceAttack reaches comboCooldownTime
        {
            ResetCombo();
        }
        else if (!comboCooldownActive && timeSinceAttack >= comboResetTime) // Reset combo if no attack is performed within comboResetTime
        {
            ResetCombo();
        }
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }

    void Flip()
    {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-2, transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(2, transform.localScale.y);
        }
    }

    private void Move()
    {
        isMoving = xAxis != 0;

        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(walkspeed * xAxis, rb.linearVelocity.y);
            anim.SetBool("Walking", isMoving && Grounded());
        }
        else // Stops movement during attack
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); 
            anim.SetBool("Walking", false);
        }
    }

    void StartDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isAttacking)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash() // nael mi dovresti spiegare cosa fa tutta questa roba
    {
        canDash = false;
        pState = PlayerState.Dashing;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        //Collision Disability with Attackable Layer(Enemies) during the dash
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("attackable"), true); 

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState = PlayerState.Idle;
        //Collision recovery after dash
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("attackable"), false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    void Jump()
    {
        if (!isAttacking)
        {
            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            }

            if (Input.GetButtonDown("Jump") && Grounded())
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }

            anim.SetBool("Jumping", !Grounded());
        }
    }

    void Attack()
    {
        isAttacking = true;
        timeSinceAttack = 0f;
        attackCount++;
        if (attackCount == 1)
        {
            anim.SetTrigger("FrontAttacking");
            Debug.Log("Attack1");
            Hit(FrontAttackTransform, FrontAttackArea, damage);
        }
        else if (attackCount == 2)
        {
            anim.SetTrigger("FrontAttacking");
            Debug.Log("Attack2");
            Hit(FrontAttackTransform, FrontAttackArea, damage);
        }
        else if (attackCount == 3)
        {
            anim.SetTrigger("FrontAttacking");
            comboCooldownActive = true; // Activate longer cooldown only after third attack
            Debug.Log("Attack3");
            Hit(FrontAttackTransform, FrontAttackArea, damage + extraComboDamage);
        }
            StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack()
    {
        Debug.Log("EndAttack()");
        yield return new WaitForSeconds(0.4f); // Cooldown between non-combo attacks
        isAttacking = false;
        pState = PlayerState.Idle;
    }

    private void ResetCombo()
    {
        Debug.Log("ResetCombo");
        attackCount = 0;
        comboCooldownActive = false;
        timeSinceAttack = 0f;
    }

    void Hit(Transform _AttackTransform, Vector2 _AttackArea, int damageTotal)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_AttackTransform.position, _AttackArea, 0, attackableLayer);

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damageTotal, (transform.position - objectsToHit[i].transform.position).normalized, 100);
            }
        }
    }
}