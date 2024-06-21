using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }

    public void LoadHowToScene()
    {
        SceneManager.LoadScene("HowTo");
    }
}
