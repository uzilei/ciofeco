using UnityEngine;

public class CameraScript : MonoBehaviour {
    private float followSpeed = 0.05f;
    [SerializeField] private Vector3 baseOffset = new Vector3(1f, 2f, -7f);
    private float aspectRatioMultiplier = 1.0f;

    private Vector3 velocity = Vector3.zero;

    [Header("Camera Bounds")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private float shakeInterval = 0.01f;

    private float shakeTimer = 0f;
    private Vector3 originalPosition;
    private float timeSinceLastShake = 0f;

    private float currentShakeAmount;
    private float currentShakeDuration;

    void Start() {
        float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        float scaleMultiplier = currentAspect / targetAspect * aspectRatioMultiplier;

        baseOffset = new Vector3(baseOffset.x, baseOffset.y * scaleMultiplier, baseOffset.z);
    }

    void FixedUpdate() {
        Vector3 targetPosition = PlayerController.Instance.transform.position + baseOffset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        if (shakeTimer > 0) {
            targetPosition += new Vector3(Random.Range(-currentShakeAmount, currentShakeAmount), Random.Range(-currentShakeAmount, currentShakeAmount), 0);
            shakeTimer -= Time.deltaTime;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);

        if (shakeTimer <= 0 && transform.position != originalPosition) {
            originalPosition = transform.position;
        }

        timeSinceLastShake += Time.deltaTime;
    }

    public void Shake(float shakeAmount, float shakeDuration) {
        currentShakeAmount = shakeAmount;
        currentShakeDuration = shakeDuration;

        if (timeSinceLastShake >= shakeInterval) {
            shakeTimer = currentShakeDuration;
            timeSinceLastShake = 0f;
        }
    }
}