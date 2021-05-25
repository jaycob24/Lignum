using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Compilation");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
