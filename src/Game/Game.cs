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

            var wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(1200, 500))
                  .AddComponent(wall, new Rec(50, 500, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(50, 500, 0, PhysicsType.Static));
            wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(700, 1000))
                  .AddComponent(wall, new Rec(500, 50, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(500, 50, 0, PhysicsType.Static));
            wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(700, 450))
                  .AddComponent(wall, new Rec(500, 50, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(500, 50, 0, PhysicsType.Static));
            wall = _world.CreateEntity();
            _world.AddComponent(wall, new Transform(650, 500))
                  .AddComponent(wall, new Rec(50, 500, Color.Orange))
                  .AddComponent(wall, PhysicsBody.CreateRecBody(50, 500, 0, PhysicsType.Static));

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
                          .AddComponent(sphere, new Movement(Utils.GetRandomDirection(), 100))
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
