using UnityEngine;
using UnityEngine.UI;

public class HealthGUI : MonoBehaviour {
    private Slider slide;
    private PlayerController player;

    void Start() {
        slide = GetComponent<Slider>();
        if (slide == null) {
            return;
        }

        player = PlayerController.Instance;
        if (player == null) {
            return;
        }

        slide.maxValue = player.maxHealth;
    }

    void Update() {
        if (player != null) {
            slide.value = player.health;
        }
    }
}
