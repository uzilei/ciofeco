using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {
    [Header("Enemy Attributes")]
    [SerializeField] float health;
    [SerializeField] float recoilLength;
    [SerializeField] float recoilFactor;
    [SerializeField] float comboRecoilMultiplier;
    [SerializeField] float speed;
    [SerializeField] float attackTime;

    [Header("Attack Settings")]
    [SerializeField] Transform enemyAttackTransform;
    [SerializeField] Vector2 enemyAttackArea = new Vector2(1, 1);

    PlayerController player;

    bool isRecoiling = false;
    bool isAttacking = false;
    bool isDead = false;
    float recoilTimer;
    Rigidbody2D rb;
    Animator anim;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb != null) {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        StartCoroutine(AssignPlayer());
    }

    private IEnumerator AssignPlayer() {
        while (PlayerController.Instance == null) {
            yield return null;
        }
        player = PlayerController.Instance;
    }


    void Start() {
        rb.gravityScale = 4f;
    }

    void FixedUpdate() {
        if (isDead || isAttacking || player == null) return;

        if (isRecoiling) {
            HandleRecoil();
        } else {
            FollowPlayer();
        }

        PlayerHit();
        Flip();
    }

    void HandleRecoil() {
        if (recoilTimer < recoilLength) {
            recoilTimer += Time.deltaTime;
        } else {
            isRecoiling = false;
            anim.SetBool("Recoiling", false);
            recoilTimer = 0;
        }
    }

    void FollowPlayer() {
        if (player == null) {
            return;
        }

        if (!isRecoiling) {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(player.transform.position.x, transform.position.y),
                speed * Time.deltaTime
            );
        }
    }


    void Flip() {
        if (player.transform.position.x < transform.position.x) {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        } else if (player.transform.position.x > transform.position.x) {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void PlayerHit() {
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
                player.TakeDamageEnemy(1, hitDirection);
                anim.SetTrigger("Attacked");
                isAttacking = true;
                StartCoroutine(EndAttack());
            }
        }
    }

    IEnumerator EndAttack() {
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce) {
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
        rb.linearVelocity = Vector2.zero;
    }

    void EnemyDeath() {
        Destroy(gameObject);
    }

    void OnDrawGizmos() {
        if (enemyAttackTransform != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(enemyAttackTransform.position, enemyAttackArea);
        }
    }
}