using Godot;
using Godot.Collections;

namespace SuperRogalik
{
    public partial class BaseAIComponent : Component
    {
        public virtual void Perform() { }

        public Vector2[] GetPointPathTo(Vector2I destination) =>
            GetMapData().Pathfinder.GetPointPath(Entity.GridPosition, destination);

        public virtual Dictionary GetSaveData()
        {
            return new Dictionary();
        }
    }
}
