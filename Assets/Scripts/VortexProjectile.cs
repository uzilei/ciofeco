using UnityEngine;
using System.Collections;

public class VortexProjectile : MonoBehaviour {
    [Header("Collision Settings")]
    [SerializeField] private Transform vortexHitBox;
    [SerializeField] private Vector2 vortexHitBoxSize = new Vector2(1f, 1f);
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private int damage;

    private void Start() {
        StartCoroutine(CheckCollision());
    }

    private IEnumerator CheckCollision() {
        yield return new WaitForSeconds(1.6f);

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(vortexHitBox.position, vortexHitBoxSize, 0f, collisionLayers);

        foreach (var collider in hitColliders) {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (player != null) {
                    Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                    player.TakeDamageAbsolute(damage, hitDirection);
                }
            }
        }
    }

    private void Destroy() {
        Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        if (vortexHitBox != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(vortexHitBox.position, vortexHitBoxSize);
        }
    }
}