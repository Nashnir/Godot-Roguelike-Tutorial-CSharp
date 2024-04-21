using Godot;

namespace SuperRogalik
{
    public partial class Component : Node2D
    {
        public Entity Entity { get; set; }

        public override void _Ready()
        {
            Entity = GetParent() as Entity;
        }

        public MapData GetMapData() =>
            Entity.MapData;
    }
}
