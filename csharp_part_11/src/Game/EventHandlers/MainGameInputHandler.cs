using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperRogalik
{
    public partial class MainGameInputHandler : BaseInputHandler
    {
        public readonly Dictionary<string, Vector2I> directions = new()
        {
            {"move_up",  Vector2I.Up},
            {"move_down", Vector2I.Down},
            {"move_left", Vector2I.Left},
            {"move_right", Vector2I.Right},
            {"move_up_left", Vector2I.Up + Vector2I.Left},
            {"move_up_right", Vector2I.Up + Vector2I.Right},
            {"move_down_left", Vector2I.Down + Vector2I.Left},
            {"move_down_right", Vector2I.Down + Vector2I.Right},
        };

        private PackedScene inventoryMenuScene;

        [Export]
        public Reticle Reticle { get; set; }

        public override void _Ready()
        {
            inventoryMenuScene = ResourceLoader.Load<PackedScene>("res://src/GUI/InventoryMenu/inventory_menu.tscn");
        }

        public override async Task<Action> GetActionAsync(Entity player)
        {
            Action action = null;

            foreach (var direction in directions.Keys)
            {
                if (Input.IsActionJustPressed(direction))
                {
                    var offset = directions[direction];
                    action = new BumpAction(player, offset.X, offset.Y);
                }
            }
            if (Input.IsActionJustPressed("wait")) action = new WaitAction(player);

            if (Input.IsActionJustPressed("view_history"))
                GetParent<InputHandler>().TransitionTo(InputHandler.InputHandlers.HISTORY_VIEWER);

            if (Input.IsActionJustPressed("pickup"))
                action = new PickupAction(player);

            if (Input.IsActionJustPressed("drop"))
            {
                var item = await GetItemAsync("Select an item to drop", player.InventoryComponent);
                action = new DropItemAction(player, item);
            }

            if (Input.IsActionJustPressed("activate"))
                action = await ActivateItem(player);

            if (Input.IsActionJustPressed("look"))
                await GetGridPosition(player, 0);

            if (Input.IsActionJustPressed("descend"))
                action = new TakeStairsAction(player);

            if (Input.IsActionJustPressed("quit") || Input.IsActionJustPressed("ui_back"))
                action = new EscapeAction(player);

            return action;
        }

        private async Task<Action> ActivateItem(Entity player)
        {
            var selectedItem = await GetItemAsync("Select an item to use", player.InventoryComponent, true);

            if (selectedItem == null)
                return null;

            var targetRadius = -1;
            if (selectedItem.ConsumableComponent != null)
                targetRadius = selectedItem.ConsumableComponent.GetTargetingRadius();

            if (targetRadius == -1)
                return new ItemAction(player, selectedItem);

            var targetPosition = await GetGridPosition(player, targetRadius);
            if (targetPosition == new Vector2I(-1, -1))
                return null;

            return new ItemAction(player, selectedItem, targetPosition);
        }

        public async Task<Entity> GetItemAsync(string windowTitle, InventoryComponent inventory, bool EvaluateForNextStep = false)
        {
            if(inventory.Items.Count == 0)
            {
                await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
                MessageLog.SendMessage("No items in inventory.", GameColors.IMPOSSIBLE);
                return null;
            }
            var inventoryMenu = inventoryMenuScene.Instantiate<InventoryMenu>();
            AddChild(inventoryMenu);
            inventoryMenu.Build(windowTitle, inventory);
            GetParent<InputHandler>().TransitionTo(InputHandler.InputHandlers.DUMMY);
            var selectedItem = await ToSignal(inventoryMenu, InventoryMenu.SignalName.ItemSelected);

            ConsumableComponent cc = null;
            if (selectedItem.Length > 0)
            {
                cc = ((Entity)selectedItem[0]).ConsumableComponent; //selected at all?
            }

            if (!EvaluateForNextStep || (cc?.GetTargetingRadius() == -1)) //drop or activate without targeting
            {
                await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
                GetParent<InputHandler>().CallDeferred(nameof(InputHandler.TransitionTo),
                    (int)InputHandler.InputHandlers.MAIN_GAME);
            }

            return (Entity)selectedItem?.FirstOrDefault();
        }

        public async Task<Vector2I> GetGridPosition(Entity player, int radius)
        {
            GetParent<InputHandler>().TransitionTo(InputHandler.InputHandlers.DUMMY);
            var selected_position = await Reticle.SelectPositionAsync(player, radius);
            await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
            GetParent<InputHandler>().CallDeferred(nameof(InputHandler.TransitionTo),
                (int)InputHandler.InputHandlers.MAIN_GAME);
            return selected_position;
        }
    }
}
