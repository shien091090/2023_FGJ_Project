using System;
using SNShien.Common.AudioTools;
using UnityEngine;

public class SavePointView : MonoBehaviour, ISavePointView
{
    [SerializeField] private SpriteRenderer sp_recordStateHint;
    [SerializeField] private GameObject go_leftArrow;
    [SerializeField] private GameObject go_rightArrow;
    [SerializeField] private Sprite recordTypeSprite;
    [SerializeField] private Sprite notRecordTypeSprite;

    public Vector3 SavePointPos => transform.position;
    public ISavePointModel GetModel => savePointModel;

    private SavePointModel savePointModel;

    public void SetRecordStateHintActive(bool isActive)
    {
        sp_recordStateHint.gameObject.SetActive(isActive);
    }

    public void SetLeftArrowActive(bool isActive)
    {
        go_leftArrow.SetActive(isActive);
    }

    public void SetRightArrowActive(bool isActive)
    {
        go_rightArrow.SetActive(isActive);
    }

    public void RefreshRecordState(bool isRecorded)
    {
        sp_recordStateHint.sprite = isRecorded ?
            recordTypeSprite :
            notRecordTypeSprite;
    }

    private void Start()
    {
        savePointModel = new SavePointModel(this, SavePointManager.Instance, FmodAudioManager.Instance);
    }
}