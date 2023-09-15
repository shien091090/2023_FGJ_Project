using System;
using SNShien.Common.AudioTools;
using UnityEngine;

public class TeleportComponent : MonoBehaviour, ITeleport
{
    [SerializeField] private Rigidbody2D character;
    [SerializeField] private float bottomPos;

    private Vector3 originPos;

    public void BackToOrigin()
    {
        FmodAudioManager.Instance.PlayOneShot(GameConst.AUDIO_KEY_TELEPORT);
        character.position = originPos;
        character.velocity = Vector2.zero;
    }

    private void Update()
    {
        if (character.transform.position.y < bottomPos)
            BackToOrigin();
    }

    private void Start()
    {
        originPos = character.position;
    }
}