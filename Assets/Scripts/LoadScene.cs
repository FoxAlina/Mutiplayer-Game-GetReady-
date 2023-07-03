using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void StartGame(string name)
    {
        SceneManager.LoadScene(name);
    }
}
