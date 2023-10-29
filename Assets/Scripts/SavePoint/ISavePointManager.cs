using UnityEngine;

public interface ISavePointManager
{
    bool IsRecorded(Vector3 pos);
    Vector3 GetNextSavePoint(Vector3 savePointPos);
    Vector3 GetPreviousSavePoint(Vector3 savePointPos);
    void AddSavePoint(Vector3 savePointPos);
    bool HavePreviousSavePoint(Vector3 savePointPos);
    bool HaveNextSavePoint(Vector3 savePointPos);
}