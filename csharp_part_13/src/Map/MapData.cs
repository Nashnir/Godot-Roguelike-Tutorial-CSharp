using Godot;
using Godot.Collections;
using System.Linq;

namespace SuperRogalik
{
    public partial class MapData : RefCounted
    {
        [Signal]
        public delegate void EntityPlacedEventHandler(Entity item);

        public int Width { get; set; }
        public int Height { get; set; }
        public Array<Tile> Tiles { get; set; }
        public Array<Entity> Entities { get; set; }

        const float entityPathfindingWeight = 10.0f;

        public Entity Player { get; set; }

        public AStarGrid2D Pathfinder { get; set; }

        public Vector2I DownStairsLocation { get; set; }
        public int CurrentFloor { get; set; } = 0;


        public MapData(int mapWidth, int mapHeight, Entity player)
        {
            Width = mapWidth;
            Height = mapHeight;
            Entities = new();
            Player = player;
            SetupTiles();
        }

        private void SetupTiles()
        {
            Tiles = new Array<Tile>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var tilePosition = new Vector2I(x, y);
                    var tile = new Tile(tilePosition, Tile.WALL);
                    Tiles.Add(tile);
                }
            }
        }

        public Tile GetTile(Vector2I gridPosition)
        {
            var tileIndex = GridToIndex(gridPosition);
            if (tileIndex == -1) return null;
            return Tiles[tileIndex];
        }

        public Tile GetTile(int x, int y) =>
            GetTile(new Vector2I(x, y));

        private int GridToIndex(Vector2I gridPosition)
        {
            if (!IsInBounds(gridPosition)) return -1;
            return gridPosition.Y * Width + gridPosition.X;
        }

        private bool IsInBounds(Vector2I coordinate)
        {
            return coordinate.X >= 0
                && coordinate.X < Width
                && coordinate.Y >= 0
                && coordinate.Y < Height;
        }

        public Entity GetBlockingEntityAtLocation(Vector2I gridPosition)
        {
            foreach (var entity in Entities)
            {
                if (entity.BlocksMovement && entity.GridPosition == gridPosition)
                    return entity;
            }
            return null;
        }

        public void RegisterBlockingEntity(Entity entity) =>
            Pathfinder.SetPointWeightScale(entity.GridPosition, entityPathfindingWeight);

        public void UnregisterBlockingEntity(Entity entity) =>
            Pathfinder.SetPointWeightScale(entity.GridPosition, 0);

        public void SetupPathfinding()
        {
            Pathfinder = new AStarGrid2D();
            Pathfinder.Region = new Rect2I(0, 0, Width, Height);
            Pathfinder.Update();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Height; x++)
                {
                    var gridPosition = new Vector2I(x, y);
                    var tile = GetTile(gridPosition);
                    Pathfinder.SetPointSolid(gridPosition, !tile.IsWalkable());
                }
            }
            foreach (var entity in Entities)
            {
                if (entity.BlocksMovement)
                {
                    RegisterBlockingEntity(entity);
                }
            }
        }

        public System.Collections.Generic.IEnumerable<Entity> GetActors() =>
            Entities.Where(e => e.EntityType == EntityType.ACTOR && e.IsAlive());

        public Entity GetActorAtLocation(Vector2I location) =>
            GetActors().FirstOrDefault(a => a.GridPosition == location);

        public System.Collections.Generic.IEnumerable<Entity> GetItems() =>
            Entities.Where(e => e.ConsumableComponent != null || e.EquippableComponent != null);

        public Dictionary GetSaveData() {

            var stairs = new Dictionary() {
                { "x", DownStairsLocation.X },
                { "y", DownStairsLocation.Y },
            };

            var save_data = new Dictionary() {
                { "width", Width },
                { "height", Height },
                { "player", Player.GetSaveData() },
                { "current_floor", CurrentFloor },
                { "down_stairs_location", stairs },
                { "entities", new Array() },
                { "tiles", new Array() },
            };

            foreach (var entity in Entities) {
                if (entity == Player)
                    continue;
                save_data["entities"].AsGodotArray().Add(entity.GetSaveData());
            }
            foreach (var tile in Tiles)
                save_data["tiles"].AsGodotArray().Add(tile.GetSaveData());
            
            return save_data;
        }

        public void Restore(Dictionary save_data)
        {
            Width = save_data["width"].AsInt32();
            Height = save_data["height"].AsInt32();

            DownStairsLocation = new Vector2I(
                save_data["down_stairs_location"].AsGodotDictionary()["x"].AsInt32(),
                save_data["down_stairs_location"].AsGodotDictionary()["y"].AsInt32());
            CurrentFloor = save_data["current_floor"].AsInt32();

            SetupTiles();
            for (int i = 0; i < Tiles.Count; i++)
                Tiles[i].Restore(save_data["tiles"].AsGodotArray()[i].AsGodotDictionary());

            SetupPathfinding();

            Player.Restore(save_data["player"].AsGodotDictionary());
            Player.MapData = this;
            Entities = new Array<Entity> { Player };
	        foreach( var entity_data in save_data["entities"].AsGodotArray())
            {
                var new_entity = new Entity(this, Vector2I.Zero, "");
                new_entity.Restore(entity_data.AsGodotDictionary());
                Entities.Add(new_entity);
            }
        }

        public void Save()
        {
            var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Write);

            var save_data = GetSaveData();

            var save_string = Json.Stringify(save_data);
            var save_hash = save_string.Sha256Text();
            file.StoreLine(save_hash);
            file.StoreLine(save_string);
            file.Close();
        }

        public bool LoadGame()
        {
            var file = FileAccess.Open("user://save_game.dat", FileAccess.ModeFlags.Read);

            var retrieved_hash = file.GetLine();
            var save_string = file.GetLine();
            file.Close();

            var calculated_hash = save_string.Sha256Text();
            var valid_hash = retrieved_hash == calculated_hash;
	        if (!valid_hash)
		        return false;

            var save_data = Json.ParseString(save_string);
            Restore(save_data.AsGodotDictionary());
            return true;
        }
    }
}
