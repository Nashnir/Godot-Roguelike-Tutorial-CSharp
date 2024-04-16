using Godot;

namespace SuperRogalik
{
    public partial class MeleeAction : ActionWithDirection
    {
        public MeleeAction() : base() { }

        public MeleeAction(int dx, int dy) : base(dx, dy) { }

        public override void Perform(Game game, Entity entity)
        {
            var destination = entity.GridPosition + Offset;
            var target = game.GetMapData().GetBlockingEntityAtLocation(destination);
            if (target == null)
                return;

            GD.Print($"You kick the {target.GetEntityName()}, much to it's annoyance!");
        }
    }
}