using UnityEngine;

public class CameraFollow : MonoBehaviour {
    private float followSpeed = 0.2f; // Speed of camera smoothing
    private Vector3 baseOffset = new Vector3(1f, 2f, -7f); // Default offset
    private float aspectRatioMultiplier = 1.0f; // Multiplier for dynamic aspect ratio adjustment

    private Vector3 velocity = Vector3.zero; // Used for SmoothDamp

    private float fixedYPosition; // Store the fixed Y position for the camera

    // Adjusts the camera offset dynamically based on the current screen resolution and aspect ratio.
    void Start() {
        DontDestroyOnLoad(gameObject);

        float targetAspect = 16f / 9f; // Desired standard aspect ratio
        float currentAspect = (float)Screen.width / Screen.height;

        // Scale the offset dynamically based on the aspect ratio
        float scaleMultiplier = currentAspect / targetAspect * aspectRatioMultiplier;

        baseOffset = new Vector3(baseOffset.x, baseOffset.y * scaleMultiplier, baseOffset.z);

        // Store the initial Y position to lock it in place
        fixedYPosition = transform.position.y;
    }

    void FixedUpdate() {
        if (PlayerController.Instance == null) {
            Debug.LogWarning("PlayerController instance not found!");
            return;
        }

        // Calculate target position with dynamic offset
        Vector3 targetPosition = PlayerController.Instance.transform.position + baseOffset;

        // Lock the vertical (Y) position of the camera
        targetPosition.y = fixedYPosition;

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);
    }
}