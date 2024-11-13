using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f; // Velocità di follow
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f); // Offset rispetto al giocatore

    private Vector3 velocity = Vector3.zero; // Variabile per SmoothDamp

    // Start è chiamato all'inizio
    void Start()
    {
        // Imposta un FOV di default per evitare zoom strani
        Camera.main.fieldOfView = 60f; // Puoi regolarlo a seconda delle necessità
    }

    // Update è chiamato una volta per frame
    void Update()
    {
        // Calcola la posizione target con l'offset
        Vector3 targetPosition = PlayerControll.Instance.transform.position + offset;

        // Sposta la telecamera verso la posizione target con SmoothDamp per un movimento più fluido
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, followSpeed);
    }
}
