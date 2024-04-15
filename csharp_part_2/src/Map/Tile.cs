using Godot;

namespace SuperRogalik
{
	public partial class Tile : Sprite2D
	{
		private TileDefinition definition;

		public Tile() { }
		public Tile(Vector2I gridPosition, TileDefinition tileDefinition)
		{
			Centered = false;
			Position = Grid.GridToWorld(gridPosition);
			SetTileType(tileDefinition);
		}

		public void SetTileType(TileDefinition tileDefinition)
		{
			definition = tileDefinition;
			Texture = definition.Texture;
			Modulate = definition.ColorDark;
		}

		public bool IsWalkable() =>
			definition.IsWalkable;

		public bool IsTransparent() =>
			definition.IsTransparent;
	}
}