using UnityEngine;

public class CameraFinalScene : MonoBehaviour {
  private float followSpeed = 0.5f;
    [SerializeField] private Vector3 baseOffset = new Vector3(1f, 2f, -7f);
    private float aspectRatioMultiplier = 1.0f;

    private Vector3 velocity = Vector3.zero;

    [Header("Camera Bounds")]
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -5f;
    [SerializeField] private float maxY = 5f;
    
    void Start() {
        float targetAspect = 16f / 9f;
        float currentAspect = (float)Screen.width / Screen.height;

        float scaleMultiplier = currentAspect / targetAspect * aspectRatioMultiplier;

        baseOffset = new Vector3(baseOffset.x, baseOffset.y * scaleMultiplier, baseOffset.z);
    }

    void FixedUpdate() {
        Vector3 targetPosition = MovementFinalScene.Instance.transform.position + baseOffset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);
    }
}
