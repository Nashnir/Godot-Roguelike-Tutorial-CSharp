using Godot;

namespace SuperRogalik
{
    public partial class Message : Label
    {
        private LabelSettings baseLabelSettings = ResourceLoader
            .Load<LabelSettings>("res://assets/resources/message_label_settings.tres");

        public string PlainText { get; set; }

        private int count = 1;
        public int Count
        {
            get => count;
            set
            {
                count = value;
                Text = FullText();
            }
        }

        public Message() { }

        public Message(string msgText, Color foregroundColor)
        {
            PlainText = msgText;
            LabelSettings = (LabelSettings) baseLabelSettings.Duplicate();
            LabelSettings.FontColor = foregroundColor;
            Text = PlainText;
            AutowrapMode = TextServer.AutowrapMode.WordSmart;
        }

        public string FullText() =>
            count > 1 ? $"{PlainText} (x{Count})" : PlainText;
    }
}
