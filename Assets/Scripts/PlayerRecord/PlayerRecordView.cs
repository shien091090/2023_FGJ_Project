using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerRecordView : MonoBehaviour, IPlayerRecordView
{
    [SerializeField] private GameObject go_root;
    [SerializeField] private GameObject go_ownRecordEffect;
    [SerializeField] private List<PlayerRecordItem> playerRecordList;

    [Inject] private IPlayerRecordModel model;

    public void SetOwnRecordEffectActive(bool isActive)
    {
        go_ownRecordEffect.SetActive(isActive);
    }

    public void SetOwnRecordEffectPosition(int ownRecordIndex)
    {
        go_ownRecordEffect.transform.position = new Vector3(go_ownRecordEffect.transform.position.x, playerRecordList[ownRecordIndex].transform.position.y);
    }

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