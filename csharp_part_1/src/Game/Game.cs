using Godot;
using SuperRogalik.src.utils;

public partial class Game : Node2D
{
	Vector2I playerGridPos = Vector2I.Zero;
	Sprite2D player;
	EventHandler eventHandler;

    public override void _Ready()
	{
		player = GetNode<Sprite2D>("Player");
		eventHandler = GetNode<EventHandler>("EventHandler");
    }

	public override void _Process(double delta)
    {
		var action = eventHandler.GetAction();

		if (action is MovementAction movementAction) {
			playerGridPos += movementAction.Offset;
			player.Position = Grid.GridToWorld(playerGridPos);
		}
		else if (action is EscapeAction)
		{
			GetTree().Quit();
		}
    }
}
