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
		Map map;

		public override void _Ready()
		{
			eventHandler = GetNode<EventHandler>("EventHandler");
			map = GetNode<Map>("Map");
			var camera = GetNode<Camera2D>("Camera2D");

			player = new Entity(Vector2I.Zero, playerDefinition);
			RemoveChild(camera);
			player.AddChild(camera);
			map.Generate(player);
			map.UpdateFov(player.GridPosition);
        }

        public MapData GetMapData() =>
			map.MapData;

        public override void _PhysicsProcess(double delta)
        {
            var action = EventHandler.GetAction();
			var previousPlayerPosition = player.GridPosition;
			if (action != null)
			{
				action.Perform(this, player);
				HandleEnemyTurns();
				if (player.GridPosition != previousPlayerPosition)
				{
					map.UpdateFov(player.GridPosition);
				}
			}
        }

        private void HandleEnemyTurns()
        {
            foreach (var entity in GetMapData().Entities)
			{
				if (entity == player)
					continue;

				GD.Print($"The {entity.GetEntityName()} wonders when it will get to take a real turn.");
			}
        }
    }
}
