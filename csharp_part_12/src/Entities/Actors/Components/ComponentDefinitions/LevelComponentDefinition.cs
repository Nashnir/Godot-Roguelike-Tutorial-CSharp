using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class LevelComponentDefinition : Resource
    {
        [Export]
        public int LevelUpBase { get; set; } = 0;

        [Export]
        public int LevelUpFactor { get; set; } = 150;

        [Export]
        public int XpGiven { get; set; } = 0;
    }
}
