using Godot;

namespace SuperRogalik
{
    public partial class PickupAction : Action
    {
        public PickupAction(Entity entity) : base(entity)
        {
        }

        public override bool Perform()
        {
            var inventory = Entity.InventoryComponent;
            var mapData = GetMapData();

            foreach (var item in mapData.GetItems())
            {
                if (Entity.GridPosition == item.GridPosition)
                {
                    if (inventory.Items.Count >= inventory.Capacity)
                    {
                        MessageLog.SendMessage("Your inventory is full.", GameColors.IMPOSSIBLE);
                        return false;
                    }

                    mapData.Entities.Remove(item);
                    item.GetParent().RemoveChild(item);
                    inventory.Items.Add(item);
                    MessageLog.SendMessage($"You picked up the {item.EntityName}!", Colors.White);
                    return true;
                }
            }
            MessageLog.SendMessage("There is nothing here to pick up.", GameColors.IMPOSSIBLE);
            return false;
        }
    }
}
