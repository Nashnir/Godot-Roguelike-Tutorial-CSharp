using Godot;
using Godot.Collections; //Dictionary<Variant>

namespace SuperRogalik
{
    public partial class Tile : Sprite2D
    {
        public const string FLOOR = "floor";
        public const string WALL = "wall";
        public static readonly Dictionary<string, TileDefinition> tileTypes = new() {
            { FLOOR, ResourceLoader.Load<TileDefinition>("res://assets/definitions/tiles/tile_definition_floor.tres") },
            { WALL, ResourceLoader.Load<TileDefinition>("res://assets/definitions/tiles/tile_definition_wall.tres") },
        };

        public string Key { get; set; }

        private TileDefinition definition;

        private bool isExplored = false;
        public bool IsExplored
        {
            get => isExplored;
            set
            {
                isExplored = value;
                if (isExplored && !Visible)
                {
                    Visible = true;
                }
            }
        }

        private bool isInView = false;
        public bool IsInView
        {
            get => isInView;
            set
            {
                isInView = value;
                Modulate = isInView ? definition.ColorLit : definition.ColorDark;
                if (isInView && !IsExplored)
                {
                    IsExplored = true;
                }
            }
        }

        public Tile() { }
        public Tile(Vector2I gridPosition, string key)
        {
            Visible = false;
            Centered = false;
            Position = Grid.GridToWorld(gridPosition);
            SetTileType(key);
        }

        public void SetTileType(string key)
        {
            Key = key;
            definition = tileTypes[key];
            Texture = definition.Texture;
            Modulate = definition.ColorDark;
        }

        public bool IsWalkable() =>
            definition.IsWalkable;

        public bool IsTransparent() =>
        definition.IsTransparent;

        public Dictionary GetSaveData()
        {
            return new Dictionary(){
               { "key", Key },
               { "is_explored", IsExplored }
            };
        }

        public void Restore(Dictionary saveData)
        {
            SetTileType(saveData["key"].AsString());
            IsExplored = saveData["is_explored"].AsBool();
        }
    }
}