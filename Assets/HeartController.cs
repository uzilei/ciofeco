using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    private PlayerController player;
    private Image healthBarFill;
    public Transform heartsParent; 
    public Image healthBarPrefab; 

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        if (player == null)
        {
            Debug.LogError("PlayerController instance is null!");
            return;
        }

        if (healthBarPrefab == null)
        {
            Debug.LogError("healthBarPrefab is not assigned!");
            return;
        }

        if (heartsParent == null)
        {
            Debug.LogError("heartsParent is not assigned!");
            return;
        }

        healthBarFill = Instantiate(healthBarPrefab, heartsParent).GetComponent<Image>();

        if (healthBarFill == null)
        {
            Debug.LogError("Failed to instantiate healthBarPrefab or find Image component!");
            return;
        }

        PlayerController.Instance.OnHealthChangedCallBack += UpdateHeartsHUD;
        UpdateHeartsHUD();
    }

    void UpdateHeartsHUD()
    {
        if (player != null && healthBarFill != null)
        {
            // Aggiorna il valore di fillAmount in base alla salute del giocatore
            healthBarFill.fillAmount = (float)PlayerController.Instance.Health / PlayerController.Instance.maxHealth;
        }
    }
}
