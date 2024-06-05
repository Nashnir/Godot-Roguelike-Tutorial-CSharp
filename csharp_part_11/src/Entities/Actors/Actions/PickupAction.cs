using Godot;
using System.Linq;

namespace SuperRogalik
{
    public partial class PickupAction : Action
    {
        public PickupAction(Entity entity) : base(entity) { }

        public override bool Perform()
        {
            var inventory = Entity.InventoryComponent;
            var mapData = GetMapData();
            var item = mapData.GetItems().FirstOrDefault(i => i.GridPosition == Entity.GridPosition);

            if (item == null)
            {
                MessageLog.SendMessage("There is nothing here to pick up.", GameColors.IMPOSSIBLE);
                return false;
            }

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
}
