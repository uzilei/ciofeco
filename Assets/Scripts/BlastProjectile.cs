using UnityEngine;

public class BlastProjectile : MonoBehaviour {
    [Header("Blast Settings")]
    [SerializeField] private float blastSpeed;
    [SerializeField] private int blastDamage;
    [SerializeField] private float blastLifetime;

    [Header("Collision Detection")]
    [SerializeField] private Transform blastHitBox;
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1f, 1f);
    [SerializeField] private LayerMask collisionLayers;

    private float timeAlive = 0f;
    private bool hasCollided = false;
    private Animator anim;

    void Start() {
        Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        transform.up = -direction;
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        if (hasCollided)
            return;
        transform.Translate(Vector3.down * blastSpeed * Time.deltaTime);

        CheckCollisions();

        timeAlive += Time.deltaTime;
        if (timeAlive >= blastLifetime) {
            Destroy(gameObject);
        }
    }

    private void CheckCollisions() {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(blastHitBox.position, hitBoxSize, 0f, collisionLayers);

        foreach (var collider in hitColliders) {
            if (hasCollided) return;

            if (((1 << collider.gameObject.layer) & collisionLayers) != 0) {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                    PlayerController player = collider.GetComponent<PlayerController>();
                    if (player != null) {
                        Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                        player.TakeDamageEnemy(blastDamage, hitDirection);
                    }
                }

                HandleCollision();
                return;
            }
        }
    }

    private void HandleCollision() {
        hasCollided = true;
        transform.rotation = Quaternion.identity;
        blastSpeed = 0;

        if (anim != null) {
            anim.SetTrigger("Explode");
        }
    }

    private void OnDrawGizmos() {
        if (blastHitBox != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(blastHitBox.position, hitBoxSize);
        }
    }

    private void Destroy() {
        Destroy(gameObject);
    }
}