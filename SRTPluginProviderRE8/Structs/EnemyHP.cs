using System.Diagnostics;

namespace SRTPluginProviderRE8.Structs
{
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
