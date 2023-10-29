using UnityEngine;

public interface ISavePointModel
{
    void Save();
    void ShowRecordStateHint();
    void HideAllUI();
    void ShowInteractHint();
    void HideInteractHint();
    bool HaveNextSavePoint();
    Vector3 GetNextSavePointPos();
    bool HavePreviousSavePoint();
    Vector3 GetPreviousSavePointPos();
}