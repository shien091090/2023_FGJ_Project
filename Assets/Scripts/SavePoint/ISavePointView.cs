using UnityEngine;

public interface ISavePointView
{
    Vector3 SavePointPos { get; }
    ISavePointModel GetModel { get; }
    void SetRecordStateHintActive(bool isActive);
    void SetLeftArrowActive(bool isActive);
    void SetRightArrowActive(bool isActive);
    void RefreshRecordState(bool isRecorded);
    void PlaySavePointPopupEffect();
}