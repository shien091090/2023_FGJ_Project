using UnityEngine;
using Zenject;

public class SpaceKeyHint : MonoBehaviour
{
    [SerializeField] private GameObject go_effect;

    [Inject] private ICharacterModel characterModel;

    void Start()
    {
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        characterModel.OnTriggerInteractiveObject -= OnTriggerInteractiveObject;
        characterModel.OnTriggerInteractiveObject += OnTriggerInteractiveObject;

        characterModel.OnUnTriggerInteractiveObject -= OnUnTriggerInteractiveObject;
        characterModel.OnUnTriggerInteractiveObject += OnUnTriggerInteractiveObject;

        characterModel.OnChangeCharacterState -= OnChangeCharacterState;
        characterModel.OnChangeCharacterState += OnChangeCharacterState;
    }

    private void OnChangeCharacterState(CharacterState newState)
    {
        if (newState == CharacterState.Die ||
            go_effect.activeSelf && newState == CharacterState.IntoHouse)
            go_effect.SetActive(false);
    }

    private void OnUnTriggerInteractiveObject()
    {
        go_effect.SetActive(false);
    }

    private void OnTriggerInteractiveObject()
    {
        go_effect.SetActive(true);
    }
}