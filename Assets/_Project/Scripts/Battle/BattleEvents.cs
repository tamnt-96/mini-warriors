using TWR.Core;
using TWR.Data;

namespace TWR.Battle
{
    public struct BattleStartedEvent    : IEvent { public StageDefinitionSO stage; }
    public struct BattleVictoryEvent    : IEvent { }
    public struct BattleDefeatEvent     : IEvent { }
    public struct PlayerLeveledUpEvent  : IEvent { public int newLevel; }
    public struct EnemyDiedEvent        : IEvent { public EnemyDefinitionSO def; public UnityEngine.Vector3 position; }
    public struct WarriorDiedEvent      : IEvent { public WarriorDefinitionSO def; }
    public struct HeroDiedEvent         : IEvent { }
    public struct Castle70PctEvent      : IEvent { }
    public struct Castle30PctEvent      : IEvent { }
    public struct TalentPickedEvent     : IEvent { public TalentDefinitionSO talent; public WarriorDefinitionSO warrior; }
    public struct WarriorUnlockedEvent  : IEvent { public WarriorDefinitionSO warrior; }
    public struct WarriorEvolvedEvent   : IEvent { public WarriorDefinitionSO newForm; }
    public struct PhaseChangedEvent     : IEvent { public BattlePhase previous; public BattlePhase next; }
    public struct WaveStartedEvent      : IEvent { public int waveNumber; }
}