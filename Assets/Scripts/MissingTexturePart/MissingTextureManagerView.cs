using UnityEngine;
using UnityEngine.UI;

public class MissingTextureManagerView : MonoBehaviour, IMissingTextureManagerView
{
    private const string ANIM_KEY_CLEAR = "missing_texture_clear";

    [SerializeField] private Text txt_remainPercent;
    [SerializeField] private Image img_progressFill;
    [SerializeField] private Animator anim;

    private MissingTextureManager missingTextureManager;

    public void RefreshRemainPercentText(string remainPercentText)
    {
        txt_remainPercent.text = remainPercentText;
    }

    public void RefreshProgress(float progress)
    {
        img_progressFill.fillAmount = progress;
    }

    private void Start()
    {
        missingTextureManager = MissingTextureManager.Instance;
        missingTextureManager.BindView(this);
    }

    public void PlayClearAnimation()
    {
        anim.Play(ANIM_KEY_CLEAR, 0);
    }
}