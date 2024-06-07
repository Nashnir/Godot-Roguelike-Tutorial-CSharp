using Godot;
using Godot.Collections;
using System;

namespace SuperRogalik
{
    public partial class LevelComponent : Component
    {
        [Signal]
        public delegate void LevelUpRequiredEventHandler();

        [Signal]
        public delegate void LeveledUpEventHandler();

        [Signal]
        public delegate void XpChangedEventHandler(int xp, int maxXp);

        public int CurrentLevel { get; set; } = 1;
        public int CurrentXp { get; set; } = 0;
        
        public int LevelUpBase { get; set; }
        public int LevelUpFactor { get; set; }
        public int XpGiven { get; set; }

        public LevelComponent(LevelComponentDefinition definition)
        {
            LevelUpBase = definition.LevelUpBase;
            LevelUpFactor = definition.LevelUpFactor;
            XpGiven = definition.XpGiven;
        }

        public int GetExperienceToNextLevel() =>
            LevelUpBase + CurrentLevel * LevelUpFactor;

        public bool IsLevelUpRequired() =>
            CurrentXp >= GetExperienceToNextLevel();

        public void AddXp(int xp)
        {
            if (xp == 0 || LevelUpBase == 0)
                return;

            CurrentXp += xp;
            MessageLog.SendMessage($"You gain {xp} experience points.", Colors.White);
            EmitSignal(SignalName.XpChanged, CurrentXp, GetExperienceToNextLevel());
	        if (IsLevelUpRequired())
            {
                MessageLog.SendMessage($"You advance to level {CurrentLevel + 1}!", Colors.White);
                EmitSignal(SignalName.LevelUpRequired);
            }
        }

        public void IncreaseLevel()
        {
            CurrentXp -= GetExperienceToNextLevel();
            CurrentLevel++;
            EmitSignal(SignalName.XpChanged, CurrentXp, GetExperienceToNextLevel());
            EmitSignal(SignalName.LeveledUp);
        }

        public void IncreaseMaxHp(int amount = 20)
        {
            var fighter = Entity.FighterComponent;
            fighter.MaxHP += amount;
            fighter.HP += amount;

            MessageLog.SendMessage("Your health improves!", Colors.White);
            IncreaseLevel();
        }


        public void IncreasePower(int amount = 1){
            var fighter = Entity.FighterComponent;
            fighter.Power += amount;

            MessageLog.SendMessage("You feel stronger!", Colors.White);
            IncreaseLevel();
        }

        public void IncreaseDefense(int amount = 1)
        {
            var fighter = Entity.FighterComponent;
            fighter.Defense += amount;

            MessageLog.SendMessage("Your movements are getting swifter!", Colors.White);
            IncreaseLevel();
        }

        public Dictionary GetSaveData()
        {
            return new Dictionary {
                { "current_level", CurrentLevel },
                { "current_xp", CurrentXp },
                { "level_up_base", LevelUpBase },
                { "level_up_factor", LevelUpFactor },
                { "xp_given", XpGiven },
            };
        }

        public void Restore(Dictionary saveData)
        {
            CurrentLevel = saveData["current_level"].AsInt32();
            CurrentXp = saveData["current_xp"].AsInt32();

            LevelUpBase = saveData["level_up_base"].AsInt32();
            LevelUpFactor = saveData["level_up_factor"].AsInt32();

            XpGiven = saveData["xp_given"].AsInt32();
        }
    }
}
