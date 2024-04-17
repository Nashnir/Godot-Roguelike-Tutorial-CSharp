using Godot;
using System.Collections.Generic;

namespace SuperRogalik
{
    public partial class MainGameInputHandler : BaseInputHandler
    {
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

        public override Action GetAction(Entity player)
        {
            Action action = null;

            foreach (var direction in directions.Keys)
            {
                if (Input.IsActionJustPressed(direction))
                {
                    var offset = directions[direction];
                    action = new BumpAction(player, offset.X, offset.Y);
                }
            }
            if (Input.IsActionJustPressed("wait")) action = new WaitAction(player);
            if (Input.IsActionJustPressed("quit")) action = new EscapeAction(player);

            return action;
        }
    }
}
