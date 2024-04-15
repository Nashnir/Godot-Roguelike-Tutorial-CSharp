using Godot;

namespace SuperRogalik
{
	public partial class Map : Node2D
	{
		[Export]
		public int MapWidth { get; set; } = 80;

		[Export]
		public int MapHeight { get; set; } = 45;

		public MapData MapData { get; set; }

		public override void _Ready()
		{
			MapData = new MapData(MapWidth, MapHeight);
			PlaceTiles();
		}

		private void PlaceTiles()
		{
			foreach (var tile in MapData.Tiles)
			{
				AddChild(tile);
			}
		}
	}
}
