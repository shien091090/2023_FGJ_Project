using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameCompleteManager : MonoBehaviour
{
    [SerializeField] private GameObject go_rankingButton;
    [SerializeField] private GameObject go_resetButton;

    [Inject] private IMissingTextureManager missingTextureManager;
    [Inject] private IPlayerRecordModel playerRecordModel;
    [Inject] private IAudioManager audioManager;
    [Inject] private ITimerModel timerModel;

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
        SetButtonActive(false);

        audioManager.Play(GameConst.AUDIO_KEY_BGM_GAME, 1);
        GetAnim.Play(ANIM_KEY_IDLE);

        missingTextureManager.OnMissingTextureAllClear -= OnMissingTextureAllClear;
        missingTextureManager.OnMissingTextureAllClear += OnMissingTextureAllClear;
    }

    private void SetButtonActive(bool isActive)
    {
        go_rankingButton.SetActive(isActive);
        go_resetButton.SetActive(isActive);
    }

    private IEnumerator Cor_GameComplete()
    {
        playerRecordModel.RequestAddPlayerRecord((int)timerModel.CurrentTime);
        audioManager.Stop(1);
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_VICTORY);
        GetAnim.Play(ANIM_KEY_GAME_COMPLETED);

        yield return new WaitForSeconds(4f);

        audioManager.Play(GameConst.AUDIO_KEY_BGM_ENDING, 1);

        yield return new WaitForSeconds(1f);

        playerRecordModel.RequestOpen();
        SetButtonActive(true);
    }

    private void OnMissingTextureAllClear()
    {
        StartCoroutine(Cor_GameComplete());
    }

    public void OnClickRanking()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_BUTTON_CLICK);
        playerRecordModel.RequestOpen();
    }

    public void OnClickReset()
    {
        missingTextureManager.ResetGame();
        SceneManager.UnloadSceneAsync("MainScene");
        SceneManager.LoadScene("StartScene", LoadSceneMode.Additive);
    }
}