using Godot;
using System.Linq;

namespace SuperRogalik
{
    public partial class LightningDamageConsumableComponent : ConsumableComponent
    {
        public int Damage { get; set; } = 0;

        public int MaximumRange { get; set; } = 0;

        public LightningDamageConsumableComponent(LightningDamageConsumableComponentDefinition definition)
        {
            Damage = definition.Damage;
            MaximumRange = definition.MaximumRange;
        }

        public override bool Activate(ItemAction action)
        {
            var consumer = action.Entity;
            Entity target = null;
            var closestDistance = MaximumRange + 1;
            var mapData = consumer.MapData;

            var actors = mapData.GetActors().Where(a => a != consumer && mapData.GetTile(a.GridPosition).IsInView);
            foreach (var actor in actors)
            {
                var distance = consumer.Distance(actor.GridPosition);
                if (distance < closestDistance)
                {
                    target = actor;
                    closestDistance = distance;
                }
            }

            if (target == null)
            {
                MessageLog.SendMessage("No enemy is close enough to strike.", GameColors.IMPOSSIBLE);
                return false;
            }

            MessageLog.SendMessage(
                    $"A lightning bolt strikes {target.EntityName} with a loud thunder, for {Damage} damage!",
                    Colors.White);

            target.FighterComponent.TakeDamage(Damage);
            Consume(consumer);
            return true;
        }
    }
}
