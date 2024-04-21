using Godot;
using SuperRogalik.src.Game.EventHandlers;
using System.Collections.Generic;

namespace SuperRogalik
{
    public partial class InputHandler : Node
    {
        public enum InputHandlers { MAIN_GAME, GAME_OVER, HISTORY_VIEWER }

        [Export]
        public InputHandlers StartInputHandler { get; set; }

        public Dictionary<InputHandlers, BaseInputHandler> inputHandlerNodes;

        BaseInputHandler currentInputHandler;

        public override void _Ready()
        {
            inputHandlerNodes = new(){
                {InputHandlers.MAIN_GAME, GetNode<MainGameInputHandler>("MainGameInputHandler") },
                {InputHandlers.GAME_OVER, GetNode<GameOverInputHandler>("GameOverInputHandler") },
                {InputHandlers.HISTORY_VIEWER, GetNode<HistoryViewerInputHandler>("HistoryViewerInputHandler") },
            };

            SignalBus.Instance.PlayerDied += () => TransitionTo(InputHandlers.GAME_OVER);
            TransitionTo(StartInputHandler);
        }

        public Action GetAction(Entity player)
        {
            return currentInputHandler.GetAction(player);
        }

        public void TransitionTo(InputHandlers inputHandler)
        {
            currentInputHandler?.Exit();
            currentInputHandler = inputHandlerNodes[inputHandler];
            currentInputHandler.Enter();
        }
    }
}
