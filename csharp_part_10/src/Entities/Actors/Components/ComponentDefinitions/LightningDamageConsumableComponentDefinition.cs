using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class LightningDamageConsumableComponentDefinition : ConsumableComponentDefinition
    {
        [Export]
        public int Damage { get; set; } = 0;

        [Export]
        public int MaximumRange { get; set; } = 0;
    }
}
