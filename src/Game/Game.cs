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
            _renderer = new();
            _renderer.Add(new RenderSystem(_world));
            _updater = new();
            _updater.Add(new MovementSystem(_world));
        }
        private World _world;

        private RenderPipeline _renderer;
        private UpdatePipeline _updater;

        public void InitGame()
        {
            RL.InitWindow(1920, 1080, "Test");
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
                _updater.Update(dt);
                RL.BeginDrawing();
                RL.ClearBackground(Color.DarkGray);
                _renderer.Draw();
                RL.EndDrawing();
            }
        }
    }
}
