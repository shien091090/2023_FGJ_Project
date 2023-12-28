using System;
using UnityEngine;

public class BackgroundParallaxEffect : MonoBehaviour
{
    [SerializeField] private Transform background;
    [SerializeField] private Vector2 sceneUpRightPos;
    [SerializeField] private Vector2 sceneDownLeftPos;
    [SerializeField] private Vector2 backgroundUpRightPos;
    [SerializeField] private Vector2 backgroundDownLeftPos;
    private float SceneRightPosX => sceneUpRightPos.x;
    private float SceneLeftPosX => sceneDownLeftPos.x;
    private float BackgroundRightPosX => backgroundUpRightPos.x;
    private float BackgroundLeftPosX => backgroundDownLeftPos.x;
    private float SceneUpPosY => sceneUpRightPos.y;
    private float SceneDownPosY => sceneDownLeftPos.y;
    private float BackgroundUpPosY => backgroundUpRightPos.y;
    private float BackgroundDownPosY => backgroundDownLeftPos.y;
    private Vector3 CameraPos => Camera.main.transform.position;
    private float GetSceneWidth => SceneRightPosX - SceneLeftPosX;
    private float GetBackgroundWidth => BackgroundRightPosX - BackgroundLeftPosX;
    private float GetSceneHeight => SceneUpPosY - SceneDownPosY;
    private float GetBackgroundHeight => BackgroundUpPosY - BackgroundDownPosY;

    private void Update()
    {
        float backgroundPosX = BackgroundRightPosX + CameraPos.x - GetBackgroundWidth * (CameraPos.x - SceneLeftPosX) / GetSceneWidth;
        float backgroundPosY = BackgroundUpPosY + CameraPos.y - GetBackgroundHeight * (CameraPos.y - SceneDownPosY) / GetSceneHeight;
        background.position = new Vector3(backgroundPosX, backgroundPosY, background.position.z);
    }
}