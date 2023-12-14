using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_ranking;
    [SerializeField] private Animator startSceneAnimator;
    [SerializeField] private Animator tutorialAnimator;

    private PlayerRecordModel playerRecordModel;
    private IAudioManager audioManager;
    private bool isSwitchedTutorialNext;

    private void Start()
    {
        playerRecordModel = PlayerRecordModel.Instance;
        audioManager = FmodAudioManager.Instance;

        Init();
    }

    private void Init()
    {
        isSwitchedTutorialNext = false;
        playerRecordModel.RequestPlayerRecord();

        btn_start.enabled = true;
        btn_ranking.enabled = true;
        startSceneAnimator.Play(GameConst.ANIMATION_KEY_START_SCENE_IDLE);
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_HIDE);
        audioManager.Play(GameConst.AUDIO_KEY_BGM_START, 1);
    }

    private IEnumerator Cor_PlayTutorialAnimation()
    {
        startSceneAnimator.Play(GameConst.ANIMATION_KEY_START_SCENE_ENTER);

        yield return new WaitForSeconds(0.6f);

        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_START);
    }

    private IEnumerator Cor_PlayTutorialEndAnimation()
    {
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_ENTER_GAME);

        yield return new WaitForSeconds(3);

        SceneManager.UnloadSceneAsync("StartScene");
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }

    public void OnClickStart()
    {
        if (playerRecordModel.IsViewOpening)
            playerRecordModel.CloseView();

        btn_start.enabled = false;
        btn_ranking.enabled = false;

        StartCoroutine(Cor_PlayTutorialAnimation());
    }

    public void OnClickTutorialNext()
    {
        tutorialAnimator.Play(isSwitchedTutorialNext ?
            GameConst.ANIMATION_KEY_TUTORIAL_SWITCH_FORWARD :
            GameConst.ANIMATION_KEY_TUTORIAL_ENTER_NEXT);

        isSwitchedTutorialNext = true;
    }

    public void OnClickTutorialPrev()
    {
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_SWITCH_BACK);
    }

    public void OnClickTutorialEnd()
    {
        StartCoroutine(Cor_PlayTutorialEndAnimation());
    }

    public void OnClickPlayerRecord()
    {
        playerRecordModel.RequestOpen(false);
    }
}