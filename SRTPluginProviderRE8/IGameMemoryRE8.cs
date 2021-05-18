using System;
using SRTPluginProviderRE8.Structs;

namespace SRTPluginProviderRE8
{
    public interface IGameMemoryRE8
    {
        byte GameInit { get; set; }
        byte PauseState { get; set; }
        byte DukeState { get; set; }

        uint GameState { get; set; }
        uint CutsceneState { get; set; }
        uint CutsceneID { get; set; }

        float PlayerCurrentHealth { get; set; }
        float PlayerMaxHealth { get; set; }
        float PlayerPositionX { get; set; }
        float PlayerPositionY { get; set; }
        float PlayerPositionZ { get; set; }
        int RankScore { get; set; }
        int Rank { get; set; }

        int Lei { get; set; }

        string CurrentView { get; set; }
        string CurrentChapter { get; set; }
        string TargetChapter { get; set; }

        EnemyHP[] EnemyHealth { get; set; }

    }
}
