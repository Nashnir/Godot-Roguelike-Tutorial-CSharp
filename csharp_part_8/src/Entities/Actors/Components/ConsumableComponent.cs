namespace SuperRogalik
{
    public abstract partial class ConsumableComponent : Component
    {
        public Action GetAction(Entity consumer) =>
            new ItemAction(consumer, Entity);

        public void Consume(Entity consumer)
        {
            var inventory = consumer.InventoryComponent;
            inventory.Items.Remove(Entity);
            Entity.QueueFree();
        }

        public abstract bool Activate(ItemAction action);
    }
}
