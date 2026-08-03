using Core.Ecs;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

namespace Game
{
    public class GameState
    {
        public static GameState Game
        {
            get
            {
                _instance ??= new();
                return _instance;
            }
        }
        private static GameState? _instance;

        private GameState()
        {
            InitGame();

            _world = new();
            _input = new();
            _renderer = new();
            _renderer.Add(new RenderSystem(_world));
            _renderer.Add(new HelperRenderSystem(_world));
            _updater = new();
            _updater.Add(new MovementSystem(_world));
            _updater.Add(new TimerSystem(_world));
            _collider = new();
            _collider.Add(new SimpleCollision(_world));
        }

        public bool Helper { get => _helper; set => _helper = value; }
        public InputSystem InputSystem { get => _input; set => _input = value; }

        private World _world;
        private RenderPipeline _renderer;
        private UpdatePipeline _updater;
        private CollisionPipeLine _collider;
        private InputSystem _input;

        public SpatialGrid Grid;

        private bool _helper;

        public void InitGame()
        {
            RL.InitWindow(1920, 1080, "Test");
        }

        public void GameLoop()
        {
            // .AddComponent(sphere, new Timer(0, 2));
            // ref var timer = ref _world.GetComponent<Timer>(sphere);
            // timer.TimeOut += (EntityId id) =>
            // {
            //     Console.WriteLine($"Entity {id}: Zeit ist abgelaufen");
            //     if (_world.HasComponent<Movement>(id))
            //     {
            //         ref var move = ref _world.GetComponent<Movement>(id);
            //         move.Dir = Utils.GetRandomDirection();
            //     }
            // };
            float width = 1200;
            float height = 800;
            float thick = 50;
            float x = 60;
            float y = 10;
            var wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(x, y))
                  .AddComponent(wall, new Rec(width, thick, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(width, 50, 0, PhysicsType.Static));
            wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(x - thick, y + thick))
                  .AddComponent(wall, new Rec(thick, height , Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(thick, height, 0, PhysicsType.Static));
            wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(width + x, y + thick))
                  .AddComponent(wall, new Rec(thick, height, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(thick, height, 0, PhysicsType.Static));
            wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(x, y + thick + height))
                  .AddComponent(wall, new Rec(width, thick, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(width, thick, 0, PhysicsType.Static));


            while (!RL.WindowShouldClose())
            {
                float dt = RL.GetFrameTime();
                // Input
                _input.HandleInput();
                if (_input.Pressed[Input.F2]) _helper = !_helper;
                if (_input.Pressed[Input.Left])
                {
                    var sphere = _world.CreateEntity();
                    _world.AddComponent(sphere, new Transform(RL.GetMousePosition()))
                          .AddComponent(sphere, new Circle(20, Color.Red))
                          .AddComponent(sphere, new Sphere())
                          .AddComponent(sphere, new Movement(Utils.GetRandomDirection(), 500))
                          .AddComponent(sphere, PhysicsBody.CreateCircleBody(20, 0))
                          .AddComponent(sphere, new Timer(0, 2));
                    ref var timer = ref _world.GetComponent<Timer>(sphere);
                    timer.TimeOut += (EntityId id) =>
                    {
                        if (_world.HasComponent<Movement>(id))
                        {
                            ref var move = ref _world.GetComponent<Movement>(id);
                            move.Dir = Utils.GetRandomDirection();
                        }
                    };
                }
                if (_input.Pressed[Input.Right])
                {
                    foreach (var (entityId, transform, circle) in _world.Query<Transform, Circle>())
                    {
                        if (RL.CheckCollisionPointCircle(RL.GetMousePosition(), transform.Pos, circle.Radius))
                        {
                            _world.DestroyEntity(entityId);
                        }
                    }
                }
                if (_input.Pressed[Input.F5]) Console.WriteLine("Test");
                // Update
                _updater.Update(dt);
                // Collision
                _collider.Collision(dt);
                // Camera

                // Draw
                RL.BeginDrawing();
                RL.ClearBackground(Color.DarkGray);
                _renderer.Draw();
                RL.DrawFPS(25, 25);
                RL.EndDrawing();
            }
        }
    }
}
