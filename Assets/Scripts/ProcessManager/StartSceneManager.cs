using UnityEngine;
using UnityEngine.UI;

public class StartSceneManager : MonoBehaviour
{
    [SerializeField] private Button btn_start;
    [SerializeField] private Button btn_ranking;
    [SerializeField] private Animator animator;

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
        animator.Play(GameConst.ANIMATION_KEY_TUTORIAL_HIDE);
    }

    public void OnClickStart()
    {
        playerRecordModel.IsViewOpening
        btn_start.enabled = false;
        btn_ranking.enabled = false;
    }

    public void OnClick()
    {
        playerRecordModel.RequestOpen(false);
    }
}