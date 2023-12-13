using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_ranking;
    [SerializeField] private Animator startSceneAnimator;
    [SerializeField] private Animator tutorialAnimator;

    private PlayerRecordModel playerRecordModel;

    private void Start()
    {
        playerRecordModel = PlayerRecordModel.Instance;
        Init();
    }

    private void Init()
    {
        playerRecordModel.RequestPlayerRecord();

        btn_start.enabled = true;
        btn_ranking.enabled = true;
        startSceneAnimator.Play(GameConst.ANIMATION_KEY_START_SCENE_IDLE);
        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_HIDE);
    }

    private IEnumerator Cor_PlayEnterGameAnimation()
    {
        startSceneAnimator.Play(GameConst.ANIMATION_KEY_START_SCENE_ENTER);

        yield return new WaitForSeconds(0.6f);

        tutorialAnimator.Play(GameConst.ANIMATION_KEY_TUTORIAL_START);
    }

    public void OnClickStart()
    {
        if (playerRecordModel.IsViewOpening)
            playerRecordModel.CloseView();

        btn_start.enabled = false;
        btn_ranking.enabled = false;

        StartCoroutine(Cor_PlayEnterGameAnimation());
    }

    public void OnClickPlayerRecord()
    {
        playerRecordModel.RequestOpen(false);
    }
}