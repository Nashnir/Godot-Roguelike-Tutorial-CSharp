using Godot;

namespace SuperRogalik
{
    public partial class Map : Node2D
    {
        [Export]
        int FovRadius { get; set; } = 8;

        [Signal]
        public delegate void DungeonFloorChangedEventHandler(int floor);

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

            SignalBus.Instance.PlayerDescended += NextFloor;
        }

        private void NextFloor()
        {
            var player = MapData.Player;
            entities.RemoveChild(player);
            foreach (var entity in entities.GetChildren()) {
                entity.QueueFree();
            }
	        foreach( var tile in tiles.GetChildren())
            {
                tile.QueueFree();
            }
            Generate(player, MapData.CurrentFloor + 1);
            player.GetNode<Camera2D>("Camera2D").MakeCurrent();
            FieldOfView.ResetFov();
            UpdateFov(player.GridPosition);
        }

        public void Generate(Entity player, int floor = 1)
        {
            MapData = dungeonGenerator.GenerateDungeon(player, floor);
            MapData.EntityPlaced += OnEntityPlaced;
            PlaceTiles();
            PlaceEntities();
            EmitSignal(SignalName.DungeonFloorChanged, floor);
        }

        private void OnEntityPlaced(Entity e) => 
            entities.AddChild(e);

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

        public bool LoadGame(Entity player)
        {
            MapData = new MapData(0, 0, player);
            MapData.EntityPlaced += OnEntityPlaced;
            if (!MapData.LoadGame())
                return false;

            PlaceTiles();
            PlaceEntities();
            EmitSignal(SignalName.DungeonFloorChanged, MapData.CurrentFloor);
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            MapData.EntityPlaced -= OnEntityPlaced;
            SignalBus.Instance.PlayerDescended -= NextFloor;
            base.Dispose(disposing);
        }
    }
}
