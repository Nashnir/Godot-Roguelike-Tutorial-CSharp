namespace SuperRogalik
{
    public partial class EscapeAction : Action
    {
        public EscapeAction(Entity entity) : base(entity) { }

        public override bool Perform()
        {
            Entity.GetTree().Quit();
            return false;
        }
    }
}
