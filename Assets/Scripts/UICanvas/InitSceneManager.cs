using UnityEngine;
using UnityEngine.SceneManagement;

public class InitSceneManager : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("StartScene", LoadSceneMode.Additive);
    }
}