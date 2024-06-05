using Godot;

namespace SuperRogalik
{
    public partial class MainMenu : Control
    {
        [Signal]
        public delegate void GameRequestedEventHandler(bool load);

        public Button FirstButton { get; set; }
        public Button LoadButton { get; set; }

        public override void _Ready()
        {
            FirstButton = GetNode<Button>("%NewButton");
            LoadButton = GetNode<Button>("%LoadButton");

            FirstButton.GrabFocus();

            var hasSaveFile = FileAccess.FileExists("user://save_game.dat");
            LoadButton.Disabled = !hasSaveFile;
        }

        public void OnNewButtonPressed() =>
            EmitSignal(SignalName.GameRequested, false);

        public void OnLoadButtonPressed() =>
            EmitSignal(SignalName.GameRequested, true);

        public void OnQuitButtonPressed() =>
            GetTree().Quit();

    }
}