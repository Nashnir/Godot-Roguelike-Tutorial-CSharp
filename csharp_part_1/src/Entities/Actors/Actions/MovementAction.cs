using Godot;

public partial class MovementAction : Action
{
    public Vector2I Offset { get; set; } 

    public MovementAction() { }

    public MovementAction(int dx, int dy)
    {
        Offset = new Vector2I(dx, dy);
    }
}
