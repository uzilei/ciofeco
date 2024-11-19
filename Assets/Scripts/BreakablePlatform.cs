using System.Collections;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour {
    [Header("Trap Settings")]
    [SerializeField] Vector2 platformArea = new Vector2(2, 2);
    [SerializeField] Transform platformCenter;
    [SerializeField] LayerMask playerLayer;

    private BoxCollider2D platformCollider;
    private Animator anim;

    void Start() {
        platformCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        Collider2D hit = Physics2D.OverlapBox(platformCenter.position, platformArea, 0f, playerLayer);
        if (hit != null) {
            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null) {
                if (platformCollider.enabled) {
                    anim.SetBool("Breaking", true);
                    StartCoroutine(ColliderToggle());
                }
            }
        }
    }

    private IEnumerator ColliderToggle() {
        yield return new WaitForSeconds(1f);
        platformCollider.enabled = false;
        yield return new WaitForSeconds(13.5f);
        platformCollider.enabled = true;
        anim.SetBool("Breaking", false);
    }

    void OnDrawGizmos() {
        if (platformCenter != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(platformCenter.position, platformArea);
        }
    }
}