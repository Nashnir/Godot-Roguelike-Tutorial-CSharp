using Godot;

namespace SuperRogalik
{
    public partial class HostileEnemyAIComponent : BaseAIComponent
    {
        private Vector2[] path;

        public override void Perform()
        {
            var target = GetMapData().Player;
            var targetGridPosition = target.GridPosition;
            var offset = targetGridPosition - Entity.GridPosition;
            var distance = Mathf.Max(Mathf.Abs(offset.X), Mathf.Abs(offset.Y));

            if (GetMapData().GetTile(Entity.GridPosition).IsInView)
            {
                if (distance <= 1)
                    new MeleeAction(Entity, offset.X, offset.Y).Perform();

                path = GetPointPathTo(targetGridPosition);
                path = path[1..^0];
            }
            if (path != null && path.Length > 0)
            {
                var destination = (Vector2I)path[0];
                if (GetMapData().GetBlockingEntityAtLocation(destination) != null)
                    new WaitAction(Entity).Perform();

                path = path[1..^0];
                var moveOffset = destination - Entity.GridPosition;
                new MovementAction(Entity, moveOffset.X, moveOffset.Y).Perform();
            }

            new WaitAction(Entity).Perform();
        }
    }
}
