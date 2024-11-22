using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TextChangerWithReset : MonoBehaviour {
    public TextMeshProUGUI textMeshPro;
    private PlayerController player;
    [SerializeField] public float changeInterval = 2f;
    [SerializeField]  public float waitBeforeReset = 3f;
    private GameObject silhouette; 

    private string[] messages = { "GIOCO REALIZZATO CON UNITY", "GAME DEVELOPMENT: \nNAÉL TADINA e CHANGHAO LEI", "SOUNDTRACK PICK: \nSAMUEL LIVIERI", "VOICE ACTOR: \nRICCARDO MURA", 
    "GIOCO ISPIRATO A BLASPHEMOUS \nOST \nFrom Silent Hill 2 \nBetrayal - Akira Yamaoka \nPianissimo Epilogue - Akira Yamaoka \n© ℗ 2001 Konami Digital Entertainment Co.,Ltd. \nFrom Dark Souls Ⅲ \nOceiros, the Consumed King - Yuka Kitamura \n© ℗ 2019 Dark Souls™ Ⅲ & ©BANDAI NAMCO Entertainment Inc. / FromSoftware, Inc. \nFrom Resident Evil Remake \nSave Theme - Makoto Tomozawa, Akari Kaida and Masami Ueda" }; // Messaggi da mostrare
    private int currentIndex = 0;

    void Start() {
        if (textMeshPro != null) {
            StartCoroutine(ShowTextsAndReset()); 
        }
        player = PlayerController.Instance;
        silhouette = GameObject.Find("PlayerFinalScene");
    }

    private IEnumerator ShowTextsAndReset() {
        while (currentIndex < messages.Length) {
            textMeshPro.text = messages[currentIndex];
            currentIndex++;
            yield return new WaitForSeconds(changeInterval);
        }

        yield return new WaitForSeconds(waitBeforeReset);

        // ResetStates();
        Destroy(silhouette);
        SceneManager.LoadScene("MainMenu");
        }
    

//    private void ResetStates() {
//         currentIndex = 0;
//         textMeshPro.text = "";
//         vita = player.health;
//     }
}


