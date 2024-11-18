using UnityEngine;

public class BlastProjectile : MonoBehaviour
{
    [Header("Blast Settings")]
    [SerializeField] private float blastSpeed = 5f;  // Speed at which the blast moves
    [SerializeField] private int blastDamage = 10;   // Damage dealt on impact
    [SerializeField] private float blastLifetime = 5f; // Time before the blast self-destructs if it doesn't hit anything
    [SerializeField] private string[] hitTags;        // Tags to detect collisions with (e.g., "Player", "Ground")

    [Header("Collision Detection")]
    [SerializeField] private Transform blastHitBox;  // Reference to the transform used for collision detection
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1f, 1f);  // Size of the hitbox

    private float timeAlive = 0f; // Timer to track how long the blast has been alive
    Animator anim;

    void Start()
    {
        // Ensure the blast is facing the player on spawn
        Vector3 direction = (PlayerController.Instance.transform.position - transform.position).normalized;
        transform.up = direction; // Align the "down" side to the player
    }

    void FixedUpdate()
    {
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

    // Check for collisions with valid tags using the blastHitBox transform
    private void CheckCollisions()
    {
        // Use Physics2D.OverlapBox to check for collisions within the hitbox
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(blastHitBox.position, hitBoxSize, 0f);

        foreach (var collider in hitColliders)
        {
            // Check if the hit collider matches any of the specified hit tags
            foreach (string tag in hitTags)
            {
                if (collider.CompareTag(tag))
                {
                    // If it hits the player, deal damage
                    if (collider.CompareTag("Player"))
                    {
                        PlayerController player = collider.GetComponent<PlayerController>();
                        if (player != null)
                        {
                            Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                            player.TakeDamageAbsolute(blastDamage, hitDirection);
                        }
                    }

                    // Play animation and then destroy the blast object
                    anim.SetTrigger("Explode");
                    Destroy(gameObject, 0.5f); // Delay destruction to allow animation to play
                    return;
                }
            }
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
}