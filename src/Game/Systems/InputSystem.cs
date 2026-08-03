using Raylib_cs;
using RL = Raylib_cs.Raylib;

public enum Input
{
    Left, Right, F2, F5
}

public class InputSystem
{
    public Dictionary<Input, bool> Pressed { get => _pressed; set => _pressed = value; }
    public float MouseWheel { get => _mouseWheel; set => _mouseWheel = value; }
    private Dictionary<Input, bool> _pressed;
    private float _mouseWheel;

    public InputSystem()
    {
        _pressed = new();
        var values = Enum.GetValues<Input>().Cast<Input>();
        foreach(var value in values)
        {
            _pressed[value] = false;
        }
    }

    public void HandleInput()
    {
        _pressed[Input.Left] = RL.IsMouseButtonPressed(MouseButton.Left);
        _pressed[Input.Right] = RL.IsMouseButtonPressed(MouseButton.Right);
        _pressed[Input.F2] = RL.IsKeyPressed(KeyboardKey.F2);
        _pressed[Input.F5] = RL.IsKeyPressed(KeyboardKey.F5);

        _mouseWheel = RL.GetMouseWheelMove();
    }
}
