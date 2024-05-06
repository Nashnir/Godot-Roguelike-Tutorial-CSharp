using Godot;

namespace SuperRogalik
{
    public partial class HpDisplay : MarginContainer
    {
        public ProgressBar hpBar;
        public Label hpLabel;
        private bool isReady = false;

        public override void _Ready()
        {
            hpBar = GetNode<ProgressBar>("%HpBar");
            hpLabel = GetNode<Label>("%HpLabel");
        }

        public void PlayerHpChanged(int hp, int maxHp)
        {
            hpBar.MaxValue = maxHp;
            hpBar.Value = hp;
            hpLabel.Text = $"{hp}/{maxHp}";
        }

        public async void Initialize(Entity player)
        {
            await ToSignal(this, Node.SignalName.Ready);
            player.FighterComponent.HpChanged += PlayerHpChanged;
            var playerHp = player.FighterComponent.HP;
            var playerMaxHp = player.FighterComponent.MaxHP;
            PlayerHpChanged(playerHp, playerMaxHp);
        }
    }
}