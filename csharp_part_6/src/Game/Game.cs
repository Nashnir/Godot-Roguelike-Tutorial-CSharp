using Godot;
using System.Linq;

namespace SuperRogalik
{
    public partial class Game : Node2D
	{
		Vector2I playerGridPos = Vector2I.Zero;
		Entity player;
		readonly EntityDefinition playerDefinition = ResourceLoader
			.Load<EntityDefinition>("res://assets/definitions/entities/actors/entity_definition_player.tres");

		InputHandler inputHandler;
		Map map;

		public override void _Ready()
		{
			inputHandler = GetNode<InputHandler>("InputHandler");
			map = GetNode<Map>("Map");
			var camera = GetNode<Camera2D>("Camera2D");

			player = new Entity(null, Vector2I.Zero, playerDefinition);
			RemoveChild(camera);
			player.AddChild(camera);
			map.Generate(player);
			map.UpdateFov(player.GridPosition);
        }

        public override void _PhysicsProcess(double delta)
        {
            var action = inputHandler.GetAction(player);
			if (action != null)
			{
				action.Perform();
				HandleEnemyTurns();
				map.UpdateFov(player.GridPosition);
			}
        }

        private void HandleEnemyTurns()
        {
            foreach (var entity in GetMapData().GetActors().Where(a => a != player && a.IsAlive()))
			{
				entity.AIComponent.Perform();
			}
        }

        public MapData GetMapData() =>
			map.MapData;

    }
}
