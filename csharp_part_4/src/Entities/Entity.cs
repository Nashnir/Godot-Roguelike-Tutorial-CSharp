using Godot;

namespace SuperRogalik
{
    public partial class Entity : Sprite2D
    {
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
            Texture = entityDefinition.Texture;
            Modulate = entityDefinition.Color;
        }

        public void Move(Vector2I offset) =>
            GridPosition += offset;
    }
}
