using UnityEngine;

public class DeltaTimeGetter : IDeltaTimeGetter
{
    public float deltaTime => Time.deltaTime;
}