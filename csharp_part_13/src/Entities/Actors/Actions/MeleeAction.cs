namespace SuperRogalik
{
    public partial class MeleeAction : ActionWithDirection
    {
        public MeleeAction(Entity entity, int dx, int dy) : base(entity, dx, dy) { }

        public override bool Perform()
        {
            var target = GetTargetActor();
            if (target == null)
            {
                if (Entity == GetMapData().Player)
                {
                    MessageLog.SendMessage("Nothing to attack.", GameColors.IMPOSSIBLE);
                }
                return false;
            }

            var damage = Entity.FighterComponent.Power - target.FighterComponent.Defense;

            var attackColor = Entity == GetMapData().Player ? GameColors.PLAYER_ATTACK : GameColors.ENEMY_ATTACK;
            var attackDescription = $"{Entity.EntityName} attacks {target.EntityName}";

            if (damage > 0)
            {
                attackDescription += $" for {damage} hit points.";
                target.FighterComponent.TakeDamage(damage);
            }
            else
            {
                attackDescription += " but does no damage.";
            }
            MessageLog.SendMessage(attackDescription, attackColor);
            return true;
        }
    }
}
