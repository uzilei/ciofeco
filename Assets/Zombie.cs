using UnityEngine;

public class Zombie : Enemy
{
    [SerializeField] Transform customEnemyAttackTransform;
    [SerializeField] Vector2 customEnemyAttackArea;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        rb.gravityScale = 4f;
        EnemyAttackTransform = customEnemyAttackTransform;
        EnemyAttackArea = customEnemyAttackArea;
    }
    protected virtual void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(customEnemyAttackTransform.position, customEnemyAttackArea);
    }

    protected override void Awake() {
        base.Awake();
    }

    // Update is called once per frame
    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (!isRecoiling) {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }
    
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce) {
        base.EnemyHit(_damageDone, _hitDirection, _hitforce);
    }
}
