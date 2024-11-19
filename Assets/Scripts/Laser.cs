using UnityEngine;
using System.Collections;

public class BeamLaser : MonoBehaviour
{
    [Header("Blast Settings")]
    [SerializeField] private float beamSpeed;  // Speed at which the beam moves along the X-axis
    [SerializeField] private int beamDamage;   // Damage dealt on impact

    [Header("Collision Detection")]
    [SerializeField] private Transform beamHitBox;  // Reference to the transform used for collision detection
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1f, 1f);  // Size of the hitbox
    [SerializeField] private LayerMask collisionLayers; // Layers to detect (e.g., Ground and Player)
    bool canDamage = false;

    void Start()
    {
        StartCoroutine(FireDaLazor());
    }

    private IEnumerator FireDaLazor() {
        CameraScript cam = Camera.main.GetComponent<CameraScript>();
        yield return new WaitForSeconds(2.5f);
        canDamage = true;
        cam.Shake(0.2f, 4.8f);
    }

    void FixedUpdate()
    {
        if (!canDamage) return;
        // Move the beam towards the player on the X-axis based on the beamSpeed
        Vector3 playerPosition = PlayerController.Instance.transform.position;
        float moveDirection = Mathf.Sign(playerPosition.x - transform.position.x); // Determine direction towards the player on the X-axis

        // Apply speed to move towards the player
        transform.position = new Vector3(transform.position.x + moveDirection * beamSpeed * Time.deltaTime, transform.position.y, transform.position.z);

        // Check for collisions using the hitbox
        CheckCollisions();
    }

    // Check for collisions with Ground and Player layers
    private void CheckCollisions()
    {
        // Use Physics2D.OverlapBox to check for collisions within the hitbox
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(beamHitBox.position, hitBoxSize, 0f, collisionLayers);

        foreach (var collider in hitColliders)
        {
            // If it hits the Player, deal damage
            if (((1 << collider.gameObject.layer) & collisionLayers) != 0)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    PlayerController player = collider.GetComponent<PlayerController>();
                    if (player != null)
                    {
                        Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                        player.TakeDamageEnemy(beamDamage, hitDirection);
                    }
                }
            }
        }
    }

    // Debug the hitbox size in the Scene view
    private void OnDrawGizmos()
    {
        if (beamHitBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(beamHitBox.position, hitBoxSize);  // Visualize the hitbox area
        }
    }

    private void StopLaser() {
        canDamage = false;
    }

    private void Destroy() {
        Destroy(gameObject);
    }
}