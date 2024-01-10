public interface IPlayerRecordView
{
    void SetActive(bool isActive);
    void SetOwnRecordEffectActive(bool isActive);
    void SetOwnRecordEffectPosition(int ownRecordIndex);
    void UpdateView();
}