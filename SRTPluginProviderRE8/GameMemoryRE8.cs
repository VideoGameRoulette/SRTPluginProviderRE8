using System;
using System.Globalization;
using System.Runtime.InteropServices;
using SRTPluginProviderRE8.Structs;
using SRTPluginProviderRE8.Structs.GameStructs;

namespace SRTPluginProviderRE8
{
    public class GameMemoryRE8 : IGameMemoryRE8
    {
        public PlayerStatus PlayerStatus { get => _playerstatus; set => _playerstatus = value; }
        internal PlayerStatus _playerstatus;

        public byte GameInit { get => _gameInit; set => _gameInit = value; }
        internal byte _gameInit;

        public byte PauseState { get => _pauseState; set => _pauseState = value; }
        internal byte _pauseState;

        public byte DukeState { get => _dukeState; set => _dukeState = value; }
        internal byte _dukeState;

        public uint GameState { get => _gameState; set => _gameState = value; }
        public uint _gameState;
        public uint CutsceneTimer { get => _cutsceneTimer; set => _cutsceneTimer = value; }
        internal uint _cutsceneTimer;

        public uint CutsceneState { get => _cutsceneState; set => _cutsceneState = value; }
        internal uint _cutsceneState;

        public uint CutsceneID { get => _cutsceneID; set => _cutsceneID = value; }
        internal uint _cutsceneID;

        public float PlayerCurrentHealth { get => _playerCurrentHealth; set => _playerCurrentHealth = value; }
        internal float _playerCurrentHealth;

        public float PlayerMaxHealth { get => _playerMaxHealth; set => _playerMaxHealth = value; }
        internal float _playerMaxHealth;

        public float PlayerPositionX { get => _playerPositionX; set => _playerPositionX = value; }
        internal float _playerPositionX;
        public float PlayerPositionY { get => _playerPositionY; set => _playerPositionY = value; }
        internal float _playerPositionY;
        public float PlayerPositionZ { get => _playerPositionZ; set => _playerPositionZ = value; }
        internal float _playerPositionZ;

        public int RankScore { get => _rankScore; set => _rankScore = value; }
        internal int _rankScore;

        public int Rank { get => _rank; set => _rank = value; }
        internal int _rank;

        public int Lei { get => _lei; set => _lei = value; }
        internal int _lei;

        public string CurrentView { get => _currentview; set => _currentview = value; }
        internal string _currentview;

        public string CurrentChapter { get => _currentchapter; set => _currentchapter = value; }
        internal string _currentchapter;

        public string TargetChapter { get => _targetchapter; set => _targetchapter = value; }
        internal string _targetchapter;
        public string CurrentRoom { get => _currentroom; set => _currentroom = value; }
        internal string _currentroom;

        public EnemyHP[] EnemyHealth { get => _enemyHealth; set => _enemyHealth = value; }
        internal EnemyHP[] _enemyHealth;

        public InventoryEntry LastKeyItem { get => _lastKeyItem; set => _lastKeyItem = value; }
        internal InventoryEntry _lastKeyItem;

        public InventoryEntry[] PlayerInventory { get => _playerInventory; set => _playerInventory = value; }
        internal InventoryEntry[] _playerInventory;
    }
}
