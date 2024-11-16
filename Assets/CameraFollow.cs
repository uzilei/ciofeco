using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f); // Offset compared to player

    private Vector3 velocity = Vector3.zero; // SmoothDamp variable

    // Initial Function
    void Start() {
        Camera.main.fieldOfView = 60f; // FOV, Modify on case-by-case basis
    }

    // Update every frame
    void FixedUpdate() {
        // Calculate target positon with offset (?)
        Vector3 targetPosition = PlayerController.Instance.transform.position + offset;

        // SmoothDamp
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);
    }
}