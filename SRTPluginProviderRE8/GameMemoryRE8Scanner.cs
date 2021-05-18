using ProcessMemory;
using static ProcessMemory.Extensions;
using System;
using System.Diagnostics;
using SRTPluginProviderRE8.Structs;
using System.Text;

namespace SRTPluginProviderRE8
{
    internal class GameMemoryRE8Scanner : IDisposable
    {
        // Variables
        private ProcessMemoryHandler memoryAccess;
        private GameMemoryRE8 gameMemoryValues;
        public bool HasScanned;
        public bool ProcessRunning => memoryAccess != null && memoryAccess.ProcessRunning;
        public int ProcessExitCode => (memoryAccess != null) ? memoryAccess.ProcessExitCode : 0;
        //private int EnemyTableCount;

        // Pointer Address Variables
        //private int pointerGameInit;
        private int pointerGameStates;
        private int pointerCutsceneStates;
        private int pointerDukeStates;
        private int pointerCutsceneID;
        private int pointerAddressHP;
        private int pointerPlayerPosition;
        private int pointerDA;
        private int pointerLei;

        private int pointerChapter;

        //private int pointerAddressEnemyCount;
        private int pointerAddressEnemies;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        //private MultilevelPointer PointerGameInit { get; set; }
        private MultilevelPointer PointerGameStates { get; set; }
        private MultilevelPointer PointerCutsceneStates { get; set; }
        private MultilevelPointer PointerDukeStates { get; set; }
        private MultilevelPointer PointerCutsceneID { get; set; }
        private MultilevelPointer PointerPlayerHP { get; set; }
        private MultilevelPointer PointerPlayerPosition { get; set; }
        private MultilevelPointer PointerDA { get; set; }
        private MultilevelPointer PointerLei { get; set; }

        // Map Pointers
        private MultilevelPointer PointerCurrentView { get; set; }
        private MultilevelPointer PointerCurrentChapter { get; set; }
        private MultilevelPointer PointerTargetChapter { get; set; }

        // Enemy Pointers
        private MultilevelPointer PointerEnemyCount { get; set; }
        private MultilevelPointer[] PointerEnemyEntries { get; set; }


        internal GameMemoryRE8Scanner(Process process = null)
        {
            gameMemoryValues = new GameMemoryRE8();
            if (process != null)
                Initialize(process);
        }

        internal unsafe void Initialize(Process process)
        {
            if (process == null)
                return; // Do not continue if this is null.

            if (!SelectPointerAddresses(GameHashes.DetectVersion(process.MainModule.FileName)))
                return; // Unknown version.

            int pid = GetProcessId(process).Value;
            memoryAccess = new ProcessMemoryHandler(pid);
            if (ProcessRunning)
            {
                BaseAddress = NativeWrappers.GetProcessBaseAddress(pid, PInvoke.ListModules.LIST_MODULES_64BIT); // Bypass .NET's managed solution for getting this and attempt to get this info ourselves via PInvoke since some users are getting 299 PARTIAL COPY when they seemingly shouldn't.

                // Setup the pointers.
                //PointerGameInit = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerGameInit));
                PointerGameStates = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerGameStates));
                PointerCutsceneStates = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerCutsceneStates));
                PointerDukeStates = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerDukeStates), 0xE0L, 0x70L, 0x70L, 0x18L, 0x20L);
                PointerCutsceneID = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerCutsceneID), 0xE0L);
                PointerPlayerHP = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressHP), 0x58L, 0x18L, 0x18L, 0x78L, 0x68L, 0x48L);
                PointerPlayerPosition = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerPlayerPosition), 0x180L, 0x50L);
                PointerDA = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerDA));
                PointerLei = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerLei), 0x60);
                PointerCurrentView = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerChapter), 0x58L);
                PointerCurrentChapter = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerChapter), 0x60L);
                PointerTargetChapter = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerChapter), 0x68L);

                //Enemies
                GenerateEnemyEntries();

            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE8_WW_20210506_1:
                    {
                        //pointerGameInit = 0x0A1A4690;
                        pointerGameStates = 0x0A19E058;
                        pointerCutsceneStates = 0x0A1B1BA8;
                        pointerDukeStates = 0x0A1BB228;
                        pointerCutsceneID = 0x0A185C78;
                        pointerAddressHP = 0x0A18D990;
                        pointerPlayerPosition = 0x0A1B1B70;
                        pointerDA = 0x0A1A50C0;
                        pointerLei = 0x0A1B1C70;
                        pointerAddressEnemies = 0x0A1B1D00;
                        pointerChapter = 0x0A1B1DE8;
                        return true;
                    }
                case GameVersion.RE8_CEROD_20210506_1:
                    {
                        //pointerGameInit = 0x0A1A4690 + 0x2000;
                        pointerGameStates = 0x0A19E058 + 0x2000;
                        pointerCutsceneStates = 0x0A1B1BA8 + 0x2000;
                        pointerDukeStates = 0x0A1BB228 + 0x2000;
                        pointerCutsceneID = 0x0A185C78 + 0x2000;
                        pointerAddressHP = 0x0A18D990 + 0x2000;
                        pointerPlayerPosition = 0x0A1B1B70 + 0x2000;
                        pointerDA = 0x0A1A50C0 + 0x2000;
                        pointerLei = 0x0A1B1C70 + 0x2000;
                        pointerAddressEnemies = 0x0A1B1D00 + 0x2000;
                        pointerChapter = 0x0A1B1DE8 + 0x2000;
                        return true;
                    }
                case GameVersion.RE8_PROMO_01_20210426_1:
                    {
                        //pointerGameInit = 0x0A1A4690 + 0x1030;
                        pointerGameStates = 0x0A19E058 + 0x1030;
                        pointerCutsceneStates = 0x0A1B1BA8 + 0x1030;
                        pointerDukeStates = 0x0A1BB228 + 0x1030;
                        pointerCutsceneID = 0x0A185C78 + 0x1030;
                        pointerAddressHP = 0x0A18D990 + 0x1030;
                        pointerPlayerPosition = 0x0A1B1B70 + 0x1030;
                        pointerDA = 0x0A1A50C0 + 0x1030;
                        pointerLei = 0x0A1B1C70 + 0x1030;
                        pointerAddressEnemies = 0x0A1B1D00 + 0x1030;
                        pointerChapter = 0x0A1B1DE8 + 0x1030;
                        return true;
                    }
            }

            // If we made it this far... rest in pepperonis. We have failed to detect any of the correct versions we support and have no idea what pointer addresses to use. Bail out.
            return false;
        }

        /// <summary>
        /// Dereferences a 4-byte signed integer via the PointerEnemyEntryCount pointer to detect how large the enemy pointer table is and then create the pointer table entries if required.
        /// </summary>
        private unsafe void GenerateEnemyEntries()
        {
            if (PointerEnemyEntries == null) // Enter if the pointer table is null (first run) or the size does not match.
            {
                PointerEnemyEntries = new MultilevelPointer[32]; // Create a new enemy pointer table array with the detected size.
                for (int i = 0; i < PointerEnemyEntries.Length; ++i) // Loop through and create all of the pointers for the table.
                    PointerEnemyEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x58L, 0x10L, (i * 0x8) + 0x28L, 0x228, 0x18L, 0x48L, 0x48L);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            //PointerGameInit.UpdatePointers();
            PointerGameStates.UpdatePointers();
            PointerCutsceneStates.UpdatePointers();
            PointerDukeStates.UpdatePointers();
            PointerCutsceneID.UpdatePointers();
            PointerPlayerHP.UpdatePointers();
            PointerPlayerPosition.UpdatePointers();
            PointerDA.UpdatePointers();
            PointerLei.UpdatePointers();
            PointerEnemyCount.UpdatePointers();
            PointerCurrentView.UpdatePointers();
            PointerCurrentChapter.UpdatePointers();
            PointerTargetChapter.UpdatePointers();
            GenerateEnemyEntries(); // This has to be here for the next part.
            for (int i = 0; i < PointerEnemyEntries.Length; ++i)
                PointerEnemyEntries[i].UpdatePointers();
        }

        internal unsafe IGameMemoryRE8 Refresh()
        {
            bool success;

            // Init Bosses On First Scan
            if (!HasScanned)
            {
                gameMemoryValues.EnemyHealth = new EnemyHP[32];
            }

            //Game States
            //fixed (byte* p = &gameMemoryValues._gameInit)
            //    success = PointerGameInit.TryDerefByte(0x8, p);

            fixed (byte* p = &gameMemoryValues._pauseState)
                success = PointerGameStates.TryDerefByte(0x48, p);

            fixed (uint* p = &gameMemoryValues._gameState)
                success = PointerGameStates.TryDerefUInt(0x40, p);

            fixed (uint* p = &gameMemoryValues._cutsceneState)
                success = PointerCutsceneStates.TryDerefUInt(0xFD4, p);

            fixed (uint* p = &gameMemoryValues._cutsceneID)
                success = PointerCutsceneID.TryDerefUInt(0x54, p);

            fixed (byte* p = &gameMemoryValues._dukeState)
                success = PointerDukeStates.TryDerefByte(0x114, p);

            // Player HP
            fixed (float* p = &gameMemoryValues._playerMaxHealth)
                success = PointerPlayerHP.TryDerefFloat(0x10, p);
            fixed (float* p = &gameMemoryValues._playerCurrentHealth)
                success = PointerPlayerHP.TryDerefFloat(0x14, p);

            // Player Position
            fixed (float* p = &gameMemoryValues._playerPositionX)
                success = PointerPlayerPosition.TryDerefFloat(0x180, p);
            fixed (float* p = &gameMemoryValues._playerPositionY)
                success = PointerPlayerPosition.TryDerefFloat(0x184, p);
            fixed (float* p = &gameMemoryValues._playerPositionZ)
                success = PointerPlayerPosition.TryDerefFloat(0x188, p);

            // DA
            fixed (int* p = &gameMemoryValues._rankScore)
                success = PointerDA.TryDerefInt(0x70, p);
            fixed (int* p = &gameMemoryValues._rank)
                success = PointerDA.TryDerefInt(0x74, p);

            // Lei
            fixed (int* p = &gameMemoryValues._lei)
                success = PointerLei.TryDerefInt(0x48, p);

            // Map Data
            int size1 = PointerCurrentView.DerefInt(0x10);
            int size2 = PointerCurrentChapter.DerefInt(0x10);
            int size3 = PointerTargetChapter.DerefInt(0x10);
            
            byte[] view = PointerCurrentView.DerefByteArray(0x14, size1);
            gameMemoryValues._currentview = Encoding.Unicode.GetString(view);
            
            byte[] current = PointerCurrentChapter.DerefByteArray(0x14, size2);
            gameMemoryValues._currentchapter = Encoding.Unicode.GetString(current);
            
            byte[] target = PointerTargetChapter.DerefByteArray(0x14, size3);
            gameMemoryValues._targetchapter = Encoding.Unicode.GetString(target);

            // Enemy HP
            GenerateEnemyEntries();
            if (gameMemoryValues._enemyHealth == null)
            {
                gameMemoryValues._enemyHealth = new EnemyHP[32];
                for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();
            }
            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyEntries[i].Address != IntPtr.Zero)
                    {
                        fixed (float* p = &gameMemoryValues.EnemyHealth[i]._maximumHP)
                            PointerEnemyEntries[i].TryDerefFloat(0x10, p);
                        fixed (float* p = &gameMemoryValues.EnemyHealth[i]._currentHP)
                            PointerEnemyEntries[i].TryDerefFloat(0x14, p);
                    }
                    else
                    {
                        // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                        // This happens when the game removes pointers from the table (map/room change).
                        gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                        gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                    }
                }
                catch
                {
                    gameMemoryValues.EnemyHealth[i]._maximumHP = 0;
                    gameMemoryValues.EnemyHealth[i]._currentHP = 0;
                }
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private int? GetProcessId(Process process) => process?.Id;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (memoryAccess != null)
                        memoryAccess.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~REmake1Memory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
