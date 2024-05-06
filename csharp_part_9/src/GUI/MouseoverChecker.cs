using Godot;
using System.Linq;

namespace SuperRogalik
{
    public partial class MouseoverChecker : Node2D
    {
        [Signal]
        public delegate void EntitiesFocussedEventHandler(string entityList);

        private Vector2I mouseTile = new Vector2I(-1, -1);
        private Map map;

        public override void _Ready()
        {
            map = GetParent<Map>();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is not InputEventMouseMotion)
                return;

            var mousePosition = GetLocalMousePosition();
            var tilePosition = Grid.WorldToGrid((Vector2I)mousePosition);
            if (mouseTile != tilePosition)
            {
                mouseTile = tilePosition;
                var entityNames = GetNamesAtLocation(tilePosition);
                EmitSignal(SignalName.EntitiesFocussed, entityNames);
            }
        }

        private string GetNamesAtLocation(Vector2I gridPosition)
        {
            var mapData = map.MapData;
            var tile = mapData.GetTile(gridPosition);
            if (tile == null || !tile.IsInView)
                return string.Empty;

            var entitiesAtLocation = mapData.Entities
                .Where(entity => entity.GridPosition == gridPosition)
                .ToList();
            entitiesAtLocation.Sort((a, b) => a.ZIndex - b.ZIndex);
            var entityNames = string.Join(", ", entitiesAtLocation.Select(e => e.EntityName));

            return entityNames;
        }
    }
}