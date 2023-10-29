using UnityEngine;

public interface ISavePointManager
{
    bool IsRecorded(Vector3 pos);
    ISavePointView GetNextSavePointView(Vector3 savePointPos);
    ISavePointView GetPreviousSavePointView(Vector3 savePointPos);
    void AddSavePoint(ISavePointView savePointView);
    bool HavePreviousSavePoint(Vector3 savePointPos);
    bool HaveNextSavePoint(Vector3 savePointPos);
}