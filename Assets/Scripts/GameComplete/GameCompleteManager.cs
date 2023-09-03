using System.Collections;
using UnityEngine;

public class GameCompleteManager : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private string ANIM_KEY_IDLE = "game_complete_idle";
    private string ANIM_KEY_GAME_COMPLETED = "game_complete";

    private void Start()
    {
        anim.Play(ANIM_KEY_IDLE);
        MissingTextureManager.OnMissingTextureAllClear -= OnMissingTextureAllClear;
        MissingTextureManager.OnMissingTextureAllClear += OnMissingTextureAllClear;
    }

    private void OnMissingTextureAllClear()
    {
        anim.Play(ANIM_KEY_GAME_COMPLETED);
        StartCoroutine(Cor_GameComplete());
    }
    
    private IEnumerator Cor_GameComplete()
    {
        yield break;
    }
}