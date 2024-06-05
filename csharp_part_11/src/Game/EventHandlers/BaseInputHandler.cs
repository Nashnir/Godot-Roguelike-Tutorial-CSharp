using Godot;
using System.Threading.Tasks;

namespace SuperRogalik
{
    public partial class BaseInputHandler : Node
    {
        public virtual void Enter() { }

        public virtual void Exit() { }

        public virtual Task<Action> GetActionAsync(Entity player) =>
            new Task<Action>(() => null);
    }
}