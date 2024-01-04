public class LoadingIndicatorModel : ILoadingIndicatorModel
{
    private ILoadingIndicatorView view;

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