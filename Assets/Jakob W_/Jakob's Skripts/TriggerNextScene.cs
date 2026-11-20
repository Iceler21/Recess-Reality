using UnityEngine;

public class TriggerNextScene : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLoaderList.Instance.LoadNextScene();
            Debug.Log("it worked");
        }
    }
}