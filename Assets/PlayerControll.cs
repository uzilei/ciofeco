using UnityEngine;

public class PlayerControll : MonoBehaviour
{
    [Header("Horizontal Movement Setting")]
    private Rigidbody2D rb;

    [SerializeField] private float walkspeed = 1;

    private float xAxis;
    private bool isAttacking;
    private float timeBetweenAttack, timeSinceAttack;
    Animator anim;

    public static PlayerControll Instance;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        GetInputs();
        Move();
        Jump();
        flip();
        Attack();
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
