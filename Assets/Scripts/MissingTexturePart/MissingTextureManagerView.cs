using System;
using UnityEngine;
using UnityEngine.UI;

public class MissingTextureManagerView : MonoBehaviour, IMissingTextureManagerView
{
    private const string ANIM_KEY_CLEAR = "missing_texture_clear";

    private static MissingTextureManagerView _instance;

    [SerializeField] private int totalMissingTextureCount;
    [SerializeField] private Text txt_remainPercent;
    [SerializeField] private Image img_progressFill;
    [SerializeField] private Animator anim;

    private MissingTextureManager missingTextureManager;

    public event Action OnMissingTextureAllClear;

    public int GetTotalMissingTextureCount => totalMissingTextureCount;
    public static MissingTextureManagerView Instance => _instance;

    public void RefreshRemainPercentText(string remainPercentText)
    {
        txt_remainPercent.text = remainPercentText;
    }

    public void RefreshProgress(float progress)
    {
        img_progressFill.fillAmount = progress;
    }

    public void SendMissingTextureAllClearEvent()
    {
        OnMissingTextureAllClear?.Invoke();
    }

    public void SubtractMissingTextureCount()
    {
        missingTextureManager.SubtractMissingTextureCount();
        anim.Play(ANIM_KEY_CLEAR, 0);
    }

    public void ResetGame()
    {
        missingTextureManager.ResetGame();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        missingTextureManager = new MissingTextureManager(totalMissingTextureCount, this);
    }
}