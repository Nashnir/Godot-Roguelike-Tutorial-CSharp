using Godot;
using SuperRogalik.src.Game.EventHandlers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuperRogalik
{
    public partial class InputHandler : Node
    {
        public enum InputHandlers { MAIN_GAME, GAME_OVER, HISTORY_VIEWER, DUMMY }

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
                {InputHandlers.DUMMY, GetNode<BaseInputHandler>("DummyInputHandler") },
            };

            SignalBus.Instance.PlayerDied += () => TransitionTo(InputHandlers.GAME_OVER);
            TransitionTo(StartInputHandler);
        }

        public async Task<Action> GetActionAsync(Entity player) =>
            await currentInputHandler.GetActionAsync(player);

        public void TransitionTo(InputHandlers inputHandler)
        {
            currentInputHandler?.Exit();
            currentInputHandler = inputHandlerNodes[inputHandler];
            currentInputHandler.Enter();
        }
    }
}
