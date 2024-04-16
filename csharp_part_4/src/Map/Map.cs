using Godot;
using System;

namespace SuperRogalik
{
	public partial class Map : Node2D
	{
		[Export] 
		int FovRadius { get; set; } = 8;

        public MapData MapData { get; set; }
        public FieldOfView FieldOfView { get; set; }
     
		private DungeonGenerator dungeonGenerator;

        public override void _Ready()
		{
			dungeonGenerator = GetNode<DungeonGenerator>("DungeonGenerator");
			FieldOfView = GetNode<FieldOfView>("FieldOfView");
        }

		public void Generate(Entity player)
		{
            MapData = dungeonGenerator.GenerateDungeon(player);
            PlaceTiles();
        }

		private void PlaceTiles()
		{
			foreach (var tile in MapData.Tiles)
			{
				AddChild(tile);
            }
        }

		public void UpdateFov(Vector2I playerPosition) =>
			FieldOfView.UpdateFov(MapData, playerPosition, FovRadius);
    }
}
