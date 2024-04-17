using Godot;
using System.Collections.Generic;

namespace SuperRogalik
{
    public partial class InputHandler : Node
    {
        public enum InputHandlers { MAIN_GAME, GAME_OVER }

        [Export]
        public InputHandlers StartInputHandler { get; set; }

        public Dictionary<InputHandlers, BaseInputHandler> inputHandlerNodes;

        BaseInputHandler currentInputHandler;

        public override void _Ready()
        {
            inputHandlerNodes = new(){
                {InputHandlers.MAIN_GAME, GetNode<MainGameInputHandler>("MainGameInputHandler") },
                {InputHandlers.GAME_OVER, GetNode<GameOverInputHandler>("GameOverInputHandler") },
            };

            GetNode<SignalBus>("/root/SignalBus").PlayerDied += () => TransitionTo(InputHandlers.GAME_OVER);
            TransitionTo(StartInputHandler);
        }

        public Action GetAction(Entity player)
        {
            return currentInputHandler.GetAction(player);
        }

        private void TransitionTo(InputHandlers inputHandler) =>
            currentInputHandler = inputHandlerNodes[inputHandler];

    }
}
