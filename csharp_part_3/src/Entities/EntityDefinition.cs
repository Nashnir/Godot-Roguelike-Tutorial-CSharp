using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class EntityDefinition : Resource
    {
        [ExportCategory("Visuals")]

        [Export]
        public AtlasTexture Texture { get; set; }

        [Export(PropertyHint.ColorNoAlpha)]
        public Color Color { get; set; } = Colors.White;
    }
}
