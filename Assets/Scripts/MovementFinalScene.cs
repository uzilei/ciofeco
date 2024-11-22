using UnityEngine;

public class MovementFinalScene : MonoBehaviour {
    [SerializeField]public float speed = 5f;
    private Rigidbody2D rb;
    public static MovementFinalScene Instance;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.right * speed;
    }
     private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
