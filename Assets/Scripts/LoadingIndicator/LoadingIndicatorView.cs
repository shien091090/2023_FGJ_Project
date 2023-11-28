using UnityEngine;

public class LoadingIndicatorView : MonoBehaviour, ILoadingIndicatorView
{
    [SerializeField] private GameObject go_root;

    private LoadingIndicatorModel loadingIndicatorModel;

    public void Open()
    {
        go_root.SetActive(true);
    }

    public void Close()
    {
        go_root.SetActive(false);
    }

    public void Start()
    {
        loadingIndicatorModel = LoadingIndicatorModel.Instance;
        loadingIndicatorModel.BindView(this);
    }
}