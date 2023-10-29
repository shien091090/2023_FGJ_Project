using SNShien.Common.AudioTools;
using UnityEngine;

public class SavePointModel : ISavePointModel
{
    private readonly ISavePointManager savePointManager;
    private readonly ISavePointView savePointView;
    private readonly Vector3 savePointPos;
    private readonly IAudioManager audioManager;

    public SavePointModel(ISavePointView savePointView, ISavePointManager savePointManager, IAudioManager audioManager)
    {
        this.savePointManager = savePointManager;
        this.savePointView = savePointView;
        this.audioManager = audioManager;
        savePointPos = savePointView.SavePointPos;

        HideInteractHint();
    }

    public void HideAllUI()
    {
        savePointView.SetRecordStateHintActive(false);
        savePointView.SetLeftArrowActive(false);
        savePointView.SetRightArrowActive(false);
    }

    public void ShowInteractHint()
    {
        savePointView.SetLeftArrowActive(HavePreviousSavePoint());
        savePointView.SetRightArrowActive(HaveNextSavePoint());
    }

    public void HideInteractHint()
    {
        savePointView.SetLeftArrowActive(false);
        savePointView.SetRightArrowActive(false);
    }

    public bool HaveNextSavePoint()
    {
        return savePointManager.HaveNextSavePoint(savePointPos);
    }

    public ISavePointView GetNextSavePointView()
    {
        return savePointManager.GetNextSavePointView(savePointPos);
    }

    public bool HavePreviousSavePoint()
    {
        return savePointManager.HavePreviousSavePoint(savePointPos);
    }

    public ISavePointView GetPreviousSavePointView()
    {
        return savePointManager.GetPreviousSavePointView(savePointPos);
    }

    public void ShowRecordStateHint()
    {
        savePointView.SetRecordStateHintActive(true);
        RefreshRecordStateHint();
    }

    public void Save()
    {
        if (savePointManager.IsRecorded(savePointPos))
            return;

        audioManager.PlayOneShot(GameConst.AUDIO_KEY_SAVE_POINT);
        savePointManager.AddSavePoint(savePointView);
        RefreshRecordStateHint();
    }

    private void RefreshRecordStateHint()
    {
        savePointView.RefreshRecordState(savePointManager.IsRecorded(savePointPos));
    }
}