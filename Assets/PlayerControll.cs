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
    Animator anim;

    private bool canDash = true;
    private bool dashed;
    public static PlayerControll Instance;

    private bool isAttacking; // Indica se l'attacco è attivo
    private float timeSinceAttack;
    private bool isMoving; // Indica se il giocatore si sta muovendo

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
    [SerializeField] float damage; // Damage done to enemy on attack
    [SerializeField] float playerHealth;

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

        if (pState == PlayerState.Dashing || pState == PlayerState.Attacking) return;

        Move();
        Jump();
        Flip();
        StartDash();
        Attack();
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");

        if (!isAttacking)
        {
            isAttacking = Input.GetKeyDown("k");
        }
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

        if (!isAttacking) // Blocca il movimento durante l'attacco
        {
            rb.linearVelocity = new Vector2(walkspeed * xAxis, rb.linearVelocity.y);
            anim.SetBool("Walking", isMoving && Grounded());
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Ferma il movimento
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

    IEnumerator Dash()
    {
        canDash = false;
        pState = PlayerState.Dashing;
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState = PlayerState.Idle;
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
        timeSinceAttack += Time.deltaTime;

        if (isAttacking && timeSinceAttack >= 1)
        {
            timeSinceAttack = 0;
            anim.SetBool("IsAttacking", true);
            pState = PlayerState.Attacking; // Cambia lo stato per evitare altre animazioni

            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                Hit(FrontAttackTransform, FrontAttackArea);
                anim.SetTrigger("FrontAttacking");
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea);
                anim.SetTrigger("UpAttacking");
            }

            StartCoroutine(EndAttack());
        }
    }

    IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("IsAttacking", false);
        isAttacking = false;
        pState = PlayerState.Idle; // Ritorna allo stato Idle al termine
    }

    void Hit(Transform _AttackTransform, Vector2 _AttackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_AttackTransform.position, _AttackArea, 0, attackableLayer);

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage);
            }
        }
    }
}
