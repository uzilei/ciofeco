using UnityEngine;

public class TransportObject : MonoBehaviour {
    [Header("Transport Settings")]
    [SerializeField] string targetScene;
    [SerializeField] Vector2 detectionArea = new Vector2(2, 2);
    [SerializeField] Transform detectionCenter;
    [SerializeField] LayerMask playerLayer;

    void FixedUpdate() {
        Collider2D hit = Physics2D.OverlapBox(detectionCenter.position, detectionArea, 0f, playerLayer);
        if (hit != null) {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null) {
                player.Transport(targetScene);
            }
        }
    }

    void OnDrawGizmos() {
        if (detectionCenter != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(detectionCenter.position, detectionArea);
        }
    }
}