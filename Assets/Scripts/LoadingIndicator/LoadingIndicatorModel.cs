public class LoadingIndicatorModel
{
    private static LoadingIndicatorModel _instance;
    private ILoadingIndicatorView view;
    public static LoadingIndicatorModel Instance => _instance;

    public LoadingIndicatorModel()
    {
        _instance = this;
    }

    public void BindView(ILoadingIndicatorView view)
    {
        this.view = view;
    }

    public void Open()
    {
        view.Open();
    }

    public void Close()
    {
        view.Close();
    }
}