using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Horizontal Movement Setting")] // Reference to Rigidbody2D for physics-based movement
    private Rigidbody2D rb;

    [SerializeField] private float walkspeed = 1;

    private float xAxis; // Holds horizontal input
    private float gravity;
    private bool isAttacking;
    private float timeBetweenAttack, timeSinceAttack; // Attack timing control
    Animator anim;
    private bool canDash;//Checks whether we are dodging or not
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
    [Serialize] private float dashSpeed;
    [Serialize] private float dashTime;
    [Serialize] private float dashCooldown;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        anim = GetComponent<Animator>();

        gravity = rb.gravityScale;

    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        if (pState.dashing) return;
        Move();
        Jump();
        flip();
        Attack();
        StartDash():
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
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
        if(Input.GetKeyDown("LShift") && canDash)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (Grounded)()
        {
            dashde = false;
        }
    }
    
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashCooldown);
        rb.gravityScale = gravity;
        pState.dashing = false;
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
        if (isAttacking && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");
            Debug.Log("Attacked");
        }
    }
}
