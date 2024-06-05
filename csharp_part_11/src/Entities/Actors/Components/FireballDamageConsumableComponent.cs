using System;
using System.Linq;

namespace SuperRogalik
{
    public partial class FireballDamageConsumableComponent : ConsumableComponent
    {
        public int Damage { get; set; }
        public int Radius { get; set; }

        public FireballDamageConsumableComponent(FireballDamageConsumableComponentDefinition definition)
        {
            Damage = definition.Damage;
            Radius = definition.Radius;
        }

        public override int GetTargetingRadius() =>
            Radius;

        public override bool Activate(ItemAction action)
        {
            var consumer = action.Entity;
            var targetPosition = action.TargetPosition;
            var mapData = consumer.MapData;

            if (!mapData.GetTile(targetPosition).IsInView)
            {
                MessageLog.SendMessage("You cannot target an area that you cannot see.", GameColors.IMPOSSIBLE);
                return false;
            }

            var targets = mapData.GetActors().Where(a => a.Distance(targetPosition) <= Radius);

            if (!targets.Any())
            {
                MessageLog.SendMessage("There are no targets in the radius.", GameColors.IMPOSSIBLE);
                return false;
            }

            if (targets.Count() == 1 && targets.First() == mapData.Player)
            {
                MessageLog.SendMessage("There are not enemy targets in the radius.", GameColors.IMPOSSIBLE);
                return false;
            }

            foreach (var target in targets)
            {
                MessageLog.SendMessage(
                    $"The {target.EntityName} is engulfed in a fiery explosion, taking {Damage} damage!",
                    GameColors.PLAYER_ATTACK);
                target.FighterComponent.TakeDamage(Damage);

            }

            Consume(action.Entity);
            return true;
        }
    }
}