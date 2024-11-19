using UnityEngine;

public class EnemyBlocker : MonoBehaviour
{
    [SerializeField] Transform blockTransform;
    [SerializeField] Vector2 blockArea = new Vector2(1, 1);

    void FixedUpdate()
    {
        if (blockTransform == null || blockArea == null) return;

        // Check for collision with objects on the Player layer
        Collider2D hit = Physics2D.OverlapBox(
            blockTransform.position,
            blockArea,
            0f,
            LayerMask.GetMask("Player")
        );

        // Destroy the game object if a collision is detected
        if (hit != null)
        {
            Destroy(gameObject);
        }
    }

    // Optional: Visualize the detection area in the editor
    private void OnDrawGizmos()
    {
        if (blockTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(blockTransform.position, blockArea);
        }
    }
}
