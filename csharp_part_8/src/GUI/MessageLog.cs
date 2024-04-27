using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class MessageLog : ScrollContainer
    {
        private static MessageLog instance;

        private Message lastMessage = null;

        private VBoxContainer messageList;

        public MessageLog()
        {
            instance = this;
        }

        public override void _Ready()
        {
            messageList = GetNode<VBoxContainer>("%MessageList");
            SignalBus.Instance.MessageSent += AddMessage;
        }

        public async void AddMessage(string text, Color color)
        {
            if (lastMessage != null && lastMessage.PlainText == text)
            {
                lastMessage.Count += 1;
            }
            else
            {
                var message = new Message(text, color);
                lastMessage = message;
                messageList.AddChild(message);
                await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
                EnsureControlVisible(message);
            }
        }

        public static void SendMessage(string text, Color color) =>
            instance?.CallDeferred(nameof(SendMessageImpl), text, color);

        private static void SendMessageImpl(string text, Color color) =>
            SignalBus.Instance?.EmitSignal(SignalBus.SignalName.MessageSent, text, color);
    }
}