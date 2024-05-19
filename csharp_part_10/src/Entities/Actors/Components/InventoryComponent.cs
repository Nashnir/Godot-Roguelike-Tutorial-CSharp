using Godot;
using Godot.Collections;
using System;
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

        public Dictionary GetSaveData()
        {
            var items = new Godot.Collections.Array();
            foreach (var item in Items)
            {
                items.Add(item.GetSaveData());
            }
            var saveData = new Dictionary(){
                { "capacity", Capacity },
                { "items",  items }
            };
            return saveData;
        }

        public void Restore(Dictionary saveData)
        {
            Capacity = saveData["capacity"].AsInt32();
            foreach (var itemData in saveData["items"].AsGodotArray())
            {
                var item = new Entity(null, new Vector2I(-1, -1), "");
                item.Restore(itemData.AsGodotDictionary());
                Items.Add(item);

            }
        }
    }
}
