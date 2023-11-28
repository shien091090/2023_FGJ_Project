using UnityEngine;

public class LoadingIndicatorView : MonoBehaviour
{
    private LoadingIndicatorModel loadingIndicatorModel;

    public void Start()
    {
        loadingIndicatorModel = LoadingIndicatorModel.Instance;
    }
}