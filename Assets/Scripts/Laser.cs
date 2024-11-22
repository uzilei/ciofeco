using UnityEngine;
using System.Collections;

public class BeamLaser : MonoBehaviour {
    [Header("Blast Settings")]
    [SerializeField] private float beamSpeed;
    [SerializeField] private int beamDamage;

    [Header("Collision Detection")]
    [SerializeField] private Transform beamHitBox;
    [SerializeField] private Vector2 hitBoxSize = new Vector2(1f, 1f);
    [SerializeField] private LayerMask collisionLayers;

    private bool canDamage = false;

    void Start() {
        StartCoroutine(FireDaLazor());
    }

    private IEnumerator FireDaLazor() {
        CameraScript cam = Camera.main.GetComponent<CameraScript>();
        yield return new WaitForSeconds(2.5f);
        canDamage = true;
        cam.Shake(0.2f, 4.8f);
    }

    void FixedUpdate() {
        if (!canDamage) return;

        Vector3 playerPosition = PlayerController.Instance.transform.position;
        float moveDirection = Mathf.Sign(playerPosition.x - transform.position.x);

        transform.position = new Vector3(transform.position.x + moveDirection * beamSpeed * Time.deltaTime, transform.position.y, transform.position.z);

        CheckCollisions();
    }

    private void CheckCollisions() {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(beamHitBox.position, hitBoxSize, 0f, collisionLayers);

        foreach (var collider in hitColliders) {
            if (((1 << collider.gameObject.layer) & collisionLayers) != 0) {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                    PlayerController player = collider.GetComponent<PlayerController>();
                    if (player != null) {
                        Vector2 hitDirection = (collider.transform.position - transform.position).normalized;
                        player.TakeDamageEnemy(beamDamage, hitDirection);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos() {
        if (beamHitBox != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(beamHitBox.position, hitBoxSize);
        }
    }

    private void StopLaser() {
        canDamage = false;
    }

    private void Destroy() {
        Destroy(gameObject);
    }
}