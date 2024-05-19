using Godot;
using Godot.Collections;
using System.Linq;

namespace SuperRogalik
{
    public partial class DungeonGenerator : Node
    {
        [ExportCategory("Map Dimensions")]

        [Export]
        public int MapWidth { get; set; } = 80;

        [Export]
        public int MapHeight { get; set; } = 45;

        [ExportCategory("Rooms RNG")]

        [Export]
        public int MaxRooms { get; set; } = 30;

        [Export]
        public int RoomMaxSize { get; set; } = 10;

        [Export]
        public int RoomMinSize { get; set; } = 6;

        [ExportCategory("Monsters RNG")]

        [Export]
        public int MaxMonstersPerRoom { get; set; } = 2;

        [Export]
        public int MaxItemsPerRoom { get; set; } = 2;

        RandomNumberGenerator rng = new RandomNumberGenerator();

        

        public override void _Ready()
        {
            rng.Randomize();
        }

        public MapData GenerateDungeon(Entity player)
        {
            var dungeon = new MapData(MapWidth, MapHeight, player);
            dungeon.Entities.Add(player);
            var rooms = new Array<Rect2I>();

            for (int i = 0; i < MaxRooms; i++)
            {
                var roomWidth = rng.RandiRange(RoomMinSize, RoomMaxSize);
                var roomHeight = rng.RandiRange(RoomMinSize, RoomMaxSize);
                var x = rng.RandiRange(0, dungeon.Width - roomWidth - 1);
                var y = rng.RandiRange(0, dungeon.Height - roomHeight - 1);
                var newRoom = new Rect2I(x, y, roomWidth, roomHeight);

                var hasIntersections = rooms.Any(room => room.Intersects(newRoom.Grow(-1)));
                if (hasIntersections)
                    continue;

                CarveRoom(dungeon, newRoom);
                if (!rooms.Any())
                {
                    player.GridPosition = newRoom.GetCenter();
                    player.MapData = dungeon;
                }
                else
                {
                    TunnelBetween(dungeon, rooms.Last().GetCenter(), newRoom.GetCenter());
                }
                PlaceEntities(dungeon, newRoom);
                PlaceItems(dungeon, newRoom);

                rooms.Add(newRoom);
            }
            dungeon.SetupPathfinding();
            return dungeon;
        }

        private void PlaceEntities(MapData dungeon, Rect2I room)
        {
            var numberOfMonsters = rng.RandiRange(0, MaxMonstersPerRoom);
            for (int i = 0; i < numberOfMonsters; i++)
            {
                var newEntityPosition = GeneratePosition(dungeon, room);
                if (newEntityPosition.HasValue)
                {
                    Entity newEntity = rng.Randf() < 0.8
                        ? new Entity(dungeon, newEntityPosition.Value, Entity.ORC)
                        : new Entity(dungeon, newEntityPosition.Value, Entity.TROLL);
                    dungeon.Entities.Add(newEntity);
                }
            }
        }

        private void PlaceItems(MapData dungeon, Rect2I room)
        {
            var numberOfItems = rng.RandiRange(0, MaxItemsPerRoom);
            for (int i = 0; i < numberOfItems; i++)
            {
                Vector2I? newEntityPosition = GeneratePosition(dungeon, room);
                if (newEntityPosition != null && newEntityPosition.HasValue)
                {
                    float item_chance = rng.Randf();
                    Entity newEntity;
                    if (item_chance < 0.7)
                    {
                        newEntity = new Entity(dungeon, newEntityPosition.Value, Entity.HEALTH_POTION);
                    }
                    else if (item_chance < 0.8)
                    {
                        newEntity = new Entity(dungeon, newEntityPosition.Value, Entity.FIREBALL_SCROLL);
                    }
                    else if (item_chance < 0.9)
                    {
                        newEntity = new Entity(dungeon, newEntityPosition.Value, Entity.CONFUSION_SCROLL);
                    }
                    else
                    {
                        newEntity = new Entity(dungeon, newEntityPosition.Value, Entity.LIGHTING_SCROLL);
                    }
                    dungeon.Entities.Add(newEntity);
                }
            }
        }

        private Vector2I? GeneratePosition(MapData dungeon, Rect2I room)
        {
            var x = rng.RandiRange(room.Position.X + 1, room.End.X - 1);
            var y = rng.RandiRange(room.Position.Y + 1, room.End.Y - 1);
            var newEntityPosition = new Vector2I(x, y);
            bool canPlace = dungeon.Entities.All(e => e.GridPosition != newEntityPosition);
            return canPlace ? newEntityPosition : null;
        }

        private static void CarveTile(MapData dungeon, int x, int y)
        {
            var tilePosition = new Vector2I(x, y);
            var tile = dungeon.GetTile(tilePosition);
            tile.SetTileType(Tile.FLOOR);
        }

        private static void CarveRoom(MapData dungeon, Rect2I room)
        {
            var inner = room.Grow(-1);
            for (int y = inner.Position.Y; y <= inner.End.Y; y++)
            {
                for (int x = inner.Position.X; x <= inner.End.X; x++)
                {
                    CarveTile(dungeon, x, y);
                }
            }
        }

        private static void TunnelHorizontal(MapData dungeon, int y, int xStart, int xEnd)
        {
            var xMin = Mathf.Min(xStart, xEnd);
            var xMax = Mathf.Max(xStart, xEnd);
            for (int x = xMin; x <= xMax; x++)
            {
                CarveTile(dungeon, x, y);
            }
        }

        private static void TunnelVertical(MapData dungeon, int x, int yStart, int yEnd)
        {
            var yMin = Mathf.Min(yStart, yEnd);
            var yMax = Mathf.Max(yStart, yEnd);
            for (int y = yMin; y <= yMax; y++)
            {
                CarveTile(dungeon, x, y);
            }
        }

        private void TunnelBetween(MapData dungeon, Vector2I start, Vector2I end)
        {
            if (rng.Randf() < 0.5)
            {
                TunnelHorizontal(dungeon, start.Y, start.X, end.X);
                TunnelVertical(dungeon, end.X, start.Y, end.Y);
            }
            else
            {
                TunnelVertical(dungeon, start.X, start.Y, end.Y);
                TunnelHorizontal(dungeon, end.Y, start.X, end.X);
            }
        }
    }
}