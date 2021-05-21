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
        private int EnemyTableCount;
        private int InventoryTableCount;

        // Pointer Address Variables
        private int pointerGameInit;
        private int pointerGameStates;
        private int pointerCutsceneTimer;
        private int pointerCutsceneStates;
        private int pointerDukeStates;
        private int pointerCutsceneID;
        private int pointerAddressHP;
        private int pointerPlayerPosition;
        private int pointerDA;
        private int pointerLei;

        private int pointerChapter;

        private int pointerAddressEnemies;

        private int pointerAddressItemCount;
        private int pointerAddressItems;

        // Pointer Classes
        private IntPtr BaseAddress { get; set; }
        private MultilevelPointer PointerGameInit { get; set; }
        private MultilevelPointer PointerGameStates { get; set; }
        private MultilevelPointer PointerCutsceneTimer { get; set; }
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

        private MultilevelPointer PointerInventoryCount { get; set; }
        private MultilevelPointer[] PointerInventoryEntries { get; set; }
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
                PointerGameInit = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerGameInit));
                PointerGameStates = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerGameStates));
                PointerCutsceneTimer = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerCutsceneTimer), 0x80L);
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
                PointerEnemyCount = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x58L, 0x10L);
                GenerateEnemyEntries();

                //Enemies
                PointerInventoryCount = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressItemCount), 0x88L);
                GenerateItemEntires();
            }
        }

        private bool SelectPointerAddresses(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.RE8_WW_20210506_1:
                    {
                        pointerGameInit = 0x0A1A4690;
                        pointerGameStates = 0x0A19E058;
                        pointerCutsceneTimer = 0x0A187430;
                        pointerCutsceneStates = 0x0A1B1BA8;
                        pointerDukeStates = 0x0A1BB228;
                        pointerCutsceneID = 0x0A185C78;
                        pointerAddressHP = 0x0A18D990;
                        pointerPlayerPosition = 0x0A1B1B70;
                        pointerDA = 0x0A1A50C0;
                        pointerLei = 0x0A1B1C70;
                        pointerAddressEnemies = 0x0A1B1D00;
                        pointerChapter = 0x0A1B1DE8;
                        pointerAddressItemCount = 0x0A1B1C70;
                        pointerAddressItems = 0x0A1A5880;
                        return true;
                    }
                case GameVersion.RE8_CEROD_20210506_1:
                    {
                        pointerGameInit = 0x0A1A4690 + 0x2000;
                        pointerGameStates = 0x0A19E058 + 0x2000;
                        pointerCutsceneTimer = 0x0A187430 + 0x2000;
                        pointerCutsceneStates = 0x0A1B1BA8 + 0x2000;
                        pointerDukeStates = 0x0A1BB228 + 0x2000;
                        pointerCutsceneID = 0x0A185C78 + 0x2000;
                        pointerAddressHP = 0x0A18D990 + 0x2000;
                        pointerPlayerPosition = 0x0A1B1B70 + 0x2000;
                        pointerDA = 0x0A1A50C0 + 0x2000;
                        pointerLei = 0x0A1B1C70 + 0x2000;
                        pointerAddressEnemies = 0x0A1B1D00 + 0x2000;
                        pointerChapter = 0x0A1B1DE8 + 0x2000;
                        pointerAddressItemCount = 0x0A1B1C70 + 0x2000;
                        pointerAddressItems = 0x0A1A5880 + 0x2000;
                        return true;
                    }
                case GameVersion.RE8_PROMO_01_20210426_1:
                    {
                        pointerGameInit = 0x0A1A4690 + 0x1030;
                        pointerGameStates = 0x0A19E058 + 0x1030;
                        pointerCutsceneTimer = 0x0A187430 + 0x1030;
                        pointerCutsceneStates = 0x0A1B1BA8 + 0x1030;
                        pointerDukeStates = 0x0A1BB228 + 0x1030;
                        pointerCutsceneID = 0x0A185C78 + 0x1030;
                        pointerAddressHP = 0x0A18D990 + 0x1030;
                        pointerPlayerPosition = 0x0A1B1B70 + 0x1030;
                        pointerDA = 0x0A1A50C0 + 0x1030;
                        pointerLei = 0x0A1B1C70 + 0x1030;
                        pointerAddressEnemies = 0x0A1B1D00 + 0x1030;
                        pointerChapter = 0x0A1B1DE8 + 0x1030;
                        pointerAddressItemCount = 0x0A1B1C70 + 0x1030;
                        pointerAddressItems = 0x0A1A5880 + 0x1030;
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
            bool success;
            fixed (int* p = &EnemyTableCount)
                success = PointerEnemyCount.TryDerefInt(0x1C, p);

            if (PointerEnemyEntries == null) // Enter if the pointer table is null (first run) or the size does not match.
            {
                PointerEnemyEntries = new MultilevelPointer[64]; // Create a new enemy pointer table array with the detected size.
                for (int i = 0; i < PointerEnemyEntries.Length; ++i) // Loop through and create all of the pointers for the table.
                {
                    PointerEnemyEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x58L, 0x10L, (i * 0x8) + 0x28L, 0x228, 0x18L, 0x48L, 0x48L);
                }
            }
        }

        private unsafe void GenerateItemEntires()
        {
            bool success;
            fixed (int* p = &InventoryTableCount)
                success = PointerInventoryCount.TryDerefInt(0x2C, p);

            //Console.WriteLine(string.Format("Items: {0}", InventoryTableCount));
            if (PointerInventoryEntries == null) // Enter if the pointer table is null (first run) or the size does not match.
            {
                PointerInventoryEntries = new MultilevelPointer[256];
                for (int i = 0; i < PointerInventoryEntries.Length; ++i)
                {
                    PointerInventoryEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressItems), 0x78L, 0x70L, (i * 0x8) + 0x20L, 0x58L);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void UpdatePointers()
        {
            PointerGameInit.UpdatePointers();
            PointerGameStates.UpdatePointers();
            PointerCutsceneTimer.UpdatePointers();
            PointerCutsceneStates.UpdatePointers();
            PointerDukeStates.UpdatePointers();
            PointerCutsceneID.UpdatePointers();
            PointerPlayerHP.UpdatePointers();
            PointerPlayerPosition.UpdatePointers();
            PointerDA.UpdatePointers();
            PointerLei.UpdatePointers();
            PointerCurrentView.UpdatePointers();
            PointerCurrentChapter.UpdatePointers();
            PointerTargetChapter.UpdatePointers();
            PointerEnemyCount.UpdatePointers();
            GenerateEnemyEntries(); // This has to be here for the next part.
            for (int i = 0; i < PointerEnemyEntries.Length; ++i)
            {
                PointerEnemyEntries[i].UpdatePointers();
            }
            PointerInventoryCount.UpdatePointers();
            GenerateItemEntires();
            for (int i = 0; i < PointerInventoryEntries.Length; ++i)
            {
                PointerInventoryEntries[i].UpdatePointers();
            }
        }

        internal unsafe IGameMemoryRE8 Refresh()
        {
            bool success;

            // Init Bosses On First Scan
            if (gameMemoryValues.EnemyHealth == null && gameMemoryValues.PlayerInventory == null)
            {
                gameMemoryValues.EnemyHealth = new EnemyHP[64];
                gameMemoryValues.LastKeyItem = new InventoryEntry();
                gameMemoryValues.PlayerInventory = new InventoryEntry[256];
            }

            //Game States
            fixed (byte* p = &gameMemoryValues._gameInit)
                success = PointerGameInit.TryDerefByte(0x8, p);

            fixed (byte* p = &gameMemoryValues._pauseState)
                success = PointerGameStates.TryDerefByte(0x48, p);

            fixed (uint* p = &gameMemoryValues._gameState)
                success = PointerGameStates.TryDerefUInt(0x40, p);

            fixed (uint* p = &gameMemoryValues._cutsceneTimer)
                success = PointerCutsceneTimer.TryDerefUInt(0x94, p);

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
            byte[] view = PointerCurrentView.DerefByteArray(0x14, size1 * 2);
            gameMemoryValues._currentview = GetString(view);

            int size2 = PointerCurrentChapter.DerefInt(0x10);
            int size3 = PointerTargetChapter.DerefInt(0x10);
            if (size2 > 0 && size3 > 0)
            {
                byte[] current = PointerCurrentChapter.DerefByteArray(0x14, size2 * 2);
                byte[] target = PointerTargetChapter.DerefByteArray(0x14, size3 * 2);
                gameMemoryValues._currentchapter = GetString(current);
                gameMemoryValues._targetchapter = GetString(target);
            }
            else
            {
                gameMemoryValues._currentchapter = "None";
                gameMemoryValues._targetchapter = "None";
            }

            // Enemy HP
            GenerateEnemyEntries();
            if (gameMemoryValues._enemyHealth == null)
            {
                gameMemoryValues._enemyHealth = new EnemyHP[64];
                for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();
            }
            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyEntries[i].Address != IntPtr.Zero && i < EnemyTableCount)
                    {
                        fixed (float* p = &gameMemoryValues.EnemyHealth[i]._maximumHP)
                            success = PointerEnemyEntries[i].TryDerefFloat(0x10, p);
                        fixed (float* p = &gameMemoryValues.EnemyHealth[i]._currentHP)
                            success = PointerEnemyEntries[i].TryDerefFloat(0x14, p);
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

            // Last Key Item
            fixed (uint* p = &gameMemoryValues._lastKeyItem._itemid)
                success = PointerInventoryEntries[0].TryDerefUInt(0x3C, p);

            // Inventory
            GenerateItemEntires();
            if (gameMemoryValues._playerInventory == null)
            {
                gameMemoryValues._playerInventory = new InventoryEntry[256];
                for (int i = 0; i < gameMemoryValues._playerInventory.Length; ++i)
                    gameMemoryValues._playerInventory[i] = new InventoryEntry();
            }
            for (int i = 0; i < gameMemoryValues._playerInventory.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerInventoryEntries[i].Address != IntPtr.Zero && i < InventoryTableCount)
                    {
                        fixed (uint* p = &gameMemoryValues.PlayerInventory[i]._itemid)
                            success = PointerInventoryEntries[i].TryDerefUInt(0x3C, p);
                        if (gameMemoryValues.PlayerInventory[i].IsItem)
                        {
                            gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                            fixed (uint* p = &gameMemoryValues.PlayerInventory[i]._slotPosition)
                                success = PointerInventoryEntries[i].TryDerefUInt(0x44, p);
                            fixed (int* p = &gameMemoryValues.PlayerInventory[i]._quantity)
                                success = PointerInventoryEntries[i].TryDerefInt(0x4C, p);
                            fixed (byte* p = &gameMemoryValues.PlayerInventory[i]._ishorizontal)
                                success = PointerInventoryEntries[i].TryDerefByte(0x50, p);
                            gameMemoryValues.PlayerInventory[i]._ammo = -1;
                        }
                        else if (gameMemoryValues.PlayerInventory[i].IsWeapon)
                        {
                            fixed (byte* p = &gameMemoryValues.PlayerInventory[i]._hasAttachment)
                                success = PointerInventoryEntries[i].TryDerefByte(0x3B, p);
                            fixed (uint* p = &gameMemoryValues.PlayerInventory[i]._slotPosition)
                                success = PointerInventoryEntries[i].TryDerefUInt(0x44, p);
                            fixed (int* p = &gameMemoryValues.PlayerInventory[i]._quantity)
                                success = PointerInventoryEntries[i].TryDerefInt(0x4C, p);
                            fixed (byte* p = &gameMemoryValues.PlayerInventory[i]._ishorizontal)
                                success = PointerInventoryEntries[i].TryDerefByte(0x50, p);
                            fixed (int* p = &gameMemoryValues.PlayerInventory[i]._ammo)
                                success = PointerInventoryEntries[i].TryDerefInt(0x58, p);
                        }
                        else if (gameMemoryValues.PlayerInventory[i].IsKeyItem || gameMemoryValues.PlayerInventory[i].IsCraftable || gameMemoryValues.PlayerInventory[i].IsTreasure)
                        {
                            gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                            gameMemoryValues.PlayerInventory[i]._slotPosition = 255;
                            fixed (int* p = &gameMemoryValues.PlayerInventory[i]._quantity)
                                success = PointerInventoryEntries[i].TryDerefInt(0x4C, p);
                            gameMemoryValues.PlayerInventory[i]._ishorizontal = 0;
                            gameMemoryValues.PlayerInventory[i]._ammo = -1;
                        }
                        else
                        {
                            gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                            gameMemoryValues.PlayerInventory[i]._slotPosition = 0;
                            gameMemoryValues.PlayerInventory[i]._quantity = 0;
                            gameMemoryValues.PlayerInventory[i]._ishorizontal = 0;
                            gameMemoryValues.PlayerInventory[i]._ammo = -1;
                        }
                    }
                    else
                    {
                        // Clear these values out so stale data isn't left behind when the pointer address is no longer value and nothing valid gets read.
                        // This happens when the game removes pointers from the table (map/room change).
                        gameMemoryValues.PlayerInventory[i]._itemid = 0;
                        gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                        gameMemoryValues.PlayerInventory[i]._slotPosition = 0;
                        gameMemoryValues.PlayerInventory[i]._quantity = 0;
                        gameMemoryValues.PlayerInventory[i]._ishorizontal = 0;
                        gameMemoryValues.PlayerInventory[i]._ammo = -1;
                    }
                }
                catch
                {
                    gameMemoryValues.PlayerInventory[i]._itemid = 0;
                    gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                    gameMemoryValues.PlayerInventory[i]._slotPosition = 0;
                    gameMemoryValues.PlayerInventory[i]._quantity = 0;
                    gameMemoryValues.PlayerInventory[i]._ishorizontal = 0;
                    gameMemoryValues.PlayerInventory[i]._ammo = -1;
                }
            }

            HasScanned = true;
            return gameMemoryValues;
        }

        private string GetString(byte[] value)
        {
            if (value != null)
            {
                return Encoding.Unicode.GetString(value);
            }
            return "";
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
