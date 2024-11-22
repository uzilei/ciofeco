using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour {
   public void NewGameButton() {
      SceneManager.LoadScene("Level1");
   }
}