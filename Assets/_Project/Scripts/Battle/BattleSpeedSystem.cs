using UnityEngine;

namespace TWR.Battle
{
    public class BattleSpeedSystem : MonoBehaviour
    {
        const float Speed1x  = 1.0f;
        const float Speed15x = 1.5f;
        const float Speed2x  = 2.0f;

        public bool IsSpeed15Unlocked { get; set; }
        public bool IsSpeed2Unlocked  { get; set; }

        public float CurrentSpeed => Time.timeScale;

        void OnDestroy()
        {
            Time.timeScale = Speed1x;
        }

        public void SetSpeed(float speed)
        {
            if (speed >= Speed2x  && !IsSpeed2Unlocked)  return;
            if (speed >= Speed15x && !IsSpeed15Unlocked) return;
            Time.timeScale = speed;
        }

        public void ToggleSpeed()
        {
            if (Mathf.Approximately(Time.timeScale, Speed1x) && IsSpeed15Unlocked)
                Time.timeScale = Speed15x;
            else if (Mathf.Approximately(Time.timeScale, Speed15x) && IsSpeed2Unlocked)
                Time.timeScale = Speed2x;
            else
                Time.timeScale = Speed1x;
        }
    }
}