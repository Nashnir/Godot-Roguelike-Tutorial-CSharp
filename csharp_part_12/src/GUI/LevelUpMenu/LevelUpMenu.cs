using Godot;

namespace SuperRogalik
{
	public partial class LevelUpMenu : CanvasLayer
	{
		[Signal]
		public delegate void LevelUpCompletedEventHandler();

		public Entity Player { get; set; }

		Button HealthUpgradeButton, PowerUpgradeButton, DefenseUpgradeButton;
		
		public override void _Ready()
		{
			HealthUpgradeButton = GetNode<Button>("%HealthUpgradeButton");
			PowerUpgradeButton = GetNode<Button>("%PowerUpgradeButton");
            DefenseUpgradeButton = GetNode<Button>("%DefenseUpgradeButton");
        }

		public void Setup(Entity player)
		{
			Player = player;
			var fighter = player.FighterComponent;
			HealthUpgradeButton.Text = $"(a) Constitution (+20 HP, from {fighter.MaxHP})";
			PowerUpgradeButton.Text = $"(b) Strength (+1 attack, from {fighter.Power})";
			DefenseUpgradeButton.Text = $"(c) Agility (+1 defense, from {fighter.Defense})";
			HealthUpgradeButton.GrabFocus();
		}

		public void OnHealthUpgradeButtonPressed()
		{
			Player.LevelComponent.IncreaseMaxHp();
			QueueFree();
			EmitSignal(SignalName.LevelUpCompleted);
		}

		public void OnPowerUpgradeButtonPressed()
		{
			Player.LevelComponent.IncreasePower();
			QueueFree();
            EmitSignal(SignalName.LevelUpCompleted);
        }

        public void OnDefenseUpgradeButtonPressed()
		{
			Player.LevelComponent.IncreaseDefense();
			QueueFree();
			EmitSignal(SignalName.LevelUpCompleted);
		}
    }
}