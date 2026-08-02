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
        }

        private List<EntityId> _entities;
        private World _world;
        private RenderSystem _renderSystem;
        private MovementSystem _moveSystem;

        public void InitGame()
        {
            RL.InitWindow(1920, 1080, "Test");
            _entities = new();
            _world = new();
            _renderSystem = new(_world);
            _moveSystem = new(_world);
        }

        public void GameLoop()
        {
            var test = _world.CreateEntity();
            Transform transform = new(){
                Pos = new(200, 200),
                Rotation = 0,
                Scale = new(1, 1)
            };
            Circle circle = new()
            {
                Radius = 50,
                Color = Color.Red,
            };
            _world.AddComponent(test, transform);
            _world.AddComponent(test, circle);
            _world.AddComponent(test, new Movement { Dir = new(1, 1), Speed = 200 });

            var otherTest = _world.CreateEntity();
            _world.AddComponent(otherTest, new Transform { Pos = new(800, 500), Rotation = 0, Scale = new(1, 1) });
            _world.AddComponent(otherTest, new Rec { Width = 250, Height = 125, Color = Color.Orange });
            while (!RL.WindowShouldClose())
            {
                float dt = RL.GetFrameTime();
                _moveSystem.Move(dt);
                RL.BeginDrawing();
                RL.ClearBackground(Color.DarkGray);
                _renderSystem.Draw();
                RL.EndDrawing();
            }
        }
    }
}
