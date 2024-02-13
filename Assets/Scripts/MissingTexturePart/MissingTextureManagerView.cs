using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MissingTextureManagerView : MonoBehaviour, IMissingTextureManagerView
{
    private const string ANIM_KEY_CLEAR = "missing_texture_clear";

    [SerializeField] private TextMeshProUGUI tmp_remainPercent;
    [SerializeField] private Image img_progressFill;
    [SerializeField] private Animator anim;

    [Inject] private IMissingTextureManager missingTextureManager;

    public void RefreshRemainPercentText(string remainPercentText)
    {
        tmp_remainPercent.text = remainPercentText;
    }

    public void RefreshProgress(float progress)
    {
        img_progressFill.fillAmount = progress;
    }

    public void PlayClearAnimation()
    {
        anim.Play(ANIM_KEY_CLEAR, 0);
    }

    private void Start()
    {
        missingTextureManager.BindView(this);
    }
}