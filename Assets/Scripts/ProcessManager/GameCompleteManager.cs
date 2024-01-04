using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameCompleteManager : MonoBehaviour
{
    [Inject] private IMissingTextureManager missingTextureManager;

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
        FmodAudioManager.Instance.Play(GameConst.AUDIO_KEY_BGM_GAME, 1);
        GetAnim.Play(ANIM_KEY_IDLE);

        missingTextureManager.OnMissingTextureAllClear -= OnMissingTextureAllClear;
        missingTextureManager.OnMissingTextureAllClear += OnMissingTextureAllClear;
    }

    private IEnumerator Cor_GameComplete()
    {
        yield return new WaitForSeconds(4f);

        missingTextureManager.ResetGame();
        SceneManager.LoadScene("MainScene");
    }

    private void OnMissingTextureAllClear()
    {
        GetAnim.Play(ANIM_KEY_GAME_COMPLETED);
        StartCoroutine(Cor_GameComplete());
    }
}