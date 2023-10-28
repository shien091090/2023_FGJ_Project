using UnityEngine;

public interface ISavePointView
{
    Vector3 SavePointPos { get; }
    void SetRecordStateHintActive(bool isActive);
    void SetLeftArrowActive(bool isActive);
    void SetRightArrowActive(bool isActive);
    void Save();
    void RefreshRecordState(bool isRecorded);
    void ShowRecordStateHint();
    void HideAllUI();
}