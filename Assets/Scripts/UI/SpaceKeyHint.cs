using UnityEngine;

public class SpaceKeyHint : MonoBehaviour
{
    [SerializeField] private GameObject go_effect;

    private ICharacterModel characterModel;

    void Start()
    {
        characterModel = CharacterModel.Instance;

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