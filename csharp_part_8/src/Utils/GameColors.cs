using Godot;

namespace SuperRogalik
{
    public partial class GameColors : RefCounted
    {
        public static readonly Color PLAYER_ATTACK = new Color("e0e0e0");
        public static readonly Color ENEMY_ATTACK = new Color("ffc0c0");
        public static readonly Color PLAYER_DIE = new Color("ff3030");
        public static readonly Color ENEMY_DIE = new Color("ffa030");
        public static readonly Color WELCOME_TEXT = new Color("20a0ff");

        public static readonly Color INVALID = new Color("ffff00");
        public static readonly Color IMPOSSIBLE = new Color("808080");
        public static readonly Color ERROR = new Color("ff4040");
        public static readonly Color HEALTH_RECOVERED = new Color("00ff00");
    }
}
