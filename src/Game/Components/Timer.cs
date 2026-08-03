using Core.Ecs;

public struct Timer : IComponent
{
    public float MaxInterval;
    public float MinInterval;
    public float TimeRemaining;
    public bool Active;
    public bool OneTime;
    public event Action<EntityId> TimeOut;

    public Timer(float min, float max, bool oneTime = false)
    {
        MinInterval = min;
        MaxInterval = max;
        TimeRemaining = min;
        Active = true;
        OneTime = oneTime;
    }

    public void Trigger(EntityId id) => TimeOut.Invoke(id);
}
