using Godot;

namespace SuperRogalik
{
    public partial class DropItemAction : ItemAction
    {
        public DropItemAction(Entity entity, Entity item, Vector2I? targetPosition = null)
            : base(entity, item, targetPosition)
        {
        }

        public override bool Perform()
        {
            if (Item == null)
                return false;

            Entity.InventoryComponent.Drop(Item);
            return true;
        }
    }
}
