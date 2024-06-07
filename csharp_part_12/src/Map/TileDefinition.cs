using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class TileDefinition : Resource
    {
        [ExportCategory("Visuals")]

        [Export]
        public AtlasTexture Texture { set; get; }

        [Export(PropertyHint.ColorNoAlpha)]
        public Color ColorLit { get; set; } = Colors.White;

        [Export(PropertyHint.ColorNoAlpha)]
        public Color ColorDark { get; set; } = Colors.White;


        [ExportCategory("Mechanics")]

        [Export]
        public bool IsWalkable { get; set; } = true;

        [Export]
        public bool IsTransparent { get; set; } = true;
    }
}
