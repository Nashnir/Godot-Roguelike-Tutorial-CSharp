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
            this.player.LevelComponent.LeveledUp += UpdateLabelsAsync;
            this.player.EquipmentComponent.EquipmentChanged += UpdateLabelsAsync;
            UpdateLabelsAsync();
        }

        public async void UpdateLabelsAsync()
        {
            if (!player.IsInsideTree())
                await ToSignal(player, Node.SignalName.Ready);

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