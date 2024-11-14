using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Horizontal Movement Setting")] // Reference to Rigidbody2D for physics-based movement
    private Rigidbody2D rb;

    [SerializeField] private float walkspeed = 1;

    private float xAxis; // Holds horizontal input
    private float yAxis; // Holds vertical input
    private float gravity;
    Animator anim;

    private bool canDash = true; // Checks whether we are dodging or not
    private bool dashed;
    public static PlayerControll Instance; // Singleton instance for easy access

    private void Awake() // Singleton pattern to ensure only one instance exists
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
    [SerializeField] private Transform groundCheckPoint; // Check ground raycast
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [Header("Attacking")]
    private bool isAttacking;
    private float timeSinceAttack; // Attack timing control
    [SerializeField] Transform FrontAttackTransform, UpAttackTransform;
    [SerializeField] Vector2 FrontAttackArea, UpAttackArea;
    [SerializeField] LayerMask attackableLayer;

    // Enum per lo stato del giocatore
    private enum PlayerState { Idle, Walking, Jumping, Dashing }
    private PlayerState pState = PlayerState.Idle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;

    }

    void OnDrawGizmos() // Display attack areas in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(FrontAttackTransform.position, FrontAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        if (pState == PlayerState.Dashing) return;
        Move();
        Jump();
        flip();
        Attack();
        StartDash();

    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        isAttacking = Input.GetKeyDown("k");
    }

    void flip()
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
        rb.linearVelocity = new Vector2(walkspeed * xAxis, rb.linearVelocity.y);
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Debug.Log("Dash iniziato");
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
        Debug.Log("Dentro il coroutine Dash");
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState = PlayerState.Idle;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    void Jump()
    {

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }

        if (Input.GetButtonDown("Jump") && Grounded())
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce);
        }

        anim.SetBool("Jumping", !Grounded());
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (isAttacking && timeSinceAttack >= 1) // Number is attack cooldown in seconds
        {
            timeSinceAttack = 0;

            if (yAxis == 0 || yAxis < 0 && Grounded()) // Define attack axis
            {
                Hit(FrontAttackTransform, FrontAttackArea);
                anim.SetTrigger("FrontAttacking");
                Debug.Log("FrontAttacked");
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea);
                anim.SetTrigger("UpAttacking");
                Debug.Log("UpAttacked");
            }
        }
    }
    void Hit(Transform _AttackTransform, Vector2 _AttackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_AttackTransform.position, _AttackArea, 0, attackableLayer);
        Debug.Log(objectsToHit.Length);
    }
}
