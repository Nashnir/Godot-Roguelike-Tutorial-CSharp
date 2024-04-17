namespace SuperRogalik
{
    public partial class MovementAction : ActionWithDirection
    {
        public MovementAction(Entity entity, int dx, int dy) : base(entity, dx, dy) { }

        public override void Perform()
        {
            var mapData = GetMapData();
            var destinationTile = mapData.GetTile(GetDestination());
            if (destinationTile == null || !destinationTile.IsWalkable())
                return;

            if (GetBlockingEntityAtDestination() != null)
                return;

            Entity.Move(Offset);
        }
    }
}
