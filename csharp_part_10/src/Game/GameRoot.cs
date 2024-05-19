using Godot;

namespace SuperRogalik
{
    public partial class GameRoot : Control
	{
		[Signal]
		public delegate void MainMenuRequestedEventHandler();

		Game game;

		public void OnEscapeRequested() =>
			EmitSignal(SignalName.MainMenuRequested);

		public override void _Ready()
		{
			game = GetNode<Game>("%Game");
			SignalBus.Instance.EscapeRequested += OnEscapeRequested;
        }

		public void NewGame() =>
			game.NewGame();

		public void LoadGame()
		{
			if (!game.LoadGame())
				EmitSignal(SignalName.MainMenuRequested);
		}
    }
}
