using Godot;
using System.Collections.Generic;
using System.Linq;

namespace SuperRogalik
{
    public partial class EquipmentComponent : Component
    {
        [Signal]
        public delegate void EquipmentChangedEventHandler();

        public Dictionary<EquipmentType, Entity> slots = [];

        public int GetDefenseBonus() =>
            slots.Values.Sum(i => i.EquippableComponent?.DefenseBonus ?? 0);

        public int GetPowerBonus() =>
            slots.Values.Sum(i => i.EquippableComponent?.PowerBonus ?? 0);

        public bool IsItemEquipped(Entity item) =>
            slots.ContainsValue(item);

        public void UnequipFromSlot(EquipmentType slot, bool addMessage)
        {
            var currentItem = slots[slot];

            if (addMessage)
                MessageLog.SendMessage($"You remove the {currentItem.EntityName}.", Colors.White);

            slots.Remove(slot);
            EmitSignal(SignalName.EquipmentChanged);
        }

        public void EquipToSlot(EquipmentType slot, Entity item, bool addMessage)
        {
            if (slots.ContainsKey(slot))
                UnequipFromSlot(slot, addMessage);

            slots[slot] = item;
            if (addMessage)
                MessageLog.SendMessage($"You equip the {item.EntityName}.", Colors.White);
            EmitSignal(SignalName.EquipmentChanged);
        }

        public void ToggleEquip(Entity equippableItem, bool addMessage = true)
        {
            if (equippableItem.EquippableComponent == null)
                return;

            var slot = equippableItem.EquippableComponent.EquipmentType;
            if (slots.GetValueOrDefault(slot) == equippableItem)
            {
                UnequipFromSlot(slot, addMessage);
            }
            else
            {
                EquipToSlot(slot, equippableItem, addMessage);
            }
        }

        public Godot.Collections.Dictionary GetSaveData()
        {
            var equippedIndices = new Godot.Collections.Array<int>();
            var inventory = Entity.InventoryComponent;
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var item = inventory.Items[i];
                if (IsItemEquipped(item))
                    equippedIndices.Add(i);
            }
            return new Godot.Collections.Dictionary { { "equipped_indices", equippedIndices }, };
        }

        public void Restore(Godot.Collections.Dictionary saveData)
        {
            var equipped_indices = saveData["equipped_indices"].AsGodotArray().Select(f => (int)f);
            var inventory = Entity.InventoryComponent;
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                if (equipped_indices.Contains(i))
                {
                    var item = inventory.Items[i];
                    ToggleEquip(item, false);
                }
            }
        }
    }
}
