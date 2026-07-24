using TWR.Data;

namespace TWR.Units.Behaviors
{
    public class CavalryController : WarriorController
    {
        const float SpeedMultiplier = 1.8f;

        public override void InitializeStats(
            float hp, float atk, float range, float attackSpeed,
            WarriorFaction faction, bool isPlayer, float moveSpeed = 2f)
        {
            base.InitializeStats(hp, atk, range, attackSpeed, faction, isPlayer, moveSpeed * SpeedMultiplier);
        }
    }
}
