using Godot;
using System.Collections.Generic;

namespace SuperRogalik
{
    public partial class InventoryComponent : Component
    {
        public List<Entity> Items { get; set; }
        public int Capacity { get; set; }

        public InventoryComponent(int capacity)
        {
            Items = new List<Entity>();
            Capacity = capacity;
        }

        public void Drop(Entity item)
        {
            Items.Remove(item);
            var mapData = GetMapData();

            mapData.Entities.Add(item);
            mapData.EmitSignal(MapData.SignalName.EntityPlaced, item);

            item.MapData = mapData;
            item.GridPosition = Entity.GridPosition;
            MessageLog.SendMessage($"You dropped the {item.EntityName}", Colors.White);
        }
    }
}
