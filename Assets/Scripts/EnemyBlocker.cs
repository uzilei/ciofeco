using UnityEngine;

public class EnemyBlocker : MonoBehaviour {
    [SerializeField] Transform blockTransform;
    [SerializeField] Vector2 blockArea = new Vector2(1, 1);

    void FixedUpdate() {
        if (blockTransform == null || blockArea == null) return;

        Collider2D hit = Physics2D.OverlapBox(
            blockTransform.position,
            blockArea,
            0f,
            LayerMask.GetMask("Player")
        );

        if (hit != null) {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos() {
        if (blockTransform != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(blockTransform.position, blockArea);
        }
    }
}
