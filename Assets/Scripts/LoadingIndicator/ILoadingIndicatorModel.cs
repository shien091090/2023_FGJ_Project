public interface ILoadingIndicatorModel
{
    void Open();
    void Close();
    void BindView(ILoadingIndicatorView view);
}