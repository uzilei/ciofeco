using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour {
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected float comboRecoilMultiplier;
    [SerializeField] protected bool isRecoiling = false;
    [SerializeField] protected PlayerControll player;
    [SerializeField] protected float speed;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected virtual Transform EnemyAttackTransform { get; set; }
    protected virtual Vector2 EnemyAttackArea { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // public virtual void Start() {}

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerControll.Instance;
        if (rb != null) {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
        if(health <= 0) {
            Destroy(gameObject);
        }

        if(isRecoiling) {
            if(recoilTimer < recoilLength) {
                recoilTimer += Time.deltaTime;
            }
            else {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        PlayerHit();
    }

    protected void PlayerHit() {
        if (EnemyAttackTransform == null || EnemyAttackArea == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            EnemyAttackTransform.position,
            EnemyAttackArea,
            0f,
            LayerMask.GetMask("Player")
        );

        if (hit != null) {
            Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
            PlayerControll player = hit.GetComponent<PlayerControll>();
            if (player != null) {
                player.TakeDamage(1, hitDirection);
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce) {
        health -= _damageDone;
        float finalHitForce = _hitforce;

        if (player != null && player.attackCount == 3) {
            finalHitForce *= comboRecoilMultiplier;
        }

        rb.AddForce(-finalHitForce * recoilFactor * _hitDirection);

        isRecoiling = true;
        recoilTimer = 0f;
        rb.linearVelocity = Vector2.zero; // Stop movement
    }
}
