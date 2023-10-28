using SNShien.Common.AudioTools;
using UnityEngine;

public class SavePointModel
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

    public void Save()
    {
        if (savePointManager.IsRecorded(savePointPos))
            return;

        audioManager.PlayOneShot(GameConst.AUDIO_KEY_SAVE_POINT);
        savePointManager.AddSavePoint(savePointPos);
        RefreshRecordStateHint();
    }

    public void ShowRecordStateHint()
    {
        savePointView.SetRecordStateHintActive(true);
        RefreshRecordStateHint();
    }

    private void RefreshRecordStateHint()
    {
        savePointView.RefreshRecordState(savePointManager.IsRecorded(savePointPos));
    }
}