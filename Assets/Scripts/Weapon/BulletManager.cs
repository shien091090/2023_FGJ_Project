using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform objHolder;
    [SerializeField] private Transform character;

    private List<GameObject> bulletObjectPool;

    private void Start()
    {
        bulletObjectPool = new List<GameObject>();

        ItemStateManager.Instance.OnUseItemOneTime -= OnUseItemOneTime;
        ItemStateManager.Instance.OnUseItemOneTime += OnUseItemOneTime;
    }

    private GameObject GetBulletObject()
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

    private void FaceToTargetPos(Transform transform, Vector3 facePos)
    {
        Vector3 dir = facePos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void OnUseItemOneTime(ItemType itemType)
    {
        if (itemType != ItemType.Weapon)
            return;

        GameObject bulletObject = GetBulletObject();

        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;

        bulletObject.SetActive(true);
        bulletObject.transform.position = character.position;
        FaceToTargetPos(bulletObject.transform, pos);
    }
}