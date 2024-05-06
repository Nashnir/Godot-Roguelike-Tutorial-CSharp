using Godot;
using SuperRogalik;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class Reticle : Node2D
{
    [Signal]
    public delegate void PositionSelectedEventHandler(Vector2I gridPosition);

    public readonly Dictionary<string, Vector2I> directions = new()
    {
        {"move_up",  Vector2I.Up},
        {"move_down", Vector2I.Down},
        {"move_left", Vector2I.Left},
        {"move_right", Vector2I.Right},
        {"move_up_left", Vector2I.Up + Vector2I.Left},
        {"move_up_right", Vector2I.Up + Vector2I.Right},
        {"move_down_left", Vector2I.Down + Vector2I.Left},
        {"move_down_right", Vector2I.Down + Vector2I.Right},
    };

    public Vector2I gridPosition;
    public Vector2I GridPosition
    {
        get => gridPosition;
        set
        {
            gridPosition = value;
            Position = Grid.GridToWorld(gridPosition);
        }
    }

    public MapData MapData { get; set; }

    public Camera2D Camera { get; set; }

    public Line2D Border { get; set; }

    public override void _Ready()
    {
        Camera = GetNode<Camera2D>("Camera2D");
        Border = GetNode<Line2D>("Line2D");
        Hide();
        SetPhysicsProcess(false);
    }

    public async Task<Vector2I> SelectPositionAsync(Entity player, int radius)
    {
        MapData = player.MapData;
        GridPosition = player.GridPosition;

        var playerCamera = GetViewport().GetCamera2D();
        Camera.MakeCurrent();
        Show();
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);

        CallDeferred(Node.MethodName.SetPhysicsProcess, true);
        var selectedPosition = await ToSignal(this, SignalName.PositionSelected);
        SetPhysicsProcess(false);

        playerCamera.MakeCurrent();
        Hide();

        return (Vector2I)selectedPosition[0];
    }

    public override void _PhysicsProcess(double delta)
    {
        var offset = Vector2I.Zero;
        foreach (var direction in directions.Keys)
        {
            if (Input.IsActionJustPressed(direction))
                offset += directions[direction];
        }
        GridPosition += offset;

        if (Input.IsActionJustPressed("ui_accept"))
        {
            EmitSignal(SignalName.PositionSelected, gridPosition);
        }

        if (Input.IsActionJustPressed("ui_back"))
        {
            EmitSignal(SignalName.PositionSelected, new Vector2I(-1, -1));
        }
    }

    private void SetupBorder(int radius)
    {
        if (radius <= 0)
            Border.Hide();
        else
        {
            Border.Points = [
                new Vector2I(-radius, -radius) * Grid.tileSize,
                new Vector2I(-radius, radius + 1) * Grid.tileSize,
                new Vector2I(radius + 1, radius + 1) * Grid.tileSize,
                new Vector2I(radius + 1, -radius) * Grid.tileSize,
                new Vector2I(-radius, -radius) * Grid.tileSize,
            ];
            Border.Show();
        }
    }

}
