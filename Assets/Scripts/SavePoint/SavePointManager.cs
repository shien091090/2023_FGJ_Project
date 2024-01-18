using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SavePointManager : MonoBehaviour, ISavePointManager
{
    private static SavePointManager _instance;
    private List<ISavePointView> recordSavePointViews;
    private List<Vector3> recordSavePointPosList;

    public static SavePointManager Instance => _instance;

    public void AddSavePoint(ISavePointView savePointView)
    {
        if (IsRecorded(savePointView.SavePointPos))
            return;

        recordSavePointViews.Add(savePointView);
        recordSavePointPosList = recordSavePointViews.Select(x => x.SavePointPos).OrderBy(pos => pos.x).ToList();

        List<string> savePointPosLogs = recordSavePointPosList.Select(pos => pos.ToString()).ToList();
        Debug.Log($"SavePointManager: {string.Join(", \n", savePointPosLogs)}");
    }

    public bool HavePreviousSavePoint(Vector3 savePointPos)
    {
        return recordSavePointPosList.IndexOf(savePointPos) > 0;
    }

    public bool HaveNextSavePoint(Vector3 savePointPos)
    {
        return recordSavePointPosList.IndexOf(savePointPos) < recordSavePointPosList.Count - 1;
    }

    public bool IsRecorded(Vector3 pos)
    {
        return recordSavePointPosList.Contains(pos);
    }

    public ISavePointView GetPreviousSavePointView(Vector3 savePointPos)
    {
        Vector3 previousPointPos = recordSavePointPosList[recordSavePointPosList.IndexOf(savePointPos) - 1];
        return recordSavePointViews.FirstOrDefault(x => x.SavePointPos == previousPointPos);
    }

    public ISavePointView GetNextSavePointView(Vector3 savePointPos)
    {
        Vector3 nextPointPos = recordSavePointPosList[recordSavePointPosList.IndexOf(savePointPos) + 1];
        return recordSavePointViews.FirstOrDefault(x => x.SavePointPos == nextPointPos);
    }

    private void Start()
    {
        recordSavePointViews = new List<ISavePointView>();
        recordSavePointPosList = new List<Vector3>();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
}