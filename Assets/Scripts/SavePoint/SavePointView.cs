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

    private SavePointModel savePointModel;

    public void ShowRecordStateHint()
    {
        savePointModel.ShowRecordStateHint();
    }

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

    public void Save()
    {
        savePointModel.Save();
    }

    public void RefreshRecordState(bool isRecorded)
    {
        Debug.Log($"RefreshRecordState: {isRecorded}");
        sp_recordStateHint.sprite = isRecorded ?
            recordTypeSprite :
            notRecordTypeSprite;
    }

    public void HideAllUI()
    {
        sp_recordStateHint.gameObject.SetActive(false);
        go_leftArrow.SetActive(false);
        go_rightArrow.SetActive(false);
    }

    private void Start()
    {
        savePointModel = new SavePointModel(this, SavePointManager.Instance, FmodAudioManager.Instance);
    }
}