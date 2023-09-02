using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    
    private CharacterMoveModel characterMoveModel;

    private void Start()
    {
        characterMoveModel = new CharacterMoveModel(new CharacterMoveController(), new CharacterKeyController());

        SetEventRegister();
    }

    private void Update()
    {
        characterMoveModel.UpdateCheckJump(jumpForce);
        characterMoveModel.UpdateMove(Time.deltaTime, speed);
    }

    private void SetEventRegister()
    {
        characterMoveModel.OnHorizontalMove -= OnHorizontalMove;
        characterMoveModel.OnHorizontalMove += OnHorizontalMove;
    }

    private void OnHorizontalMove(float moveValue)
    {
        transform.Translate(new Vector2(moveValue, 0));
    }
}