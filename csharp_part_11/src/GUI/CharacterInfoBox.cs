using Godot;

namespace SuperRogalik
{
	public partial class CharacterInfoBox : HBoxContainer
	{
        Entity player;

        Label levelLabel;
        Label attackLabel;
        Label defenseLabel;

        public void Setup(Entity player)
        {
            this.player = player;
            this.player.LevelComponent.LeveledUp += UpdateLabels;
            UpdateLabels();
        }

        public void UpdateLabels()
        {
            levelLabel.Text = $"LVL: {player.LevelComponent.CurrentLevel}";
            attackLabel.Text = $"ATK: {player.FighterComponent.Power}";
            defenseLabel.Text = $"DEF: {player.FighterComponent.Defense}";
        }

        public override void _Ready()
		{
            levelLabel = GetNode<Label>("LevelLabel");
            attackLabel = GetNode<Label>("AttackLabel");
            defenseLabel = GetNode<Label>("DefenseLabel");
        }
	}
}