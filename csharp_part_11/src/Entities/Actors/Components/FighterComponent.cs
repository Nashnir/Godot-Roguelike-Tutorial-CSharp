using Godot;
using Godot.Collections;
using System.Threading;

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
                if (hp <= 0) 
                {
                    var awaiter = ToSignal(this, Node.SignalName.Ready);
                    bool deadOnArrival = !IsInsideTree();
                    new Thread(async () => 
                    {
                        if (deadOnArrival)
                            await awaiter;
                        
                        CallDeferred(MethodName.Die, !deadOnArrival);
                    }).Start();
                }
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

        public void Die(bool triggerSideEffects = true)
        {
            string deathMessage;
            Color deathColor;
            if (GetMapData().Player == Entity)
            {
                deathMessage = "You died!";
                deathColor = GameColors.PLAYER_DIE;
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerDied);
            }
            else
            {
                deathMessage = $"{Entity.EntityName} is dead!";
                deathColor = GameColors.ENEMY_DIE;
            }

            if (triggerSideEffects)
            {
                MessageLog.SendMessage(deathMessage, deathColor);
                GetMapData().Player.LevelComponent.AddXp(Entity.LevelComponent.XpGiven);
            }
            Entity.Texture = DeathTexture;
            Entity.Modulate = DeathColor;
            Entity.AIComponent.QueueFree();
            Entity.AIComponent = null;
            Entity.EntityName = $"Remains of {Entity.EntityName}";
            Entity.BlocksMovement = false;
            GetMapData().UnregisterBlockingEntity(Entity);
            Entity.EntityType = EntityType.CORPSE;
        }

        public int Heal(int amount)
        {
            if (HP == MaxHP)
                return 0;

            var newHpValue = HP + amount;
            if (newHpValue > MaxHP)
                newHpValue = MaxHP;

            var amountRecovered = newHpValue - HP;
            HP = newHpValue;
            return amountRecovered;
        }

        public void TakeDamage(int amount) =>
            HP -= amount;

        public Dictionary GetSaveData()
        {
            return new Dictionary(){
                { "max_hp", MaxHP },
                { "hp", HP },
                { "power", Power },
                { "defense", Defense }
            };
        }

        public void Restore(Dictionary saveData)
        {
            MaxHP = saveData["max_hp"].AsInt32();
            HP = saveData["hp"].AsInt32();
            Power = saveData["power"].AsInt32();
            Defense = saveData["defense"].AsInt32();
        }
    }
}
