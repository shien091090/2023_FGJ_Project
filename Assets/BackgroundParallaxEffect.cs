using System;
using UnityEngine;

public class BackgroundParallaxEffect : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sp_background;
    [SerializeField] private Vector2 sceneRightUpPos;
    [SerializeField] private Vector2 sceneLeftDownPos;
    [SerializeField] private Vector2 backgroundRightUpPos;
    [SerializeField] private Vector2 backgroundLeftDownPos;
    [SerializeField] private float parallaxSpeed; // 背景移動速度 (0沒有移動，1與角色相同速度)

    // public Transform cameraTransform; // 攝影機的Transform

    private Vector3 lastCameraPosition; // 上一幀攝影機的位置
    private Vector3 initialPosition; // 初始位置

    private Transform cameraTransform => Camera.main.transform;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
        initialPosition = transform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition; // 攝影機從上一幀到這一幀的移動差

        // 計算背景的移動，但只考慮x和y
        Vector3 parallaxPosition = new Vector3(initialPosition.x + deltaMovement.x * parallaxSpeed, initialPosition.y + deltaMovement.y * parallaxSpeed,
            transform.position.z);

        float backgroundWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        float backgroundHeight = GetComponent<SpriteRenderer>().bounds.size.y;
        float camWidth = cameraTransform.GetComponent<Camera>().orthographicSize * cameraTransform.GetComponent<Camera>().aspect;
        float camHeight = cameraTransform.GetComponent<Camera>().orthographicSize;

        parallaxPosition.x = Mathf.Clamp(parallaxPosition.x, initialPosition.x - (backgroundWidth - camWidth), initialPosition.x + (backgroundWidth - camWidth));
        parallaxPosition.y = Mathf.Clamp(parallaxPosition.y, initialPosition.y - (backgroundHeight - camHeight), initialPosition.y + (backgroundHeight - camHeight));

        transform.position = parallaxPosition; // 更新背景位置

        lastCameraPosition = cameraTransform.position; // 更新攝影機的位置
    }
}