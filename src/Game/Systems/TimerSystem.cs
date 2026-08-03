using Core.Ecs;

public class TimerSystem : AbstractUpdateSystem
{
    public TimerSystem(World world) : base(world) { }

    public override void Update(float dt)
    {
        foreach (var (entityId, timer) in _world.Query<Timer>())
        {
            ref var timerRef = ref _world.GetComponent<Timer>(entityId);
            if (!timerRef.Active) continue;
            timerRef.TimeRemaining -= dt;
            if (timerRef.TimeRemaining <= timerRef.MinInterval)
            {
                timerRef.Trigger(entityId);
                if (timerRef.OneTime)
                {
                    timerRef.Active = false;
                    continue;
                }
                timerRef.TimeRemaining = timerRef.MaxInterval;
            }
        }
    }
}
