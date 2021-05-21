using ProcessMemory;
using static ProcessMemory.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SRTPluginProviderRE8.Structs;
using System.Text;
using SRTPluginProviderRE8.Structs.GameStructs;

namespace SRTPluginProviderRE8
{
    internal class GameMemoryRE8Scanner : IDisposable
    {
        private static readonly int MAX_ENTITIES = 64;
        private static readonly int MAX_ITEMS = 256;

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
        private MultilevelPointer PointerEnemyEntryList { get; set; }
        private MultilevelPointer[] PointerEnemyEntries { get; set; }

        private MultilevelPointer PointerInventoryCount { get; set; }
        private MultilevelPointer PointerInventoryEntryList { get; set; }
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
                PointerEnemyEntryList = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x58L, 0x10L);

                //Enemies
                PointerEnemyCount = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressEnemies), 0x58L, 0x10L);
                GenerateEnemyEntries();
               
                //Items
                PointerInventoryCount = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressItemCount), 0x88L);
                PointerInventoryEntryList = new MultilevelPointer(memoryAccess, IntPtr.Add(BaseAddress, pointerAddressItems), 0x78L, 0x70L);

                GenerateItemEntries();
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

            PointerEnemyEntries = new MultilevelPointer[MAX_ENTITIES]; // Create a new enemy pointer table array with the detected size.

            // Skip the first 28 bytes and read the rest as a byte array
            // This can be done because the pointers are stored sequentially in an array
            byte[] entityPtrByteArr = PointerEnemyEntryList.DerefByteArray(0x28, MAX_ENTITIES * sizeof(IntPtr));

            // Do a block copy to convert the byte array to an IntPtr array
            IntPtr[] entityPtrArr = new IntPtr[MAX_ENTITIES];
            Buffer.BlockCopy(entityPtrByteArr, 0, entityPtrArr, 0, entityPtrByteArr.Length);

            // The pointers we read are already the address of the entity, so make sure we add the first offset here
            for (int i = 0; i < PointerEnemyEntries.Length; ++i) // Loop through and create all of the pointers for the table.
            {
                PointerEnemyEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(entityPtrArr[i], 0x228), 0x18L, 0x48L, 0x48L);
            }
        }
        private unsafe void GenerateItemEntries()
        {
            bool success;
            fixed (int* p = &InventoryTableCount)
                success = PointerInventoryCount.TryDerefInt(0x2C, p);

            PointerInventoryEntries = new MultilevelPointer[MAX_ITEMS];

            // Skip the first 28 bytes and read the rest as a byte array
            // This can be done because the pointers are stored sequentially in an array
            byte[] inventoryEntriesPtrByteArr = PointerInventoryEntryList.DerefByteArray(0x20, MAX_ITEMS * sizeof(IntPtr));

            // Do a block copy to convert the byte array to an IntPtr array
            IntPtr[] inventoryEntriesPtrArr = new IntPtr[MAX_ITEMS];
            Buffer.BlockCopy(inventoryEntriesPtrByteArr, 0, inventoryEntriesPtrArr, 0, inventoryEntriesPtrByteArr.Length);

            for (int i = 0; i < PointerInventoryEntries.Length; ++i)
            {
                PointerInventoryEntries[i] = new MultilevelPointer(memoryAccess, IntPtr.Add(inventoryEntriesPtrArr[i], 0x58));
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
            PointerEnemyEntryList.UpdatePointers();

            GenerateEnemyEntries();

            PointerInventoryCount.UpdatePointers();
            PointerInventoryEntryList.UpdatePointers();
            GenerateItemEntries();
        }

        internal unsafe IGameMemoryRE8 Refresh()
        {
            bool success;

            // Init Bosses On First Scan
            if (gameMemoryValues.EnemyHealth == null && gameMemoryValues.PlayerInventory == null)
            {
                gameMemoryValues.EnemyHealth = new EnemyHP[MAX_ENTITIES];
                gameMemoryValues.LastKeyItem = new InventoryEntry();
                gameMemoryValues.PlayerInventory = new InventoryEntry[MAX_ITEMS];
            }

            //Game States
            fixed (byte* p = &gameMemoryValues._gameInit)
                success = PointerGameInit.TryDerefByte(0x8, p);

            if (PointerGameStates.Address != IntPtr.Zero)
            {
                byte[] gameStateBytes = memoryAccess.GetByteArrayAt(PointerGameStates.Address, sizeof(GameStates));
                var gameStates = GameStates.AsStruct(gameStateBytes);
                gameMemoryValues._pauseState = gameStates.PauseState;
                gameMemoryValues._gameState = gameStates.GameState;
            }

            fixed (uint* p = &gameMemoryValues._cutsceneTimer)
                success = PointerCutsceneTimer.TryDerefUInt(0x94, p);
            fixed (uint* p = &gameMemoryValues._cutsceneState)
                success = PointerCutsceneStates.TryDerefUInt(0xFD4, p);

            fixed (uint* p = &gameMemoryValues._cutsceneID)
                success = PointerCutsceneID.TryDerefUInt(0x54, p);

            fixed (byte* p = &gameMemoryValues._dukeState)
                success = PointerDukeStates.TryDerefByte(0x114, p);

            // Player HP
            if (SafeReadByteArray(PointerPlayerHP.Address, sizeof(GamePlayerHP), out byte[] gamePlayerHpBytes))
            {
                var playerHp = GamePlayerHP.AsStruct(gamePlayerHpBytes);
                gameMemoryValues._playerMaxHealth = playerHp.Max;
                gameMemoryValues._playerCurrentHealth = playerHp.Current;
            }

            // Player Position
            if (SafeReadByteArray(PointerPlayerPosition.Address, sizeof(GamePlayerPosition), out byte[] gamePlayerPosBytes))
            {
                var playerPosition = GamePlayerPosition.AsStruct(gamePlayerPosBytes);
                gameMemoryValues._playerPositionX = playerPosition.X;
                gameMemoryValues._playerPositionY = playerPosition.Y;
                gameMemoryValues._playerPositionZ = playerPosition.Z;
            }

            // DA
            if (SafeReadByteArray(PointerDA.Address, sizeof(GameRank), out byte[] gameRankBytes))
            {
                var gameRank = GameRank.AsStruct(gameRankBytes);
                gameMemoryValues._rankScore = gameRank.Rank;
                gameMemoryValues._rank = gameRank.Score;
            }

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
            if (gameMemoryValues._enemyHealth == null)
            {
                gameMemoryValues._enemyHealth = new EnemyHP[MAX_ENTITIES];
                for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
                    gameMemoryValues._enemyHealth[i] = new EnemyHP();
            }
            for (int i = 0; i < gameMemoryValues._enemyHealth.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerEnemyEntries[i].Address != IntPtr.Zero && i < EnemyTableCount && SafeReadByteArray(PointerEnemyEntries[i].Address, sizeof(GamePlayerHP), out byte[] enemyHpBytes))
                    {
                        // Note, this is using the same structure as the player HP.
                        // This may not always be the case, but the structures match for now.
                        var enemyHp = GamePlayerHP.AsStruct(enemyHpBytes);
                        gameMemoryValues.EnemyHealth[i]._maximumHP = enemyHp.Max;
                        gameMemoryValues.EnemyHealth[i]._currentHP = enemyHp.Current;
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
            if (gameMemoryValues._playerInventory == null)
            {
                gameMemoryValues._playerInventory = new InventoryEntry[MAX_ITEMS];
                for (int i = 0; i < gameMemoryValues._playerInventory.Length; ++i)
                    gameMemoryValues._playerInventory[i] = new InventoryEntry();
            }
            for (int i = 0; i < gameMemoryValues._playerInventory.Length; ++i)
            {
                try
                {
                    // Check to see if the pointer is currently valid. It can become invalid when rooms are changed.
                    if (PointerInventoryEntries[i].Address != IntPtr.Zero && i < InventoryTableCount && SafeReadByteArray(PointerInventoryEntries[i].Address, sizeof(GameInventoryItem), out byte[] inventoryItemBytes))
                    {
                        var inventoryItem = GameInventoryItem.AsStruct(inventoryItemBytes);


                        // This hook is a little poorly done... but good enough for now
                        gameMemoryValues.PlayerInventory[i]._itemid = inventoryItem.ItemId;

                        if (gameMemoryValues.PlayerInventory[i].IsItem)
                        {
                            gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                            gameMemoryValues.PlayerInventory[i]._slotPosition = inventoryItem.SlotPosition;
                            gameMemoryValues.PlayerInventory[i]._quantity = inventoryItem.Quantity;
                            gameMemoryValues.PlayerInventory[i]._ishorizontal = inventoryItem.IsHorizontal;
                            gameMemoryValues.PlayerInventory[i]._ammo = -1;
                        }
                        else if (gameMemoryValues.PlayerInventory[i].IsWeapon)
                        {
                            gameMemoryValues.PlayerInventory[i]._hasAttachment = inventoryItem.HasAttachment;
                            gameMemoryValues.PlayerInventory[i]._slotPosition = inventoryItem.SlotPosition;
                            gameMemoryValues.PlayerInventory[i]._quantity = inventoryItem.Quantity;
                            gameMemoryValues.PlayerInventory[i]._ishorizontal = inventoryItem.IsHorizontal;
                            gameMemoryValues.PlayerInventory[i]._ammo = inventoryItem.Ammo;
                        }
                        else if (gameMemoryValues.PlayerInventory[i].IsKeyItem || gameMemoryValues.PlayerInventory[i].IsCraftable || gameMemoryValues.PlayerInventory[i].IsTreasure)
                        {
                            gameMemoryValues.PlayerInventory[i]._hasAttachment = 0;
                            gameMemoryValues.PlayerInventory[i]._slotPosition = 255;
                            gameMemoryValues.PlayerInventory[i]._quantity = inventoryItem.Quantity;
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


        private unsafe bool SafeReadByteArray(IntPtr address, int size, out byte[] readBytes)
        {
            readBytes = new byte[size];
            fixed (byte* p = readBytes)
            {
                return memoryAccess.TryGetByteArrayAt(address, size, p);
            }
        }

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
