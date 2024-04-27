using Godot;

namespace SuperRogalik
{
    public partial class ItemAction : Action
    {
        public Entity Item { get; set; }
        public Vector2I TargetPosition { get; set; }

        public ItemAction(Entity entity, Entity item, Vector2I? targetPosition = null) : base(entity)
        {
            Item = item;
            if (targetPosition == null)
                targetPosition = entity.GridPosition;

            TargetPosition = targetPosition.Value;
        }

        public Entity GetTargetActor() =>
            GetMapData().GetActorAtLocation(TargetPosition);

        public override bool Perform()
        {
            if (Item == null)
                return false;

            return Item.ConsumableComponent.Activate(this);
        }
    }
}
