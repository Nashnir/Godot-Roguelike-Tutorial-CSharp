using Godot;

namespace SuperRogalik
{
    public partial class ActionWithDirection : Action
    {
        public Vector2I Offset { get; set; }

        public ActionWithDirection(Entity entity, int dx, int dy) : base(entity)
        {
            Offset = new Vector2I(dx, dy);
        }

        public Vector2I GetDestination() =>
            Entity.GridPosition + Offset;

        public Entity GetBlockingEntityAtDestination() =>
            GetMapData().GetBlockingEntityAtLocation(GetDestination());

        public Entity GetTargetActor() =>
            GetMapData().GetActorAtLocation(GetDestination());
    }
}
