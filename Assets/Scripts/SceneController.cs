using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
   public GameObject PlayerPrefab;
    public string sceneName = "Scene1";

    void Start() {
        if (SceneManager.GetActiveScene().name == sceneName) {
            if (GameObject.FindGameObjectWithTag("Player") == null) {
                Instantiate(PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            }
        }
    }
}