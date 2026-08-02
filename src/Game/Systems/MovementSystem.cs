using Core.Ecs;

public class MovementSystem{

    private World _world;

    public MovementSystem(World world)
    {
        _world = world;
    }

    public void Move(float dt){
        var moveEntities = _world.Query<Movement>();

        foreach(var (entity, move) in moveEntities)
        {
            if (_world.HasComponent<Transform>(entity))
            {
                ref var transform = ref _world.GetComponent<Transform>(entity);
                transform.Pos += move.Dir * move.Speed * dt;
            }
        }
    }
}
