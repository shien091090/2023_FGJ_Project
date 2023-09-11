using UnityEngine;

public class TimeModel : ITimeModel
{
    public float deltaTime => Time.deltaTime;
}