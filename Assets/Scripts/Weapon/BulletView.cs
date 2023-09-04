using System;
using UnityEngine;

public class BulletView : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float disappearTime;

    private float timer;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy)
            transform.Translate(Vector3.up * Time.deltaTime * speed);

        if (timer > disappearTime)
            Hide();
        else
            timer += Time.deltaTime;
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        timer = 0;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == (int)GameConst.GameObjectLayerType.Monster)
            Hide();
    }
}