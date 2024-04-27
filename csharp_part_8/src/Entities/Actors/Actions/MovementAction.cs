namespace SuperRogalik
{
    public partial class MovementAction : ActionWithDirection
    {
        public MovementAction(Entity entity, int dx, int dy) : base(entity, dx, dy) { }

        public override bool Perform()
        {
            var mapData = GetMapData();
            var destinationTile = mapData.GetTile(GetDestination());
            if (destinationTile == null || !destinationTile.IsWalkable() || GetBlockingEntityAtDestination() != null)
            {
		        if (Entity == GetMapData().Player)
                {
                    MessageLog.SendMessage("That way is blocked.", GameColors.IMPOSSIBLE);
                }
                return false;
            }
            Entity.Move(Offset);
            return true;
        }
    }
}
