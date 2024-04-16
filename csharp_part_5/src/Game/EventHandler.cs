using Godot;

namespace SuperRogalik
{
	public partial class EventHandler : Node
	{
		public static Action GetAction()
		{
			Action action = null;

			if (Input.IsActionJustPressed("ui_cancel")) action = new EscapeAction();
			else if (Input.IsActionJustPressed("ui_up")) action = new BumpAction(0, -1);
			else if (Input.IsActionJustPressed("ui_down")) action = new BumpAction(0, 1);
			else if (Input.IsActionJustPressed("ui_left")) action = new BumpAction(-1, 0);
			else if (Input.IsActionJustPressed("ui_right")) action = new BumpAction(1, 0);

			return action;
		}
	}
}
