namespace SuperRogalik
{
    public partial class BumpAction : ActionWithDirection
    {
        public BumpAction(Entity entity, int dx, int dy) : base(entity, dx,dy) { }

        public override void Perform()
        {
            if (GetTargetActor() != null) 
            {
                new MeleeAction(Entity, Offset.X, Offset.Y).Perform();
            }
            else 
            {
                new MovementAction(Entity, Offset.X, Offset.Y).Perform();
            }
        }
    }
}
