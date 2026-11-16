using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuOptions : MonoBehaviour
{
    public string SceneToLoad;
    public void StartButton()
    {
        Debug.Log("The game will now begin...");
        // SceneManager.LoadScene(SceneToLoad);
    }
    public void QuitButton()
    {
        Debug.Log("The game has quit!!!!");
        Application.Quit();
    }
}
