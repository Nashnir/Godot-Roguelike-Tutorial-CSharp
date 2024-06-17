using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class EquippableComponentDefinition : ItemComponentDefinition
    {
        [Export]
        public EquipmentType EquipmentType { get; set; }

        [Export]
        public int PowerBonus { get; set; } = 0;

        [Export]
        public int DefenseBonus { get; set; } = 0;
    }
}
