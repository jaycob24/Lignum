using UnityEngine;
using UnityEngine.SceneManagement;

public class EscPause : MonoBehaviour
{
    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private bool isEscape;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Escape();
        }
    }

   public GameObject gameUi;
   public GameObject escapeUi;
    void Escape()
    {
        gameUi.SetActive(isEscape);  
        escapeUi.SetActive(!isEscape);
        Time.timeScale = isEscape ? 1f : 0f;
        isEscape = !isEscape;
    }
}
