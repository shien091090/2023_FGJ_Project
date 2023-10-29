using UnityEngine;

public interface ISavePointModel
{
    ISavePointView GetNextSavePointView();
    ISavePointView GetPreviousSavePointView();
    void Save();
    void ShowRecordStateHint();
    void HideAllUI();
    void ShowInteractHint();
    void HideInteractHint();
    bool HaveNextSavePoint();
    bool HavePreviousSavePoint();
}