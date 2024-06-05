namespace SuperRogalik
{
    public partial class TakeStairsAction : Action
    {
        public TakeStairsAction(Entity entity) : base(entity)
        {
        }

        public override bool Perform()
        {
            if (Entity.GridPosition == GetMapData().DownStairsLocation) 
            {
                SignalBus.Instance.EmitSignal(SignalBus.SignalName.PlayerDescended);
                MessageLog.SendMessage("You descend the staircase.", GameColors.DESCEND);
                return true;
            } 
            else 
            {
                MessageLog.SendMessage("There are no stairs here.", GameColors.IMPOSSIBLE);
                return false;
            }
        }
    }
}
