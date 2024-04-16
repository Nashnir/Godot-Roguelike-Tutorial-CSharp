using Godot;

namespace SuperRogalik
{
    public static class Grid
    {
        public static readonly Vector2I tileSize = new Vector2I(16, 16);

        public static Vector2I GridToWorld(Vector2I gridPos) =>
            gridPos * tileSize;

        public static Vector2I WorldToGrid(Vector2I gridPos) =>
            gridPos / tileSize;
    }
}
