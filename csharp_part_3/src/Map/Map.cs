using Godot;

namespace SuperRogalik
{
	public partial class Map : Node2D
	{
        private DungeonGenerator dungeonGenerator;

        public MapData MapData { get; set; }

		public override void _Ready()
		{
			dungeonGenerator = GetNode<DungeonGenerator>("DungeonGenerator");
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
	}
}
