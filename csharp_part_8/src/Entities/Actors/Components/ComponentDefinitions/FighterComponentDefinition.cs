using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class FighterComponentDefinition : Resource
    {
        [ExportCategory("Stats")]

        [Export]
        public int MaxHp { get; set; }

        [Export]
        public int Power { get; set; }

        [Export]
        public int Defense { get; set; }

        [ExportCategory("Visuals")]

        [Export]
        public AtlasTexture DeathTexture { get; set; } =
            ResourceLoader.Load<AtlasTexture>("res://assets/resources/default_death_texture.tres");

        [Export]
        public Color DeathColor { get; set; } = Colors.DarkRed;
    }
}
