using Godot;
using System.Collections.Generic;
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

        RandomNumberGenerator rng = new RandomNumberGenerator();


        static readonly int[][] maxItemsByFloor = [
            [1, 1],
            [4, 2]
        ];

        static readonly int[][] maxMonstersByFloor = [
            [1, 2],
            [4, 3],
            [6, 5]
        ];

        static readonly Dictionary<int, Dictionary<string, int>> item_chances = new(){
            { 0, new Dictionary<string, int>{ {"health_potion", 35}, }},
            { 2, new Dictionary<string, int>{ {"confusion_scroll", 10}, }},
            { 4, new Dictionary<string, int>{ {"lightning_scroll", 25}, }},
            { 6, new Dictionary<string, int>{ {"fireball_scroll", 25}, }},
        };

        static readonly Dictionary<int, Dictionary<string, int>> enemy_chances = new(){
            { 0, new Dictionary<string, int>{{"orc", 80}, }},
            { 3, new Dictionary<string, int>{{"troll", 15}, }},
            { 5, new Dictionary<string, int>{{"troll", 30}, }},
            { 7, new Dictionary<string, int>{{"troll", 60}, }},
        };


        public override void _Ready()
        {
            rng.Randomize();
        }

        public MapData GenerateDungeon(Entity player, int currentFloor)
        {
            var dungeon = new MapData(MapWidth, MapHeight, player);
            dungeon.CurrentFloor = currentFloor;
            dungeon.Entities.Add(player);

            var rooms = new List<Rect2I>();
            Vector2I centerLastRoom = Vector2I.Zero;

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
                centerLastRoom = newRoom.GetCenter();
                if (rooms.Count == 0)
                {
                    player.GridPosition = newRoom.GetCenter();
                    player.MapData = dungeon;
                }
                else
                {
                    TunnelBetween(dungeon, rooms.Last().GetCenter(), newRoom.GetCenter());
                }
                PlaceEntities(dungeon, newRoom, currentFloor);

                rooms.Add(newRoom);
            }
            dungeon.DownStairsLocation = centerLastRoom;
            var downTile = dungeon.GetTile(centerLastRoom);
            downTile.SetTileType(Tile.DOWN_STAIRS);
            dungeon.SetupPathfinding();

            return dungeon;
        }

        private void PlaceEntities(MapData dungeon, Rect2I room, int currentFloor)
        {
            var maxMonstersPerRoom = GetMaxValueForFloor(maxMonstersByFloor, currentFloor);
            var maxItemsPerRoom = GetMaxValueForFloor(maxItemsByFloor, currentFloor);
            var numberOfMonsters = rng.RandiRange(0, maxMonstersPerRoom);
            var numberOfItems = rng.RandiRange(0, maxItemsPerRoom);


            var monsters = GetEntitiesAtRandom(enemy_chances, numberOfMonsters, currentFloor);
            var items = GetEntitiesAtRandom(item_chances, numberOfItems, currentFloor);

            var entity_keys = monsters.Concat(items);
	
	        foreach(var entity_key in entity_keys)
            {
                var x = rng.RandiRange(room.Position.X + 1, room.End.X - 1);
                var y = rng.RandiRange(room.Position.Y + 1, room.End.Y - 1);
                var newEntityPosition = new Vector2I(x, y);

                var canPlace = true;
		        foreach(var entity in dungeon.Entities)
                {
			        if( entity.GridPosition == newEntityPosition)
                    {
                        canPlace = false;
                        break;
                    }
                }
		        if(canPlace)
                {
                    var newEntity = new Entity(dungeon, newEntityPosition, entity_key);
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

        static int GetMaxValueForFloor(int[][] weightedChancesByFloor, int currentFloor)
        {
            var current_value = 0;

            foreach (var chance in weightedChancesByFloor)
                if (chance[0] > currentFloor)
                    break;
                else
                    current_value = chance[1];

            return current_value;
        }

        public List<string> GetEntitiesAtRandom(
            Dictionary<int, Dictionary<string, int>> weightedChancesByFloor, int numberOfEntities, int currentFloor)
        {
            var entityWeightedChances = new Dictionary<string, int>();
            var chosenEntities = new List<string>();

            foreach (var key in weightedChancesByFloor.Keys)
            {
                if (key > currentFloor)
                {
                    break;
                }
                else {
                    foreach (var entityName in weightedChancesByFloor[key].Keys)
                        entityWeightedChances[entityName] = weightedChancesByFloor[key][entityName];
                }
            }

            for (int i = 0; i < numberOfEntities; i++)
                chosenEntities.Add(PickWeighted(entityWeightedChances));

            return chosenEntities;
        }

        public string PickWeighted(Dictionary<string, int> weighted_chances)
        {
            var keys = new List<string>();
            var cumulative_chances = new List<int>();
            var sum = 0;
	        foreach(var key in weighted_chances.Keys)
            {
                keys.Add(key);
                var chance = weighted_chances[key];
                sum += chance;
                cumulative_chances.Add(sum);
            }
            var random_chance = rng.RandiRange(0, sum - 1);
            string selection = string.Empty;
	
	        for(int i=0; i<cumulative_chances.Count; i++)
            {
		        if (cumulative_chances[i] > random_chance)
                {
                    selection = keys[i];
                    break;
                }
            }

            return selection;
        }
    }
}