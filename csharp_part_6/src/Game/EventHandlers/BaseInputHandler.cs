using Godot;

namespace SuperRogalik
{
    public abstract partial class BaseInputHandler : Node
	{
		public virtual Action GetAction(Entity player) =>
			null;
	}
}