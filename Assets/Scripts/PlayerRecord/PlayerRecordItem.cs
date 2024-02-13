using TMPro;
using UnityEngine;

public class PlayerRecordItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp_playerName;
    [SerializeField] private TextMeshProUGUI tmp_costTime;

    public void SetPlayerName(string playerName)
    {
        tmp_playerName.text = playerName;
    }

    public void SetCostTime(string costTime)
    {
        tmp_costTime.text = costTime;
    }
}