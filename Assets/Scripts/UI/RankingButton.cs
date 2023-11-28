using UnityEngine;

public class RankingButton : MonoBehaviour
{
    private PlayerRecordModel playerRecordModel;

    private void Start()
    {
        playerRecordModel = PlayerRecordModel.Instance;
    }

    public void OnClick()
    {
        playerRecordModel.Open(false);
    }
}