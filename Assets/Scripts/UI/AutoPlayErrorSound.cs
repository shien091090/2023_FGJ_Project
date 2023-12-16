using SNShien.Common.AudioTools;
using UnityEngine;

public class AutoPlayErrorSound : MonoBehaviour
{
    public void OnEnable()
    {
        FmodAudioManager.Instance.PlayOneShot(GameConst.AUDIO_KEY_ERROR);
    }
}