using UnityEngine;

public class RigidBody2DComponent : MonoBehaviour, IRigidbody
{
    public Vector3 position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector2 velocity
    {
        get => GetRigidbody.velocity;
        set => GetRigidbody.velocity = value;
    }

    private Rigidbody2D rigidbody;

    private Rigidbody2D GetRigidbody
    {
        get
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody2D>();

            return rigidbody;
        }
    }

    public void AddForce(Vector2 forceVector, ForceMode2D forceMode = ForceMode2D.Force)
    {
        GetRigidbody.AddForce(forceVector, forceMode);
    }
}