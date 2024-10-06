using Godot;

public class CameraFollow2D : Camera2D
{
    [Export]
    public NodePath _targetPath;


    private Node2D _target;

    private float speed = 200f;

    // yOffset of camera from target
    private float yOffset = 0.0f;

    // In viewport space
    [Export]
    private float _horizontalPadding = 0.3f;
    [Export]
    private float _verticalPadding = 0.3f;


    // Non-grid-aligned position
    private Vector2 _unadjustedPosition;

    public override void _Ready()
    {
        _target = GetNode<Node2D>(_targetPath);
        Position = new Vector2(_target.Position.x, _target.Position.y + yOffset) - GetViewportRect().Size / 2.0f;
        _unadjustedPosition = Position;
    }

    public override void _Process(float delta)
    {

        var topLeft = GetCameraScreenCenter() - GetViewportRect().Size / 2.0f;
        var point = new Vector2((_target.Position.x - topLeft.x) / GetViewportRect().Size.x, (_target.Position.y - topLeft.y) / GetViewportRect().Size.y);


        var deltaX = 0.0f;
        var deltaY = 0.0f;

        if (point.x > 1.0f - _horizontalPadding)
        {
            deltaX = point.x - (1 - _horizontalPadding);

        }
        else if (point.x < _horizontalPadding)
        {
            deltaX = point.x - _horizontalPadding;
        }

        if (point.y > 1.0f - _verticalPadding)
        {
            deltaY = point.y - (1 - _verticalPadding);

        }
        else if (point.y < _verticalPadding)
        {
            deltaY = point.y - _verticalPadding;
        }

        var viewportPoint = new Vector2(0.5f, 0.5f) + new Vector2(deltaX, deltaY);
        var destination = topLeft + viewportPoint * GetViewportRect().Size - GetViewportRect().Size / 2.0f;

        _unadjustedPosition = Position.MoveToward(destination, delta * speed);

        Position = _unadjustedPosition;
    }
}
