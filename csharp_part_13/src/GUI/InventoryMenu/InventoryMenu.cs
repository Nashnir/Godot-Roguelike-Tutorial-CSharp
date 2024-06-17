using Godot;

namespace SuperRogalik
{
    public partial class InventoryMenu : CanvasLayer
    {
        [Signal]
        public delegate void ItemSelectedEventHandler(Entity item);

        PackedScene inventoryMenuItemScene = ResourceLoader.Load<PackedScene>(
            "res://src/GUI/InventoryMenu/inventory_menu_item.tscn");

        VBoxContainer inventoryList;
        Label titleLabel;

        public override void _Ready()
        {
            inventoryList = GetNode<VBoxContainer>("%InventoryList");
            titleLabel = GetNode<Label>("%TitleLabel");
            Hide();
        }

        public void ButtonPressed(Entity item)
        {
            EmitSignal(SignalName.ItemSelected, item);
            QueueFree();
        }

        private void RegisterItem(int index, Entity item, bool isEqquiped)
        {
            var itemButton = inventoryMenuItemScene.Instantiate<Button>();
            char ch = (char)('a' + index);
            itemButton.Text = $"( {ch} ) {item.EntityName} {(isEqquiped ? " (E)" : string.Empty)}";
            var shortcutEvent = new InputEventKey();
            shortcutEvent.Keycode = Key.A + index;
            itemButton.Shortcut = new Shortcut();
            itemButton.Shortcut.Events = new Godot.Collections.Array { shortcutEvent };
            itemButton.Pressed += () => ButtonPressed(item);
            inventoryList.AddChild(itemButton);
        }

        public void Build(string titleText, InventoryComponent inventory)
        {
            if (inventory.Items == null || inventory.Items.Count == 0)
            {
                CallDeferred(nameof(ButtonPressed), (Entity)null);
                MessageLog.SendMessage("No items in inventory.", GameColors.IMPOSSIBLE);
                return;
            }
            titleLabel.Text = titleText;
            var equipment = inventory.Entity.EquipmentComponent;
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                var item = inventory.Items[i];
                var isEquipped = equipment.IsItemEquipped(item);
                RegisterItem(i, item, isEquipped);
            }

            inventoryList.GetChild<Button>(0).GrabFocus();
            Show();
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Input.IsActionJustPressed("ui_back"))
            {
                EmitSignal(SignalName.ItemSelected, null);
                QueueFree();
            }
        }
    }
}