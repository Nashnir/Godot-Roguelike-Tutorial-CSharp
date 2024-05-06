using Godot;
using System.Threading.Tasks;

namespace SuperRogalik
{
    public partial class GameOverInputHandler : BaseInputHandler
    {
        public override Task<Action> GetActionAsync(Entity player)
        {
            Action action = null;
            if (Input.IsActionJustPressed("quit") || Input.IsActionJustPressed("ui_back"))
                action = new EscapeAction(player);

            return new Task<Action>(() => action);
        }
    }
}
