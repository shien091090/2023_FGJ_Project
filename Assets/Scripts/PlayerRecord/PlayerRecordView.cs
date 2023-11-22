using System.Collections.Generic;
using UnityEngine;

public class PlayerRecordView : MonoBehaviour
{
    [SerializeField] private GameObject go_root;
    [SerializeField] private List<PlayerRecordItem> playerRecordList;

    private PlayerRecordModel model;

    public int GetRecordLength => playerRecordList.Count;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Open(true);
        }
    }

    public void Start()
    {
        model = new PlayerRecordModel(ServerCommunicator.Instance);
    }

    public void Open(bool isRequestRecord)
    {
        if (isRequestRecord)
            model.RequestPlayerRecord(UpdateView);
        else
            UpdateView();
    }

    private void UpdateView()
    {
        go_root.SetActive(true);
        for (int i = 0; i < GetRecordLength; i++)
        {
            playerRecordList[i].SetPlayerName(model.GetRecordPlayerName(i));
            playerRecordList[i].SetCostTime(model.GetRecordCostTime(i));
        }
    }
}