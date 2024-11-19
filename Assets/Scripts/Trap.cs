using UnityEngine;

public class Trap : MonoBehaviour {
    [Header("Trap Settings")]
    [SerializeField] int trapDamage;
    [SerializeField] Vector2 trapArea = new Vector2(2, 2);
    [SerializeField] Transform trapCenter;
    [SerializeField] LayerMask playerLayer;

    void FixedUpdate() {
        Collider2D hit = Physics2D.OverlapBox(trapCenter.position, trapArea, 0f, playerLayer);
        if (hit != null) {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null) {
                Vector2 hitDirection = (hit.transform.position - transform.position).normalized;
                player.TakeDamageAbsolute(trapDamage, hitDirection);
            }
        }
    }

    void OnDrawGizmos() {
        if (trapCenter != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(trapCenter.position, trapArea);
        }
    }
}
