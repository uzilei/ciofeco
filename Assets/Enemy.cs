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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // public virtual void Start() {}

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerControll.Instance;
    }

    // Update is called once per frame
    protected virtual void Update() {
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
    }
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce) {
        health -= _damageDone;
        float finalHitForce = _hitforce;
        if (player != null && player.attackCount == 3) {
            finalHitForce *= comboRecoilMultiplier;
        }
        rb.AddForce(-finalHitForce * recoilFactor * _hitDirection);
        if(!isRecoiling) {
            isRecoiling = true;
            rb.linearVelocity = Vector2.zero; // Stop movement
        }
    }
}
