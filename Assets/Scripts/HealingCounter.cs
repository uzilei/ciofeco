using UnityEngine;
using TMPro;

public class HealCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;


    void Start()
    {
        if (PlayerController.Instance != null) {
            textMeshPro.text = "" + PlayerController.Instance.heals;
        } else {
            textMeshPro.text = "Missing";
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