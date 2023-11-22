using UnityEngine;
using UnityEngine.UI;

public class PlayerRecordItem : MonoBehaviour
{
    [SerializeField] private Text txt_playerName;
    [SerializeField] private Text txt_costTime;

    public void SetPlayerName(string playerName)
    {
        txt_playerName.text = playerName;
    }

    public void SetCostTime(string costTime)
    {
        txt_costTime.text = costTime;
    }
}