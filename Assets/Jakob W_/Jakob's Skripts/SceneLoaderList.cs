using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneLoaderList : MonoBehaviour
{
    public static SceneLoaderList Instance { get; private set; }

    public List<string> sceneOrder = new List<string>();

    private int currentScene = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadNextScene()
    {
        currentScene++;

        if (currentScene < sceneOrder.Count)
        {
            string nextScene = sceneOrder[currentScene];
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("This is the end of the line buddy");
        }
    }

}
