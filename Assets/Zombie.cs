using UnityEngine;

public class Zombie : Enemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.gravityScale = 12f;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerControll.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }
    
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitforce);
    }
}
