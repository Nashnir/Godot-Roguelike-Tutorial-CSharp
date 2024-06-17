using Godot;

namespace SuperRogalik
{
    [GlobalClass]
    public partial class EntityDefinition : Resource
    {
        [ExportCategory("Visuals")]

        [Export]
        public string Name { get; set; } = "Unnamed Entity";

        [Export]
        public AtlasTexture Texture { get; set; }

        [Export(PropertyHint.ColorNoAlpha)]
        public Color Color { get; set; } = Colors.White;


        [ExportCategory("Mechanics")]

        [Export]
        public bool IsBlockingMovement { get; set; } = true;

        [Export]
        public EntityType EntityType { get; set; } = EntityType.ACTOR;


        [ExportCategory("Components")]

        [Export]
        public FighterComponentDefinition FighterComponentDefinition { get; set; }

        [Export]
        public Entity.AIType AIType { get; set; } = Entity.AIType.NONE;

        [Export]
        public ItemComponentDefinition ItemComponentDefinition { get; set; }

        [Export]
        public int InventoryCapacity { get; set; } = 0;

        [Export]
        public LevelComponentDefinition LevelInfo { get; set; }

        [Export]
        public bool HasEquipment { get; set; }

    }
}
