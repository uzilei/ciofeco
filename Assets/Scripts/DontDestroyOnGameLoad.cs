using UnityEngine;
// Use script as an attribute. Probably unnecessary as the only object which uses this is Camera.. but if it works, don't worry about it
public class DontDestroyOnGameLoad : MonoBehaviour
{
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}