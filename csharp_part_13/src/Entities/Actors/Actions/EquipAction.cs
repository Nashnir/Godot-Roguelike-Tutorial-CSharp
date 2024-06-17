using Godot;

namespace SuperRogalik
{
    public partial class EquipAction : Action
    {
        public Entity Item { get; }

        public EquipAction(Entity entity, Entity item) : base(entity)
        {
            Item = item;
        }

        public override bool Perform()
        {
            Entity.EquipmentComponent.ToggleEquip(Item);
            return true;
        }

    }
}
