using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletHandlerView : MonoBehaviour, IBulletHandlerView
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform objHolder;

    private List<GameObject> bulletObjectPool;

    public GameObject GetBulletObject()
    {
        List<GameObject> hidingObjs = bulletObjectPool.Where(x => x.activeSelf == false).ToList();
        if (hidingObjs.Count > 0)
            return hidingObjs[0];
        else
        {
            GameObject newObj = Instantiate(prefab, objHolder);
            bulletObjectPool.Add(newObj);
            return newObj;
        }
    }

    private void Start()
    {
        bulletObjectPool = new List<GameObject>();
        BulletHandlerModel.Instance.BindView(this);
    }
}