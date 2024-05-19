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
        public static readonly Dictionary<string, string> entityTypes = new(){
            { PLAYER, "res://assets/definitions/entities/actors/entity_definition_player.tres" },
            { ORC, "res://assets/definitions/entities/actors/entity_definition_orc.tres" },
            { TROLL, "res://assets/definitions/entities/actors/entity_definition_troll.tres" },
            { HEALTH_POTION, "res://assets/definitions/entities/items/health_potion_definition.tres" },
            { LIGHTING_SCROLL, "res://assets/definitions/entities/items/lightning_scroll_definition.tres" },
            { CONFUSION_SCROLL, "res://assets/definitions/entities/items/confusion_scroll_definition.tres" },
            { FIREBALL_SCROLL, "res://assets/definitions/entities/items/fireball_scroll_definition.tres" },
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
        public InventoryComponent InventoryComponent { get; set; }

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

            if (entityDefinition.ConsumableComponentDefinition != null)
                HandleConsumable(entityDefinition);

            if (entityDefinition.InventoryCapacity > 0)
            {
                InventoryComponent = new InventoryComponent(entityDefinition.InventoryCapacity);
                AddChild(InventoryComponent);
            }
        }

        private void HandleConsumable(EntityDefinition entityDefinition)
        {
            switch (entityDefinition.ConsumableComponentDefinition)
            {
                case HealingConsumableComponentDefinition hccd:
                    ConsumableComponent = new HealingConsumableComponent(hccd);
                    break;
                case LightningDamageConsumableComponentDefinition ldccd:
                    ConsumableComponent = new LightningDamageConsumableComponent(ldccd);
                    break;
                case ConfusionConsumableComponentDefinition cccd:
                    ConsumableComponent = new ConfusionConsumableComponent(cccd);
                    break;
                case FireballDamageConsumableComponentDefinition fdccd:
                    ConsumableComponent = new FireballDamageConsumableComponent(fdccd);
                    break;
            }

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
            return saveData;
        }

        public void Restore(Dictionary save_data) {
            GridPosition = new Vector2I(save_data["x"].AsInt32(), save_data["y"].AsInt32());
            SetEntityType(save_data["key"].AsString());
            if (FighterComponent != null && save_data.ContainsKey("fighter_component"))
                FighterComponent.Restore(save_data["fighter_component"].AsGodotDictionary());
	        if (AIComponent != null && save_data.ContainsKey("ai_component")) {
                var ai_data = save_data["ai_component"].AsGodotDictionary();
		        if (ai_data["type"].AsString() == "ConfusedEnemyAI") {
                    var confused_enemy_ai = new ConfusedEnemyAIComponent(ai_data["turns_remaining"].AsInt32());
                    AddChild(confused_enemy_ai);
                }
            }
            if (InventoryComponent != null && save_data.ContainsKey("inventory_component"))
                InventoryComponent.Restore(save_data["inventory_component"].AsGodotDictionary());
        }
    }
}
