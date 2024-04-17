using Godot;

namespace SuperRogalik
{
    public partial class Map : Node2D
	{
		[Export] 
		int FovRadius { get; set; } = 8;

        public MapData MapData { get; set; }
        public FieldOfView FieldOfView { get; set; }
     
		private DungeonGenerator dungeonGenerator;

        Node2D entities;
        Node2D tiles;

        public override void _Ready()
		{
			dungeonGenerator = GetNode<DungeonGenerator>("DungeonGenerator");
			FieldOfView = GetNode<FieldOfView>("FieldOfView");
            tiles = GetNode<Node2D>("Tiles");
            entities = GetNode<Node2D>("Entities");
        }

        public void Generate(Entity player)
		{
            MapData = dungeonGenerator.GenerateDungeon(player);
            PlaceTiles();
			PlaceEntities();
        }

        private void PlaceEntities()
        {
            foreach (var entity in MapData.Entities)
            {
                entities.AddChild(entity);
            }
        }

        private void PlaceTiles()
		{
			foreach (var tile in MapData.Tiles)
			{
				tiles.AddChild(tile);
            }
        }

        public void UpdateFov(Vector2I playerPosition)
        {
            FieldOfView.UpdateFov(MapData, playerPosition, FovRadius);
            foreach (var entity in MapData.Entities)
            {
                entity.Visible = MapData.GetTile(entity.GridPosition).IsInView;
            }
        }
    }
}
