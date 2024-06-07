using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class ConfusionConsumableComponentDefinition : ConsumableComponentDefinition
    {
        [Export]
        public int NumberOfTurns { get; set; }
    }
}
