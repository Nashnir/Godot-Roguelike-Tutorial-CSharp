using Godot;

namespace SuperRogalik
{
    public partial class FighterComponent : Component
    {
		public int MaxHP { get; set; }
		
		private int hp;
		public int HP
        {
            get => hp;
            set
            {
                hp = Mathf.Clamp(value, 0, MaxHP);
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
            if (GetMapData().Player == Entity)
            {
                deathMessage = "You died!";

                var signalBus = GetNode<SignalBus>("/root/SignalBus");
                signalBus.EmitSignal(SignalBus.SignalName.PlayerDied);
            }
            else
            {
                deathMessage = $"{Entity.EntityName} is dead!";
            }

            GD.Print(deathMessage);

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
