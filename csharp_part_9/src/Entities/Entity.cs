using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class Entity : Sprite2D
    {
        public enum AIType { NONE, HOSTILE };

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


        public Entity(MapData mapData, Vector2I startPosition, EntityDefinition entityDefinition)
        {
            Centered = false;
            GridPosition = startPosition;
            this.MapData = mapData;
            SetEntityType(entityDefinition);
        }

        public void SetEntityType(EntityDefinition entityDefinition)
        {
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
            if (entityDefinition.ConsumableComponentDefinition is HealingConsumableComponentDefinition hccd)
            {
                ConsumableComponent = new HealingConsumableComponent(hccd);
            }
            else if (entityDefinition.ConsumableComponentDefinition is LightningDamageConsumableComponentDefinition ldccd)
            {
                ConsumableComponent = new LightningDamageConsumableComponent(ldccd);
            }
            else if (entityDefinition.ConsumableComponentDefinition is ConfusionConsumableComponentDefinition cccd)
            {
                ConsumableComponent = new ConfusionConsumableComponent(cccd);
            }
            else if (entityDefinition.ConsumableComponentDefinition is FireballDamageConsumableComponentDefinition fdccd)
            {
                ConsumableComponent = new FireballDamageConsumableComponent(fdccd);
            }

            if (ConsumableComponent != null)
                AddChild(ConsumableComponent);
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
    }
}
