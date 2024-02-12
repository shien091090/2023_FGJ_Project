using System;
using System.Collections;
using SNShien.Common.AudioTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private bool testMode;
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_ranking;
    [SerializeField] private InputField inputField_playerName;
    [SerializeField] private Animator startSceneAnimator;
    [SerializeField] private Animator tutorialAnimator;
    [SerializeField] private Animator inputFieldAnimator;
    [SerializeField] private Text txt_playerNameEffect;

    [Inject] private IPlayerRecordModel playerRecordModel;
    [Inject] private IAudioManager audioManager;
    [Inject] private IGlobalStateModel globalStateModel;

    private bool isSwitchedTutorialNext;

    private void Update()
    {
        if (testMode == false)
            return;

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (globalStateModel.IsPlayerNameInputted == false)
            {
                audioManager.PlayOneShot(GameConst.AUDIO_KEY_ERROR);
                inputFieldAnimator.Play(GameConst.ANIMATION_KEY_INPUT_FIELD_NOT_PASS, 0, 0);
                return;
            }

            SwitchToGameScene();
        }
    }

    private void Init()
    {
        audioManager.InitCollectionFromProject();

        isSwitchedTutorialNext = false;
        playerRecordModel.RequestGetPlayerRecord();
        SetInputComponentInteractive(true);
        InitPlayerNameInputField();

        inputField_playerName.onValueChanged.RemoveListener(OnPlayerNameInputted);
        inputField_playerName.onValueChanged.AddListener(OnPlayerNameInputted);

        startSceneAnimator.Play(GameConst.ANIMATION_KEY_START_SCENE_IDLE);
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_HIDE);
        audioManager.Play(GameConst.AUDIO_KEY_BGM_START, 1);
    }

    private void InitPlayerNameInputField()
    {
        if (globalStateModel.IsPlayerNameInputted)
            inputField_playerName.SetTextWithoutNotify(globalStateModel.GetPlayerName);
    }

    private void SetInputComponentInteractive(bool isInteractive)
    {
        btn_start.enabled = isInteractive;
        btn_ranking.enabled = isInteractive;
        inputField_playerName.enabled = isInteractive;
    }

    private void Awake()
    {
        Init();
    }

    private void SwitchToGameScene()
    {
        SceneManager.UnloadSceneAsync("StartScene");
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
    }

    private IEnumerator Cor_PlayTutorialAnimation()
    {
        txt_playerNameEffect.text = globalStateModel.GetPlayerName;
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_SWITCH_SCENE_TO_TUTORIAL);
        startSceneAnimator.Play(GameConst.ANIMATION_KEY_START_SCENE_ENTER);
        audioManager.Stop(1);

        yield return new WaitForSeconds(1.8f);

        audioManager.Play(GameConst.AUDIO_KEY_BGM_TUTORIAL, 1);
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_START);
    }

    private IEnumerator Cor_PlayTutorialEndAnimation()
    {
        audioManager.Stop(1);
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_ENTER_GAME);

        yield return new WaitForSeconds(3);

        SwitchToGameScene();
    }

    public void OnClickStart()
    {
        if (globalStateModel.IsPlayerNameInputted == false)
        {
            audioManager.PlayOneShot(GameConst.AUDIO_KEY_ERROR);
            inputFieldAnimator.Play(GameConst.ANIMATION_KEY_INPUT_FIELD_NOT_PASS, 0, 0);
            return;
        }

        if (playerRecordModel.IsViewOpening)
            playerRecordModel.CloseView();

        SetInputComponentInteractive(false);
        StartCoroutine(Cor_PlayTutorialAnimation());
    }

    public void OnClickTutorialNext()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_BUTTON_SWITCH);

        tutorialAnimator.Play(isSwitchedTutorialNext ?
            GameConst.ANIMATION_KEY_TUTORIAL_SWITCH_FORWARD :
            GameConst.ANIMATION_KEY_TUTORIAL_ENTER_NEXT);

        isSwitchedTutorialNext = true;
    }

    public void OnClickTutorialPrev()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_BUTTON_SWITCH);

        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_SWITCH_BACK);
    }

    public void OnClickTutorialEnd()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_BUTTON_CLICK);

        StartCoroutine(Cor_PlayTutorialEndAnimation());
    }

    public void OnClickPlayerRecord()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_BUTTON_CLICK);

        playerRecordModel.RequestOpen(false);
    }

    private void OnPlayerNameInputted(string playerName)
    {
        globalStateModel.SetPlayerName(playerName);
    }
}