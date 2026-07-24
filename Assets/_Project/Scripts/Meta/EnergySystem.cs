using System;
using TWR.Save;

namespace TWR.Meta
{
    public class EnergySystem
    {
        const int   MaxEnergy       = 60;
        const float RegenPerMinute  = 1f;
        const float RegenIntervalSec = 60f / RegenPerMinute;

        readonly ProgressService _progress;

        public int   Current  => _progress.Data.energy;
        public int   Max      => MaxEnergy;

        public EnergySystem(ProgressService progress)
        {
            _progress = progress;
            ApplyOfflineRegen();
        }

        public bool TrySpend(int amount)
        {
            ApplyOfflineRegen();
            if (_progress.Data.energy < amount) return false;
            _progress.Data.energy -= amount;
            _progress.Save();
            return true;
        }

        public void Add(int amount)
        {
            _progress.Data.energy = Math.Min(MaxEnergy, _progress.Data.energy + amount);
            _progress.Save();
        }

        public float SecondsUntilNextRegen()
        {
            if (_progress.Data.energy >= MaxEnergy) return 0f;
            long now     = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsed = now - _progress.Data.energyLastRegenTimestamp;
            float remainder = RegenIntervalSec - (elapsed % (long)RegenIntervalSec);
            return remainder;
        }

        void ApplyOfflineRegen()
        {
            if (_progress.Data.energy >= MaxEnergy) return;

            long now     = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsed = now - _progress.Data.energyLastRegenTimestamp;
            int  gained  = (int)(elapsed / RegenIntervalSec);

            if (gained <= 0) return;

            _progress.Data.energy = Math.Min(MaxEnergy, _progress.Data.energy + gained);
            _progress.Data.energyLastRegenTimestamp = now;
            _progress.Save();
        }
    }
}