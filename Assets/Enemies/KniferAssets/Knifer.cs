using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Enemy Attributes")]
    [SerializeField] protected float health = 10f;
    [SerializeField] protected float recoilLength = 0.5f;
    [SerializeField] protected float recoilFactor = 10f;
    [SerializeField] protected float comboRecoilMultiplier = 2f;
    [SerializeField] protected float speed = 2f;

    [Header("Attack Settings")]
    [SerializeField] protected Transform enemyAttackTransform;
    [SerializeField] protected Vector2 enemyAttackArea = new Vector2(1, 1);

    [Header("References")]
    [SerializeField] protected PlayerController player;

    protected bool isRecoiling = false;
    protected bool isDead = false;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected Animator anim;

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        StartCoroutine(AssignPlayer());
    }

    private IEnumerator AssignPlayer() {
        while (PlayerController.Instance == null) {
            yield return null; // Wait for PlayerController to be instantiated
        }
        player = PlayerController.Instance;
        Debug.Log("Player assigned successfully.");
    }


    protected virtual void Start() {
        rb.gravityScale = 4f; // Default gravity scale for grounded enemies
    }

    protected virtual void FixedUpdate() {
        if (isDead) return;

        if (isRecoiling) {
            HandleRecoil();
        } else {
            FollowPlayer();
        }

        PlayerHit();
        Flip();
    }

    protected virtual void HandleRecoil() {
        if (recoilTimer < recoilLength) {
            recoilTimer += Time.deltaTime;
        } else {
            isRecoiling = false;
            anim.SetBool("Recoiling", false);
            recoilTimer = 0;
        }
    }

    protected virtual void FollowPlayer() {
        if (player == null) {
            return; // Exit the method if player is not set
        }

        if (!isRecoiling) {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(player.transform.position.x, transform.position.y),
                speed * Time.deltaTime
            );
        }
    }


    protected virtual void Flip() {
        if (player.transform.position.x < transform.position.x) {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        } else if (player.transform.position.x > transform.position.x) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    protected void PlayerHit() {
        if (enemyAttackTransform == null || enemyAttackArea == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            enemyAttackTransform.position,
            enemyAttackArea,
            0f,
            LayerMask.GetMask("Player")
        );

        if (hit != null) {
            Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null) {
                player.TakeDamage(1, hitDirection);
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce) {
        if (isDead) return;
        health -= _damageDone;
        float finalHitForce = _hitforce;

        if (player.attackCount == 3) {
            finalHitForce *= comboRecoilMultiplier;
        }

        if (health <= 0) {
            isDead = true;
            anim.SetTrigger("Death");
        }

        rb.AddForce(-finalHitForce * recoilFactor * _hitDirection);

        isRecoiling = true;
        anim.SetBool("Recoiling", true);
        recoilTimer = 0f;
        rb.linearVelocity = Vector2.zero; // Stop movement
    }

    protected virtual void EnemyDeath() {
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmos() {
        if (enemyAttackTransform != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(enemyAttackTransform.position, enemyAttackArea);
        }
    }
}
