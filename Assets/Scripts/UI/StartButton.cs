using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.UnloadSceneAsync("StartScene");
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }
}