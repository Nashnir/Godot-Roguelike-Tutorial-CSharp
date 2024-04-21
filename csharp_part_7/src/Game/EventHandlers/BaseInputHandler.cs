using Godot;

namespace SuperRogalik
{
    public abstract partial class BaseInputHandler : Node
	{
        public virtual void Enter() {}
		
		public virtual void Exit() {}

        public virtual Action GetAction(Entity player) =>
			null;
	}
}