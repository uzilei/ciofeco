using UnityEngine;

public class BlastProjectile : MonoBehaviour {
    [Header("Blast Settings")]
    [SerializeField] private float blastSpeed;  // Speed at which the blast moves
    [SerializeField] private int blastDamage;   // Damage dealt on impact
    [SerializeField] private float blastLifetime; // Time before the blast self-destructs if it doesn't hit anything

    [Header("Collision Detection")]
    [SerializeField] private Transform blastHitBox;  // Reference to the transform used for collision detection
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1f, 1f);  // Size of the hitbox
    [SerializeField] private LayerMask collisionLayers; // Layers to detect (e.g., Ground and Player)

    private float timeAlive = 0f; // Timer to track how long the blast has been alive
    private bool hasCollided = false; // Tracks if the blast has collided
    private Animator anim; // Animator to play explosion animation

    void Start()
    {
        // Ensure the blast is facing the player on spawn
        Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        transform.up = -direction; // Align the "down" side to the player

        anim = GetComponent<Animator>(); // Cache animator reference
    }

    void FixedUpdate()
    {
        // Skip movement logic if collision has occurred
        if (hasCollided)
            return;

        // Move the blast downward
        transform.Translate(Vector3.down * blastSpeed * Time.deltaTime);

        // Check for collisions using the hitbox
        CheckCollisions();

        // If the blast is still alive, check if it exceeds the lifetime
        timeAlive += Time.deltaTime;
        if (timeAlive >= blastLifetime)
        {
            Destroy(gameObject); // Destroy if it's alive too long
        }
    }

    // Check for collisions with Ground and Player layers
    private void CheckCollisions()
    {
        // Use Physics2D.OverlapBox to check for collisions within the hitbox
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(blastHitBox.position, hitBoxSize, 0f, collisionLayers);

        foreach (var collider in hitColliders)
        {
            if (hasCollided) return; // Prevent processing multiple collisions

            // If it hits the Player, deal damage
            if (((1 << collider.gameObject.layer) & collisionLayers) != 0)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    PlayerController player = collider.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                        player.TakeDamageEnemy(blastDamage, hitDirection);
                    }
                }

                // Handle collision effects
                HandleCollision();
                return;
            }
        }
    }

    private void HandleCollision()
    {
        hasCollided = true; // Mark as collided
        transform.rotation = Quaternion.identity; // Reset rotation to default
        blastSpeed = 0; // Stop movement

        // Play explosion animation
        if (anim != null)
        {
            anim.SetTrigger("Explode");
        }
    }

    // Debug the hitbox size in the Scene view
    private void OnDrawGizmos()
    {
        if (blastHitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(blastHitBox.position, hitBoxSize);  // Visualize the hitbox area
        }
    }

    private void Destroy() {
        Destroy(gameObject);
    }
}