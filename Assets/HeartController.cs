using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    private Slider slide; // Riferimento allo Slider
    private PlayerController player; // Riferimento al PlayerController

    void Start()
    {
        // Ottieni il componente Slider
        slide = GetComponent<Slider>();
        if (slide == null)
        {
            Debug.LogError("Slider component not found on this GameObject!");
            return;
        }

        // Accedi all'istanza del PlayerController
        player = PlayerController.Instance;
        if (player == null)
        {
            Debug.LogError("PlayerController instance not found!");
            return;
        }

        // Imposta il valore massimo dello Slider in base alla salute massima del giocatore
        slide.maxValue = player.maxHealth;
    }

    void Update()
    {
        if (player != null)
        {
            // Aggiorna il valore dello Slider in base alla salute corrente
            slide.value = player.health;
        }
    }
}
