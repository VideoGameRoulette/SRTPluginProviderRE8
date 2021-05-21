using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SRTPluginProviderRE8.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct GameInventoryItem
    {
        [FieldOffset(0x38)] public byte HasAttachment;
        [FieldOffset(0x3C)] public uint ItemId;
        [FieldOffset(0x44)] public uint SlotPosition;
        [FieldOffset(0x4C)] public int Quantity;
        [FieldOffset(0x50)] public byte IsHorizontal;
        [FieldOffset(0x58)] public int Ammo;

        public static GameInventoryItem AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(GameInventoryItem*)pb;
            }
        }
    }
}
