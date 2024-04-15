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

			var size = GetViewportRect().Size.Floor() / 2;
			var player_start_pos = Grid.WorldToGrid((Vector2I)size);
			player = new Entity(player_start_pos, playerDefinition);
			entities.AddChild(player);

			var npc = new Entity(player_start_pos + Vector2I.Right, playerDefinition);
			npc.Modulate = Colors.OrangeRed;
			entities.AddChild(npc);
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
