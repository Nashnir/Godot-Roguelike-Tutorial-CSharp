using Godot;

namespace SuperRogalik
{
    public partial class Action : RefCounted 
    {
        public Entity Entity { get; set; }

        public Action(Entity entity) 
        {
            Entity = entity;
        }

        public virtual void Perform() { }

        public MapData GetMapData() =>
            Entity.MapData;

    }
}
