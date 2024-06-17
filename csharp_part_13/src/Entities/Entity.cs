using Godot;
using Godot.Collections;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class Entity : Sprite2D
    {
        public enum AIType { NONE, HOSTILE };

        public const string PLAYER = "player";
        public const string ORC = "orc";
        public const string TROLL = "troll";
        public const string HEALTH_POTION = "health_potion";
        public const string LIGHTING_SCROLL = "lightning_scroll";
        public const string CONFUSION_SCROLL = "confusion_scroll";
        public const string FIREBALL_SCROLL = "fireball_scroll";
        public const string DAGGER = "dagger";
        public const string SWORD = "sword";
        public const string CHAINMAIL = "chainmail";
        public const string LEATHER_ARMOR = "leather_armor";

        public static readonly Dictionary<string, string> entityTypes = new(){
            { PLAYER, "res://assets/definitions/entities/actors/entity_definition_player.tres" },
            { ORC, "res://assets/definitions/entities/actors/entity_definition_orc.tres" },
            { TROLL, "res://assets/definitions/entities/actors/entity_definition_troll.tres" },
            { HEALTH_POTION, "res://assets/definitions/entities/items/health_potion_definition.tres" },
            { LIGHTING_SCROLL, "res://assets/definitions/entities/items/lightning_scroll_definition.tres" },
            { CONFUSION_SCROLL, "res://assets/definitions/entities/items/confusion_scroll_definition.tres" },
            { FIREBALL_SCROLL, "res://assets/definitions/entities/items/fireball_scroll_definition.tres" },
            { DAGGER, "res://assets/definitions/entities/items/dagger_definition.tres" },
            { SWORD, "res://assets/definitions/entities/items/sword_definition.tres" },
            { CHAINMAIL, "res://assets/definitions/entities/items/chainmail_definition.tres" },
            { LEATHER_ARMOR, "res://assets/definitions/entities/items/leather_armor_definition.tres" },
        };

        string key;

        private Vector2I gridPosition;
        public Vector2I GridPosition
        {
            get => gridPosition;
            set
            {
                gridPosition = value;
                Position = Grid.GridToWorld(gridPosition);
            }
        }

        private EntityDefinition definition;
        public string EntityName { get; set; }
        public bool BlocksMovement { get; set; }

        private EntityType entityType;
        public EntityType EntityType
        {
            get => entityType;
            set
            {
                entityType = value;
                ZIndex = (int)entityType;
            }
        }

        public MapData MapData { get; set; }
        public FighterComponent FighterComponent { get; set; }
        public BaseAIComponent AIComponent { get; set; }
        public ConsumableComponent ConsumableComponent { get; set; }
        public EquippableComponent EquippableComponent { get; set; }
        public InventoryComponent InventoryComponent { get; set; }
        public LevelComponent LevelComponent { get; set; }
        public EquipmentComponent EquipmentComponent { get; set; }

        public Entity() { }


        public Entity(MapData mapData, Vector2I startPosition, string key = "")
        {
            Centered = false;
            GridPosition = startPosition;
            MapData = mapData;
            if (key != string.Empty)
                SetEntityType(key);
        }

        public void SetEntityType(string key)
        {
            this.key = key;
            var entityDefinition = ResourceLoader.Load<EntityDefinition>(entityTypes[key]);
            definition = entityDefinition;
            EntityType = entityDefinition.EntityType;
            BlocksMovement = definition.IsBlockingMovement;
            EntityName = entityDefinition.Name;
            Texture = entityDefinition.Texture;
            Modulate = entityDefinition.Color;

            switch (entityDefinition.AIType)
            {
                case AIType.HOSTILE:
                    AIComponent = new HostileEnemyAIComponent();
                    AddChild(AIComponent);
                    break;
            }

            if (entityDefinition.FighterComponentDefinition != null)
            {
                FighterComponent = new FighterComponent(entityDefinition.FighterComponentDefinition);
                AddChild(FighterComponent);
            }

            if (entityDefinition.ItemComponentDefinition != null)
            {
                if (entityDefinition.ItemComponentDefinition is ConsumableComponentDefinition ccd)
                {
                    HandleConsumable(ccd);
                }
                else if (entityDefinition.ItemComponentDefinition is EquippableComponentDefinition ecd)
                {
                    EquippableComponent = new EquippableComponent(ecd);
                    AddChild(EquippableComponent);
                }
            }

            if (entityDefinition.InventoryCapacity > 0)
            {
                InventoryComponent = new InventoryComponent(entityDefinition.InventoryCapacity);
                AddChild(InventoryComponent);
            }

            if (entityDefinition.LevelInfo != null)
            {
                LevelComponent = new LevelComponent(entityDefinition.LevelInfo);
                AddChild(LevelComponent);
            }

            if (entityDefinition.HasEquipment)
            {
                EquipmentComponent = new EquipmentComponent();
                AddChild(EquipmentComponent);
                EquipmentComponent.Entity = this;
            }

        }

        private void HandleConsumable(ConsumableComponentDefinition consumableDefinition)
        {
            ConsumableComponent = consumableDefinition switch
            {
                HealingConsumableComponentDefinition hccd => new HealingConsumableComponent(hccd),
                LightningDamageConsumableComponentDefinition ldccd => new LightningDamageConsumableComponent(ldccd),
                ConfusionConsumableComponentDefinition cccd => new ConfusionConsumableComponent(cccd),
                FireballDamageConsumableComponentDefinition fdccd => new FireballDamageConsumableComponent(fdccd),
                _ => null,
            };
            if (ConsumableComponent != null)
            {
                AddChild(ConsumableComponent);
                ConsumableComponent.Entity = this;
            }
        }

        public void Move(Vector2I offset)
        {
            MapData.UnregisterBlockingEntity(this);
            GridPosition += offset;
            MapData.RegisterBlockingEntity(this);
        }

        public bool IsAlive() =>
            AIComponent != null;

        public int Distance(Vector2I otherPosition)
        {
            var relative = otherPosition - GridPosition;
            return Mathf.Max(Mathf.Abs(relative.X), Mathf.Abs(relative.Y));
        }

        public Dictionary GetSaveData()
        {
            var saveData = new Dictionary() {
                { "x", GridPosition.X },
                { "y", GridPosition.Y },
                { "key", key }
            };
            if (FighterComponent != null)
                saveData["fighter_component"] = FighterComponent.GetSaveData();
            if (AIComponent != null)
                saveData["ai_component"] = AIComponent.GetSaveData();
            if (InventoryComponent != null)
                saveData["inventory_component"] = InventoryComponent.GetSaveData();
            if (EquipmentComponent != null)
                saveData["equipment_component"] = EquipmentComponent.GetSaveData();
            if (LevelComponent != null)
                saveData["level_component"] = LevelComponent.GetSaveData();
            return saveData;
        }

        public void Restore(Dictionary save_data)
        {
            GridPosition = new Vector2I(save_data["x"].AsInt32(), save_data["y"].AsInt32());
            SetEntityType(save_data["key"].AsString());
            if (FighterComponent != null && save_data.ContainsKey("fighter_component"))
                FighterComponent.Restore(save_data["fighter_component"].AsGodotDictionary());
            if (AIComponent != null && save_data.ContainsKey("ai_component"))
            {
                var ai_data = save_data["ai_component"].AsGodotDictionary();
                if (ai_data["type"].AsString() == "ConfusedEnemyAI")
                {
                    var confused_enemy_ai = new ConfusedEnemyAIComponent(ai_data["turns_remaining"].AsInt32());
                    AddChild(confused_enemy_ai);
                }
            }
            if (InventoryComponent != null && save_data.ContainsKey("inventory_component"))
                InventoryComponent.Restore(save_data["inventory_component"].AsGodotDictionary());
            if (EquipmentComponent != null && save_data.ContainsKey("equipment_component"))
                EquipmentComponent.Restore(save_data["equipment_component"].AsGodotDictionary());
            if (LevelComponent != null && save_data.ContainsKey("level_component"))
                LevelComponent.Restore(save_data["level_component"].AsGodotDictionary());
        }
    }
}
