using Godot;

namespace SuperRogalik
{
    public partial class MeleeAction : ActionWithDirection
    {
        public MeleeAction(Entity entity, int dx, int dy) : base(entity, dx, dy) { }

        public override void Perform()
        {
            var target = GetTargetActor();
            if (target == null)
                return;

            var damage = Entity.FighterComponent.Power - target.FighterComponent.Defense;

            var attackDescription = $"{Entity.EntityName} attacks {target.EntityName}";

            if (damage > 0)
            {
                attackDescription += $" for {damage} hit points.";
                target.FighterComponent.HP -= damage;
            }
            else
            {
                attackDescription += " but does no damage.";
            }
            GD.Print(attackDescription);
        }
    }
}