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
	
            if (entityDefinition.FighterDefinition != null)
            {
                FighterComponent = new FighterComponent(entityDefinition.FighterDefinition);
                AddChild(FighterComponent);
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

    }
}
