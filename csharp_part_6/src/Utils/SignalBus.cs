using Godot;

namespace SuperRogalik
{
    public partial class SignalBus : Node
    {
        [Signal]
        public delegate void PlayerDiedEventHandler();
    }
}
