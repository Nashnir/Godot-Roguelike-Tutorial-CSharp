namespace SuperRogalik
{
    public partial class WaitAction : Action
    {
        public WaitAction(Entity entity) : base(entity) { }

        public override bool Perform() =>
            true;
    }
}
