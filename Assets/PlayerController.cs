using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Horizontal Movement Setting")]
    private Rigidbody2D rb;

    [SerializeField] public float walkspeed;

    private float xAxis;
    private float yAxis;
    private float gravity;

    [Header("Health Settings")]
    public int health;
    public int maxHealth;
    public int healingCooldown;
    public int heals;
    private int healingAmount = 5;


    //Life bar management
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate OnHealthChangedCallBack;
    [Space(5)]

    Animator anim;
    private bool canDash = true;
    public static PlayerController Instance;
    public int attackCount = 0;
    private float timeSinceAttack = 0f;
    private float timeSinceHeal = 0f;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } 
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    [Header("Attack Settings")]
    [SerializeField] Transform FrontAttackTransform, UpAttackTransform;
    [SerializeField] Vector2 FrontAttackArea, UpAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] int damage;
    [SerializeField] int extraComboDamage;
    [SerializeField] private float comboCooldown;
    [SerializeField] private float comboResetTime;
    private bool comboCooldownActive = false;

    [Header("Knockback and iFrames Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.1f;
    [SerializeField] private float iFrameDuration = 1f;
    private float iFrameTimer = 0f;
    private float knockbackTimer = 0f;

    private enum PlayerState {Idle, Moving, Jumping, Falling, Dashing, Attacking, Healing, Dead, KnockedBack}
    private PlayerState pState = PlayerState.Idle;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(FrontAttackTransform.position, FrontAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
    }

    void Update() {
        if (pState == PlayerState.Dashing || pState == PlayerState.Dead || pState == PlayerState.Attacking || pState == PlayerState.Healing) return;
    
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump")) {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) {
            StartCoroutine(Dash());
        }

        if (Input.GetKeyDown("k")) {
            if (yAxis > 0 && Grounded()) {
                UpAttack();
            }
            else if (!Grounded()){
                AirAttack();
            }
            else {
                Attack();
            }
        }

        if (Input.GetKeyDown("q")) {
            Heal();
        }
    }

    void FixedUpdate() {
        timeSinceAttack += Time.deltaTime;
        timeSinceHeal += Time.deltaTime;
    if (pState == PlayerState.Dashing || pState == PlayerState.Dead || pState == PlayerState.Attacking || pState == PlayerState.Healing) return;

        if (comboCooldownActive && timeSinceAttack >= comboCooldown) {
            ResetCombo();
        }
        else if (!comboCooldownActive && timeSinceAttack >= comboResetTime) {
            ResetCombo();
        }

        if (iFrameTimer > 0) {
            iFrameTimer -= Time.deltaTime;
        }

        if (knockbackTimer > 0) {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0) {
                pState = PlayerState.Idle;
            }
            return;
        }

        if (rb.linearVelocity.y < 0 && !Grounded() && pState != PlayerState.Attacking) {
            pState = PlayerState.Falling;
            anim.SetBool("Falling", true);
        }
        else {
            pState = PlayerState.Idle;
            anim.SetBool("Falling", false);
        }
        
        Move();
        if (pState != PlayerState.Attacking) {
            Flip(); 
        }
    }

    void Flip() {
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    private void Move() {
        if (pState != PlayerState.Attacking) {
            if (xAxis != 0) {
                pState = PlayerState.Moving;
                rb.linearVelocity = new Vector2(walkspeed * xAxis, rb.linearVelocity.y);
                anim.SetBool("Moving", Grounded());
            }
            else {
                pState = PlayerState.Idle;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                anim.SetBool("Moving", false);
            }
        }
        else {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y); // Stops movement during attack
            anim.SetBool("Moving", false);
        }
    }

    IEnumerator Dash() {
        canDash = false;
        pState = PlayerState.Dashing;
        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("attackable"), true);

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState = PlayerState.Idle;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("attackable"), false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public bool Grounded() {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    void Jump() {
        if (pState != PlayerState.Attacking) {
            if (Grounded() && pState != PlayerState.Attacking) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("Jumping");
            pState = PlayerState.Jumping;
            }
        }
    }

    void Attack() {
        if (pState == PlayerState.Attacking || comboCooldownActive) return;

        pState = PlayerState.Attacking;
        timeSinceAttack = 0f;
        attackCount++;

        if (attackCount == 1) {
            anim.SetTrigger("Attack1");
            Debug.Log("Attack1");
            Hit(FrontAttackTransform, FrontAttackArea, damage);
        }
        else if (attackCount == 2) {
            anim.SetTrigger("Attack2");
            Debug.Log("Attack2");
            Hit(FrontAttackTransform, FrontAttackArea, damage);
        }
        else if (attackCount == 3) {
            anim.SetTrigger("Attack3");
            comboCooldownActive = true;
            Debug.Log("Attack3");
            Hit(FrontAttackTransform, FrontAttackArea, damage + extraComboDamage);
        }
        StartCoroutine(EndAttack());
    }

    void AirAttack() {
        if (pState == PlayerState.Attacking) return;
        pState = PlayerState.Attacking;
        timeSinceAttack = 0f;
        comboCooldownActive = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        anim.SetTrigger("AttackAir");
        Debug.Log("AttackAir");
        rb.gravityScale = 0;
        Hit(FrontAttackTransform, FrontAttackArea, damage + extraComboDamage);
        StartCoroutine(EndAttack());
    }
    void UpAttack() {
        if (pState == PlayerState.Attacking) return;
        pState = PlayerState.Attacking;
        timeSinceAttack = 0f;
        comboCooldownActive = true;
        anim.SetTrigger("AttackUp");
        Debug.Log("AttackUp");
        Hit(UpAttackTransform, UpAttackArea, damage + extraComboDamage);
        StartCoroutine(EndAttack());
    }

    IEnumerator EndAttack() {
        yield return new WaitForSeconds(0.66f); // Cooldown between non-combo attacks
        pState = PlayerState.Idle;
        rb.gravityScale = gravity;
    }

    private void ResetCombo() {
        attackCount = 0;
        comboCooldownActive = false;
        timeSinceAttack = 0f;
    }

    void Hit(Transform _AttackTransform, Vector2 _AttackArea, int damageTotal) {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_AttackTransform.position, _AttackArea, 0, attackableLayer);

        for (int i = 0; i < objectsToHit.Length; i++) {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damageTotal, (transform.position - objectsToHit[i].transform.position).normalized, 100);
            }
        }
    }
    public void TakeDamage(int damage, Vector2 hitDirection) {
        if (iFrameTimer > 0f) {
            return;
        }

        if (pState == PlayerState.Dashing) {
            Debug.Log("Dash invincibility, no damage");
            return;
        }

        health -= damage;
        iFrameTimer = iFrameDuration;

        if (health <= 0) {
            health = 0;
            pState = PlayerState.Dead;
            anim.SetTrigger("Dead");
        }
        else {
            ApplyKnockback(hitDirection);
            Debug.Log($"Player took {damage} damage, Current health: {health}");
        }
    }

    public void ApplyKnockback(Vector2 direction) {
        pState = PlayerState.KnockedBack;
        knockbackTimer = knockbackDuration;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }

    public void ExitGame() {
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // This line is for testing in the Unity editor
    #endif
    }

    private void Heal() {
        if (timeSinceHeal <= healingCooldown) {
            Debug.Log("Time since last heal too short!");
            Debug.Log(timeSinceHeal);
            return;
        }
        else {
            timeSinceHeal = 0f;
            health += healingAmount;
            if (health > maxHealth) {
                health = maxHealth;
            }
            Debug.Log($"Player healed, Current health: {health}");
            pState = PlayerState.Healing;
            anim.SetTrigger("Healing");
        }
    }

    private void endHeal() {
        // Called by animation
        pState = PlayerState.Idle;
        Debug.Log("End heal");
    }
}