using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerRecordView : MonoBehaviour, IPlayerRecordView
{
    [SerializeField] private GameObject go_root;
    [SerializeField] private List<PlayerRecordItem> playerRecordList;

    [Inject] private IPlayerRecordModel model;

    public void UpdateView()
    {
        for (int i = 0; i < playerRecordList.Count; i++)
        {
            playerRecordList[i].SetPlayerName(model.GetRecordPlayerName(i));
            playerRecordList[i].SetCostTime(model.GetRecordCostTime(i));
        }
    }

    public void SetActive(bool isActive)
    {
        go_root.SetActive(isActive);
    }

    public void Start()
    {
        model.BindView(this);
    }

    public void Close()
    {
        model.CloseView();
    }
}