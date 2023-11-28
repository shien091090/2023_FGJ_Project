public class LoadingIndicatorModel
{
    private static LoadingIndicatorModel _instance;
    public static LoadingIndicatorModel Instance => _instance;

    public LoadingIndicatorModel()
    {
        _instance = this;
    }
}