using UnityEngine;

public class CameraFollowComponent : MonoBehaviour
{
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private float smoothTime;

    public void Update()
    {
        Vector2 pos = Vector2.SmoothDamp(transform.position, followTarget.position, ref velocity, smoothTime);
        transform.position = new Vector3(pos.x, followTarget.position.y, transform.position.z);
    }
}