public interface ICollider
{
    int Layer { get; }
    T GetComponent<T>();
}