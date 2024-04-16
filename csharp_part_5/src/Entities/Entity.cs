using Godot;

namespace SuperRogalik
{
    public partial class Entity : Sprite2D
    {
        private EntityDefinition definition;

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

        public Entity() { }

        public Entity(Vector2I startPosition, EntityDefinition entityDefinition)
        {
            Centered = false;
            GridPosition = startPosition;
            SetEntityType(entityDefinition);
        }

        public void Move(Vector2I offset) =>
            GridPosition += offset;

        public void SetEntityType(EntityDefinition entityDefinition)
        {
            definition = entityDefinition;
            Texture = entityDefinition.Texture;
            Modulate = entityDefinition.Color;
        }

        public bool IsBlockingMovement() =>
            definition.IsBlockingMovement;

        public string GetEntityName() =>
            definition.Name;

    }
}
