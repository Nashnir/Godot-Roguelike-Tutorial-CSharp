namespace SuperRogalik
{
    public partial class ConfusionConsumableComponent : ConsumableComponent
    {
        int numberOfTurns;

        public ConfusionConsumableComponent(ConfusionConsumableComponentDefinition definition)
        {
            numberOfTurns = definition.NumberOfTurns;
        }

        public override int GetTargetingRadius() =>
            0;

        public override bool Activate(ItemAction action)
        {
            var consumer = action.Entity;
            var target = action.GetTargetActor();
            var mapData = consumer.MapData;

            if (!mapData.GetTile(action.TargetPosition).IsInView)
            {
                MessageLog.SendMessage("You cannot target an area that you cannot see.", GameColors.IMPOSSIBLE);
                return false;
            }

            if (target == null)
            {
                MessageLog.SendMessage("You must select an enemy to target.", GameColors.IMPOSSIBLE);
                return false;
            }

            if (target == consumer)
            {
                MessageLog.SendMessage("You cannot confuse yourself!", GameColors.IMPOSSIBLE);
                return false;
            }

            MessageLog.SendMessage($"The eyes of the {target.EntityName} look vacant, as it starts to stumble around!",
                GameColors.STATUS_EFFECT_APPLIED);
            target.AddChild(new ConfusedEnemyAIComponent(numberOfTurns));
            Consume(consumer);
            return true;
        }
    }
}
