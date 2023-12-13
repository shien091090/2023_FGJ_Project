using System.Collections.Generic;
using UnityEngine;

public class PlayerRecordView : MonoBehaviour, IPlayerRecordView
{
    [SerializeField] private GameObject go_root;
    [SerializeField] private List<PlayerRecordItem> playerRecordList;

    private PlayerRecordModel model;

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
        model = PlayerRecordModel.Instance;
        model.BindView(this);
    }

    public void Close()
    {
        model.CloseView();
    }
}