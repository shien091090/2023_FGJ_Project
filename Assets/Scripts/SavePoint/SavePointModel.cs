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
    }

    public void HideAllUI()
    {
        savePointView.SetRecordStateHintActive(false);
        savePointView.SetLeftArrowActive(false);
        savePointView.SetRightArrowActive(false);
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
        savePointManager.AddSavePoint(savePointPos);
        RefreshRecordStateHint();
    }

    private void RefreshRecordStateHint()
    {
        savePointView.RefreshRecordState(savePointManager.IsRecorded(savePointPos));
    }
}