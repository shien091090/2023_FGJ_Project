using UnityEngine;

public interface ISavePointManager
{
    bool IsRecorded(Vector3 pos);
    void AddSavePoint(Vector3 savePointPos);
}