using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Calibration");
    }

    public void LoadHowToScene()
    {
        SceneManager.LoadScene("HowTo");
    }

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
