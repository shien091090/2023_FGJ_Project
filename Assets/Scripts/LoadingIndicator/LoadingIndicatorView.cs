using UnityEngine;
using Zenject;

public class LoadingIndicatorView : MonoBehaviour, ILoadingIndicatorView
{
    [SerializeField] private GameObject go_root;

    [Inject] private ILoadingIndicatorModel loadingIndicatorModel;

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
        loadingIndicatorModel.BindView(this);
    }
}