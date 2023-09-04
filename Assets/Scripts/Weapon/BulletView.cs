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
        {
            gameObject.SetActive(false);
            timer = 0;
        }
        else
            timer += Time.deltaTime;
    }
}