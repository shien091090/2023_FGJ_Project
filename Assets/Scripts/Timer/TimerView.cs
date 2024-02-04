using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TimerView : MonoBehaviour
{
    [SerializeField] private Text txt_timer;

    [Inject] private ITimerModel timerModel;
    [Inject] private IMissingTextureManager missingTextureManager;

    private void Start()
    {
        RegisterEvent();
        timerModel.StartTimer();
    }

    private void Update()
    {
        timerModel.UpdateTimer(Time.deltaTime);
    }

    private void RegisterEvent()
    {
        timerModel.OnUpdateTimer -= OnUpdateTimer;
        timerModel.OnUpdateTimer += OnUpdateTimer;

        missingTextureManager.OnMissingTextureAllClear -= OnMissingTextureAllClear;
        missingTextureManager.OnMissingTextureAllClear += OnMissingTextureAllClear;
    }

    private void OnMissingTextureAllClear()
    {
        timerModel.SetPause(true);
    }

    private void OnUpdateTimer(TimerUpdateEventInfo eventInfo)
    {
        txt_timer.text = eventInfo.GetTimerString(TimerStringFormatType.MMSS);
    }
}