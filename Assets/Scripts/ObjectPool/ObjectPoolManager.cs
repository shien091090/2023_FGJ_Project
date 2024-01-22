using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour, IGameObjectPool
{
    public List<ObjectPoolUnit> objectPoolSetting; //物件池設定
    private Dictionary<string, ObjectPoolUnit> objectPoolTagDict { set; get; } //(字典)從物件名稱查找ObjectPoolUnit

    private void Start()
    {
        if (objectPoolSetting == null || objectPoolSetting.Count <= 0)
            return; //若有設定物件池

        objectPoolTagDict = new Dictionary<string, ObjectPoolUnit>();

        for (int i = 0; i < objectPoolSetting.Count; i++)
        {
            if (objectPoolTagDict.ContainsKey(objectPoolSetting[i].gameObjectName)) throw new System.Exception("[ERROR]物件名稱重複");

            objectPoolTagDict.Add(objectPoolSetting[i].gameObjectName, objectPoolSetting[i]); //建立字典
        }
    }

    public void SpawnGameObject(string prefabName, Vector3 position = default, Vector3 scale = default)
    {
        if (objectPoolTagDict.ContainsKey(prefabName) == false)
            return;

        CheckAutoCreateHolder(prefabName);
        GameObject go = PickUpObject(prefabName);

        if (position != default)
            go.transform.position = position;

        if (scale != default)
            go.transform.localScale = scale;

        go.SetActive(true);
    }

    //從物件池中取得指定物件
    public GameObject PickUpObject(string goName)
    {
        if (objectPoolTagDict.ContainsKey(goName) == false)
            return null;

        GameObject _result = null;
        ObjectPoolUnit _unit = objectPoolTagDict[goName];

        //創立新物件(Lambda)
        System.Action<ObjectPoolUnit> CreateNew = (ObjectPoolUnit u) =>
        {
            GameObject _go = Instantiate(u.prefabReference, u.parentHolder);
            u.AddElement(_go);

            _result = _go;
        };

        if (_unit.ObjectPoolList == null || _unit.ObjectPoolList.Count == 0) //若物件池中無物件
        {
            CreateNew(_unit); //創立新物件
        }
        else //若物件池中存在物件
        {
            for (int i = 0; i < _unit.ObjectPoolList.Count; i++) //尋找隱藏中(active = false)物件
            {
                if (!_unit.ObjectPoolList[i].activeSelf)
                {
                    _result = _unit.ObjectPoolList[i];
                    break;
                }
            }

            if (_result == null) CreateNew(_unit); //若所有物件皆在非隱藏狀態, 則建立新物件
        }

        return _result;
    }

    private void CheckAutoCreateHolder(string prefabName)
    {
        if (objectPoolTagDict.TryGetValue(prefabName, out ObjectPoolUnit unit) == false)
            return;

        if (unit.parentHolder != null)
            return;

        unit.parentHolder = new GameObject(prefabName + "Holder").transform;
        unit.parentHolder.parent = transform;
    }
}