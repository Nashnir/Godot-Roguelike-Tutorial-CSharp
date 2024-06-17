namespace SuperRogalik
{
    public partial class EquippableComponent : Component
    {
        public EquipmentType EquipmentType { get; set; }
        public int PowerBonus { get; set; }
        public int DefenseBonus { get; set; }

        public EquippableComponent(EquippableComponentDefinition definition)
        {
            EquipmentType = definition.EquipmentType;
            PowerBonus = definition.PowerBonus;
            DefenseBonus = definition.DefenseBonus;
        }
    }
}
