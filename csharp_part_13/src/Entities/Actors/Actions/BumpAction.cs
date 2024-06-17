namespace SuperRogalik
{
    public partial class BumpAction : ActionWithDirection
    {
        public BumpAction(Entity entity, int dx, int dy) : base(entity, dx, dy) { }

        public override bool Perform()
        {
            bool hasAttackTarget = GetTargetActor() != null;
            return hasAttackTarget
                ? new MeleeAction(Entity, Offset.X, Offset.Y).Perform()
                : new MovementAction(Entity, Offset.X, Offset.Y).Perform();
        }
    }
}
