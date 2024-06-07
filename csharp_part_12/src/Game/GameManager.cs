using Godot;

namespace SuperRogalik
{
    public partial class GameManager : Node
	{
		public PackedScene GameScene { get; set; }
        public PackedScene MainMenuScene { get; set; }
		
		Node CurrentChild { get; set; }

        public override void _Ready()
		{
			GameScene = ResourceLoader.Load<PackedScene>("res://src/Game/game.tscn");
			MainMenuScene = ResourceLoader.Load<PackedScene>("res://src/GUI/MainMenu/main_menu.tscn");

			LoadMainMenu();
        }

        public Node SwitchToScene(PackedScene scene)
		{
            DisconnectSignals();
			CurrentChild?.QueueFree();

			CurrentChild = scene.Instantiate();
			AddChild(CurrentChild);
			return CurrentChild;
        }

		public void LoadMainMenu()
		{
			var mainMenu = (MainMenu)SwitchToScene(MainMenuScene);
			mainMenu.GameRequested += OnGameRequested;
		}

        private void OnGameRequested(bool load)
        {
            var game = (GameRoot)SwitchToScene(GameScene);
			game.MainMenuRequested += LoadMainMenu;

			if (load)
				game.LoadGame();
			else
				game.NewGame();
        }

        protected override void Dispose(bool disposing)
        {
            DisconnectSignals();
            base.Dispose(disposing);
        }

        private void DisconnectSignals()
        {
            if (CurrentChild is GameRoot gr)
                gr.MainMenuRequested -= LoadMainMenu;
            else if (CurrentChild is MainMenu mm)
                mm.GameRequested -= OnGameRequested;
        }
    }
}