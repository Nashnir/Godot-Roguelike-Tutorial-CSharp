namespace SuperRogalik
{
    public partial class BumpAction : ActionWithDirection
    {
        public BumpAction() : base() { }

        public BumpAction(int dx, int dy) : base(dx,dy) { }

        public override void Perform(Game game, Entity entity)
        {
            var destination = entity.GridPosition + Offset;

            if (game.GetMapData().GetBlockingEntityAtLocation(destination) != null) 
            {
                new MeleeAction(Offset.X, Offset.Y).Perform(game, entity);
            }
            else 
            {
                new MovementAction(Offset.X, Offset.Y).Perform(game, entity);
            }
        }
    }
}
