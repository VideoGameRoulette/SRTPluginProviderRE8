using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SRTPluginProviderRE8.Structs.GameStructs
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]

    public unsafe struct GamePlayerPosition
    {
        [FieldOffset(0x180)] public float X;
        [FieldOffset(0x184)] public float Y;
        [FieldOffset(0x188)] public float Z;

        public static GamePlayerPosition AsStruct(byte[] data)
        {
            fixed (byte* pb = &data[0])
            {
                return *(GamePlayerPosition*)pb;
            }
        }
    }
}
