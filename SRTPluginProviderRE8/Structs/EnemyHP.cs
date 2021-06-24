using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SRTPluginProviderRE8.Structs
{
    public class Boss
    {
        //public static float[] HitPoints = { 2900, 3400, 3700, 9000, 25000, 26000, 30000, 75000, 100000 };
        public static Dictionary<float, string> Bosses = new Dictionary<float, string>()
        {
            { 2900, "Bella" },
            { 3400, "Cassandra" },
            { 3700, "Daniella" },
            { 9000, "Lady D" },
            { 25000, "Urias" },
            { 26000, "Moreau" },
            { 30000, "Miranda" },
            { 75000, "Urias" },
            { 100000, "Heisenberg" }
        };
    }

    [DebuggerDisplay("{_DebuggerDisplay,nq}")]
    public struct EnemyHP
    {
        /// <summary>
        /// Debugger display message.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string _DebuggerDisplay
        {
            get
            {
                if (IsTrigger)
                {
                    return string.Format("TRIGGER", CurrentHP, MaximumHP, Percentage);
                }  
                else if (IsAlive)
                {
                    return string.Format("{0} / {1} ({2:P1})", CurrentHP, MaximumHP, Percentage);
                }
                return "DEAD / DEAD (0%)";
            }
        }
        
        public bool IsBoss => Boss.Bosses.ContainsKey(_maximumHP);
        public string BossName => IsBoss ? Boss.Bosses[_maximumHP] : "";
        public float MaximumHP { get => _maximumHP; }
        internal float _maximumHP;
        public float CurrentHP { get => _currentHP; }
        internal float _currentHP;
        public bool IsTrigger => MaximumHP <= 10f || MaximumHP > 100000f || (MaximumHP == 100f && CurrentHP == 100f) || (MaximumHP == 999f && CurrentHP == 999f);
        public bool IsNaN => MaximumHP.CompareTo(float.NaN) == 0;
        public bool IsAlive => !IsNaN && !IsTrigger && MaximumHP != CurrentHP;
        public float Percentage => ((IsAlive) ? CurrentHP / MaximumHP : 0f);
    }
}
