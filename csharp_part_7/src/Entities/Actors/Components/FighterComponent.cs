using Godot;

namespace SuperRogalik
{
    public partial class FighterComponent : Component
    {
        [Signal]
        public delegate void HpChangedEventHandler(int hp, int hpMax);

        public int MaxHP { get; set; }
		
		private int hp;
		public int HP
        {
            get => hp;
            set
            {
                hp = Mathf.Clamp(value, 0, MaxHP);
                EmitSignal(SignalName.HpChanged, hp, MaxHP);
                if (hp <= 0) Die();
            }
        }

        public int Defense { get; set; }
		public int Power { get; set; }

        public AtlasTexture DeathTexture { get; set; }
        public Color DeathColor { get; set; }


        public FighterComponent() { }

        public FighterComponent(FighterComponentDefinition definition) 
		{
			MaxHP = definition.MaxHp;
			HP = definition.MaxHp;
			Defense = definition.Defense;
			Power = definition.Power;
            DeathTexture = definition.DeathTexture;
            DeathColor = definition.DeathColor;
        }

        public void Die()
        {
            string deathMessage;
            Color deathColor;
            if (GetMapData().Player == Entity)
            {
                deathMessage = "You died!";
                deathColor = GameColors.PLAYER_DIE;
                SignalBus.Instance?.EmitSignal(SignalBus.SignalName.PlayerDied);
            }
            else
            {
                deathMessage = $"{Entity.EntityName} is dead!";
                deathColor = GameColors.ENEMY_DIE;
            }

            SignalBus.Instance?.EmitSignal(SignalBus.SignalName.MessageSent, deathMessage, deathColor);

            Entity.Texture = DeathTexture;
            Entity.Modulate = DeathColor;
            Entity.AIComponent.QueueFree();
            Entity.AIComponent = null;
            Entity.EntityName = $"Remains of {Entity.EntityName}";
            Entity.BlocksMovement = false;
            GetMapData().UnregisterBlockingEntity(Entity);
            Entity.EntityType = EntityType.CORPSE;
        }
    }
}
