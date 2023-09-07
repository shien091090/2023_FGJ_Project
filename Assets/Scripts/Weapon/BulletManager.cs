using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform objHolder;
    [SerializeField] private CharacterView character;

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

    private void FaceToCharacterFaceDirection(GameObject bulletObject)
    {
        if (character.IsFaceRight)
            bulletObject.transform.rotation = Quaternion.Euler(0, 0, -90);
        else
            bulletObject.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    private void OnUseItemOneTime(ItemType itemType)
    {
        if (itemType != ItemType.Weapon)
            return;

        GameObject bulletObject = GetBulletObject();

        bulletObject.SetActive(true);
        bulletObject.transform.position = character.position;
        FaceToCharacterFaceDirection(bulletObject);
    }
}