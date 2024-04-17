using Godot;

namespace SuperRogalik
{
    public partial class GameOverInputHandler : BaseInputHandler
    {
        public override Action GetAction(Entity player)
        {
            Action action = null;
            if (Input.IsActionJustPressed("quit")) 
                action = new EscapeAction(player);
            
            return action;
        }
    }
}
