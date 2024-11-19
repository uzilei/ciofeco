using UnityEngine;
using TMPro;

public class HealCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;

    void FixedUpdate()
    {
        if (PlayerController.Instance != null) {
            textMeshPro.text = "" + PlayerController.Instance.heals;
        } else {
            textMeshPro.text = "Player Missing";
        }
    }

    public void UpdateText(string newText)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = newText;
        }
    }
}