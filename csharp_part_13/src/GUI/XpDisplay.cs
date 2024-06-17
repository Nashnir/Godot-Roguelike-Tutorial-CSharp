using Godot;

namespace SuperRogalik
{
	public partial class XpDisplay : MarginContainer
	{
		ProgressBar xpBar;
		Label xpLabel;

        public override void _Ready()
        {
            xpBar = GetNode<ProgressBar>("%XpBar");
            xpLabel = GetNode<Label>("%XpLabel");
        }

        public async void Initialize(Entity player)
		{
			if(!IsInsideTree())
			{
                await ToSignal(this, Node.SignalName.Ready);
			}
			player.LevelComponent.XpChanged += PlayerXpChanged;
			var playerXp = player.LevelComponent.CurrentXp;
			var playerMaxXp = player.LevelComponent.GetExperienceToNextLevel();
			PlayerXpChanged(playerXp, playerMaxXp);
		}

		public void PlayerXpChanged(int xp, int maxXp)
		{
			xpBar.MaxValue = maxXp;
			xpBar.Value = xp;
			xpLabel.Text = $"XP: {xp} / {maxXp}";
		}		
	}
}