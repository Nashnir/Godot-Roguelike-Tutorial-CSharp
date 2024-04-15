using Godot;
using Godot.Collections;

namespace SuperRogalik
{
    public partial class MapData : RefCounted
    {
        private const string FLOOR = "floor";
        private const string WALL = "wall";
        private readonly Dictionary<string, TileDefinition> tileTypes = new Dictionary<string, TileDefinition> {
            { FLOOR, ResourceLoader.Load<TileDefinition>("res://assets/definitions/tiles/tile_definition_floor.tres") },
            { WALL, ResourceLoader.Load<TileDefinition>("res://assets/definitions/tiles/tile_definition_wall.tres") },
        };

        public int Width { get; set; }
        public int Height { get; set; }
        public Array<Tile> Tiles { get; set; }

        public MapData(int mapWidth, int mapHeight)
        {
            Width = mapWidth;
            Height = mapHeight;
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
                    var tile = new Tile(tilePosition, tileTypes[FLOOR]);
                    Tiles.Add(tile);
                }
            }
            for (int x = 30; x <= 34; x++)
            {
                Tile tile = GetTile(new Vector2I(x, 22));
                tile.SetTileType(tileTypes[WALL]);
            }
        }

        public Tile GetTile(Vector2I gridPosition)
        {
            var tileIndex = GridToIndex(gridPosition);
            if (tileIndex == -1) return null;
            return Tiles[tileIndex];
        }

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
    }
}
