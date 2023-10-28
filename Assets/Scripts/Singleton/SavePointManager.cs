using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SavePointManager : MonoBehaviour, ISavePointManager
{
    private static SavePointManager _instance;
    private List<Vector3> recordSavePoints;

    public static SavePointManager Instance => _instance;

    public void AddSavePoint(Vector3 savePointPos)
    {
        if (IsRecorded(savePointPos))
            return;

        recordSavePoints.Add(savePointPos);
        recordSavePoints = recordSavePoints.OrderBy(pos => pos.x).ToList();

        List<string> savePointPosStrs = recordSavePoints.Select(pos => pos.ToString()).ToList();
        Debug.Log($"SavePointManager: {string.Join(", \n", savePointPosStrs)}");
    }

    public bool IsRecorded(Vector3 pos)
    {
        return recordSavePoints.Contains(pos);
    }

    private void Start()
    {
        recordSavePoints = new List<Vector3>();
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
    }
}