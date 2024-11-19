using UnityEngine;
using System.Collections;

public class VortexProjectile : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private Transform vortexHitBox;  // Reference to the transform used for collision detection
    [SerializeField] private Vector2 vortexHitBoxSize = new Vector2(1f, 1f);  // Size of the hitbox
    [SerializeField] private LayerMask collisionLayers; // Layers to detect (e.g., Ground and Player)
    [SerializeField] private int damage; // Damage to apply to hit targets

    private void Start()
    {
        StartCoroutine(CheckCollision());
    }

    private IEnumerator CheckCollision()
    {
        yield return new WaitForSeconds(1.6f);
        // Check for collisions using Physics2D.OverlapBox
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(vortexHitBox.position, vortexHitBoxSize, 0f, collisionLayers);

        foreach (var collider in hitColliders)
        {
            // Example: Apply damage if the collider is the Player
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerController player = collider.GetComponent<PlayerController>();
                if (player != null)
                {
                    Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                    player.TakeDamageEnemy(damage, hitDirection);
                }
            }
        }
    }

    private void Destroy() {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (vortexHitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(vortexHitBox.position, vortexHitBoxSize); // Visualize the hitbox area
        }
    }
}