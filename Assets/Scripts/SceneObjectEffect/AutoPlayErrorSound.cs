using SNShien.Common.AudioTools;
using UnityEngine;
using Zenject;

public class AutoPlayErrorSound : MonoBehaviour
{
    [Inject] private IAudioManager audioManager;

    public void OnEnable()
    {
        audioManager.PlayOneShot(GameConst.AUDIO_KEY_ERROR);
    }
}