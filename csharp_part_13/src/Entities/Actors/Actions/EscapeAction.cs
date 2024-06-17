namespace SuperRogalik
{
    public partial class EscapeAction : Action
    {
        public EscapeAction(Entity entity) : base(entity) { }

        public override bool Perform()
        {
            Entity.MapData.Save();
            SignalBus.Instance.EmitSignal(SignalBus.SignalName.EscapeRequested);
            return false;
        }
    }
}
