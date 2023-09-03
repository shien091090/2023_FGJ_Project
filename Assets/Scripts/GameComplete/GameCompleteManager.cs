using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameCompleteManager : MonoBehaviour
{
    private string ANIM_KEY_IDLE = "game_complete_idle";
    private string ANIM_KEY_GAME_COMPLETED = "game_complete";

    private Animator anim;

    public Animator GetAnim
    {
        get
        {
            if (anim == null)
                anim = GetComponent<Animator>();

            return anim;
        }
    }

    private void Start()
    {
        FmodAudioManager.Instance.PlayOneShot("BGM");
        GetAnim.Play(ANIM_KEY_IDLE);
        MissingTextureManager.OnMissingTextureAllClear -= OnMissingTextureAllClear;
        MissingTextureManager.OnMissingTextureAllClear += OnMissingTextureAllClear;
    }

    private IEnumerator Cor_GameComplete()
    {
        yield return new WaitForSeconds(4f);

        MissingTextureManager.Instance.ResetGame();
        SceneManager.LoadScene("MainScene");
    }

    private void OnMissingTextureAllClear()
    {
        GetAnim.Play(ANIM_KEY_GAME_COMPLETED);
        StartCoroutine(Cor_GameComplete());
    }
}