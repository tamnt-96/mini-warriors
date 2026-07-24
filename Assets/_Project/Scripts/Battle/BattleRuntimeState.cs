using System.Collections.Generic;
using TWR.Data;

namespace TWR.Battle
{
    public class ActiveWarriorState
    {
        public WarriorDefinitionSO def;
        public bool   isUnlocked;
        public bool   isEvolved;
        public int    talentsPicked;
        public float  currentHP;
        public float  currentATK;
        public float  currentRange;
        public float  currentAttackSpeed;
        public float  currentSpawnCooldown;
        public float  spawnTimer;

        public ActiveWarriorState(WarriorDefinitionSO definition)
        {
            def                  = definition;
            currentHP            = definition.baseHP;
            currentATK           = definition.baseATK;
            currentRange         = definition.baseRange;
            currentAttackSpeed   = definition.baseAttackSpeed;
            currentSpawnCooldown = definition.spawnCooldown;
            spawnTimer           = 0f;
        }
    }

    public class ActiveBuffState
    {
        public BuffDefinitionSO def;
        public float            value;
    }

    public class ActiveSkillState
    {
        public SkillDefinitionSO def;
        public float             cdRemaining;
        public bool              isActive;
    }

    public class BattleRuntimeState
    {
        public StageDefinitionSO          stage;
        public int                        playerLevel       = 1;
        public int                        currentExp;
        public int                        expToNextLevel    = 10;
        public float                      playerCastleHP;
        public float                      enemyCastleHP;
        public bool                       wave70PctTriggered;
        public bool                       wave30PctTriggered;
        public bool                       heroAlive         = true;
        public bool                       heroReviveUsed;
        public List<ActiveWarriorState>   warriors          = new();
        public List<ActiveBuffState>      buffs             = new();
        public List<ActiveSkillState>     skills            = new();

        public void Initialize(StageDefinitionSO stageDef)
        {
            stage             = stageDef;
            playerCastleHP    = stageDef.playerCastleMaxHP;
            enemyCastleHP     = stageDef.enemyCastleMaxHP;
            playerLevel       = 1;
            currentExp        = 0;
            expToNextLevel    = 10;
            wave70PctTriggered = false;
            wave30PctTriggered = false;
            heroAlive          = true;
            heroReviveUsed     = false;
            warriors.Clear();
            buffs.Clear();
            skills.Clear();
        }
    }
}