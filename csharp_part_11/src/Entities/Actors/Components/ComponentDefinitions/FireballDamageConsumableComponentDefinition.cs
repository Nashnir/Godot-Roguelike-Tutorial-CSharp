using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class FireballDamageConsumableComponentDefinition : ConsumableComponentDefinition
    {
        [Export]
        public int Damage { get; set; }

        [Export]
        public int Radius { get; set; }
    }
}
