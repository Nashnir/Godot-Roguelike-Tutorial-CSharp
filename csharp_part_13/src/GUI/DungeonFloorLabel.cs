using Godot;

namespace SuperRogalik
{
    public partial class DungeonFloorLabel : Label
    {
        public void SetDungeonFloor(int currentFloor) =>
            Text = $"Dungeon Level: {currentFloor}";
    }
}
