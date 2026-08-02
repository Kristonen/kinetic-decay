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
            _world.AddComponent(test, new Transform(200, 200));
            _world.AddComponent(test, new Circle(50, Color.Red));
            _world.AddComponent(test, new Movement { Dir = new(1, 1), Speed = 200 });

            var otherTest = _world.CreateEntity();
            _world.AddComponent(otherTest, new Transform(800 ,500));
            _world.AddComponent(otherTest, new Rec(250, 125, Color.Orange));

            var nextTest = _world.CreateEntity();
            _world.AddComponent(nextTest, new Transform(RL.GetScreenWidth() / 2 - 50, RL.GetScreenHeight()));
            _world.AddComponent(nextTest, new Rec(100, 50, Color.Beige));
            _world.AddComponent(nextTest, new Movement(new(0, -1), 200));

            while (!RL.WindowShouldClose())
            {
                float dt = RL.GetFrameTime();
                _updater.Update(dt);
                RL.BeginDrawing();
                RL.ClearBackground(Color.DarkGray);
                _renderer.Draw();
                RL.DrawFPS(25, 25);
                RL.EndDrawing();
            }
        }
    }
}
