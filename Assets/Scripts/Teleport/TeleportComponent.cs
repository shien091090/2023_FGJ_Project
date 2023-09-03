using SNShien.Common.AudioTools;
using UnityEngine;

public class TeleportComponent : MonoBehaviour, ITeleport
{
    [SerializeField] private Transform character;
    private Vector3 originPos;

    public void BackToOrigin()
    {
        FmodAudioManager.Instance.PlayOneShot("Teleport");
        character.position = originPos;
    }

    private void Start()
    {
        originPos = character.position;
    }
}