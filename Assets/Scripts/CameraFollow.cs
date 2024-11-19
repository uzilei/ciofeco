using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float followSpeed = 0.05f;
    [SerializeField] private Vector3 baseOffset = new Vector3(1f, 2f, -7f);
    private float aspectRatioMultiplier = 1.0f;

    private Vector3 velocity = Vector3.zero; // Used for SmoothDamp

    // Camera boundary limits
    [Header("Camera Bounds")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;

    private float shakeInterval = 0.01f; // Interval between each shake (in seconds)

    private float shakeTimer = 0f;
    private Vector3 originalPosition; // To store the original camera position
    private float timeSinceLastShake = 0f; // Track time since last shake trigger

    private float currentShakeAmount; // To store current shake amount
    private float currentShakeDuration; // To store current shake duration

    void Start()
    {
        float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        // Scale the offset dynamically based on the aspect ratio
        float scaleMultiplier = currentAspect / targetAspect * aspectRatioMultiplier;

        baseOffset = new Vector3(baseOffset.x, baseOffset.y * scaleMultiplier, baseOffset.z);
    }

    void FixedUpdate()
    {
        // Calculate target position with dynamic offset
        Vector3 targetPosition = PlayerController.Instance.transform.position + baseOffset;

        // Apply bounds to the camera position
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX); // Clamp X position
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY); // Clamp Y position

        // If shaking, apply the shake offset to the target position
        if (shakeTimer > 0)
        {
            targetPosition += new Vector3(Random.Range(-currentShakeAmount, currentShakeAmount), Random.Range(-currentShakeAmount, currentShakeAmount), 0);
            shakeTimer -= Time.deltaTime;
        }

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);

        // Store the original position for shake calculation
        if (shakeTimer <= 0 && transform.position != originalPosition)
        {
            originalPosition = transform.position;
        }

        // Update the time since the last shake
        timeSinceLastShake += Time.deltaTime;
    }

    // Function to trigger camera shake with shakeAmount and shakeDuration as parameters
    public void Shake(float shakeAmount, float shakeDuration)
    {
        // Store the passed shakeAmount and shakeDuration in the respective variables
        currentShakeAmount = shakeAmount;
        currentShakeDuration = shakeDuration;

        if (timeSinceLastShake >= shakeInterval)
        {
            shakeTimer = currentShakeDuration; // Set shake duration
            timeSinceLastShake = 0f; // Reset the timer for the next shake interval
        }
    }
}