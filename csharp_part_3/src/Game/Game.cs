using Godot;

namespace SuperRogalik
{
    public partial class Game : Node2D
	{
		Vector2I playerGridPos = Vector2I.Zero;
		Entity player;
		readonly EntityDefinition playerDefinition = ResourceLoader
			.Load<EntityDefinition>("res://assets/definitions/entities/actors/entity_definition_player.tres");

		EventHandler eventHandler;
		Node2D entities;
		Map map;

		public override void _Ready()
		{
			eventHandler = GetNode<EventHandler>("EventHandler");
			entities = GetNode<Node2D>("Entities");
			map = GetNode<Map>("Map");
			var camera = GetNode<Camera2D>("Camera2D");

			player = new Entity(Vector2I.Zero, playerDefinition);
			RemoveChild(camera);
			player.AddChild(camera);
            entities.AddChild(player);
			map.Generate(player);
        }

        public MapData GetMapData() =>
			map.MapData;

        public override void _PhysicsProcess(double delta)
        {
            var action = EventHandler.GetAction();
			action?.Perform(this, player);
        }
	}
}
