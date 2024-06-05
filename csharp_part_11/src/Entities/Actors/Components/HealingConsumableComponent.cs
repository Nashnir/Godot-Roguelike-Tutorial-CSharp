namespace SuperRogalik
{
    public partial class HealingConsumableComponent : ConsumableComponent
    {
        public int Amount { get; set; }

        public HealingConsumableComponent(HealingConsumableComponentDefinition definition) =>
            Amount = definition.HealingAmount;

        public override bool Activate(ItemAction action)
        {
            var consumer = action.Entity;
            var amountRecovered = consumer.FighterComponent.Heal(Amount);

            if (amountRecovered > 0)
            {
                MessageLog.SendMessage(
                    $"You consume the {Entity.EntityName}, and recover {amountRecovered} HP!", GameColors.HEALTH_RECOVERED);
                Consume(consumer);
                return true;
            }
            MessageLog.SendMessage("Your health is already full.", GameColors.IMPOSSIBLE);
            return false;
        }
    }
}
