using Godot;
using Godot.Collections;

namespace SuperRogalik
{
    public partial class ConfusedEnemyAIComponent : BaseAIComponent
    {
        private BaseAIComponent previousAi;
        private int turnsRemaining;
        private RandomNumberGenerator rng;

        private static readonly Vector2I[] directions = [
            new Vector2I(-1, -1),
            new Vector2I(0, -1),
            new Vector2I(1, -1),
            new Vector2I(-1, 0),
            new Vector2I(1, 0),
            new Vector2I(-1, 1),
            new Vector2I(0, 1),
            new Vector2I(1, 1),
        ];

        public override void _Ready()
        {
            base._Ready();
            previousAi = Entity.AIComponent;
            Entity.AIComponent = this;
            rng = new RandomNumberGenerator();
        }

        public ConfusedEnemyAIComponent(int turnsRemaining)
        {
            this.turnsRemaining = turnsRemaining;
        }

        public override void Perform()
        {
            if (turnsRemaining <= 0)
            {
                MessageLog.SendMessage($"The {Entity.EntityName} is no longer confused.", Colors.White);
                Entity.AIComponent = previousAi;
                QueueFree();
            }
            else
            {
                var rnd = rng.RandiRange(0, directions.Length - 1);
                var direction = directions[rnd];
                --turnsRemaining;
                new BumpAction(Entity, direction.X, direction.Y).Perform();
            }
        }

        public override Dictionary GetSaveData()
        {
            return new Dictionary() {
                {"type", "HostileEnemyAI"},
                { "turns_remaining", turnsRemaining }
            };
        }
    }
}