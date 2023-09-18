using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Audio
{
    public class AudioClass
    {

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct IDXHeader
    {
        //0x47414241 or GABA(ABAG) indicates a audio IDX/BAG pair
        //0xEBBCEDFA regular data container, in NoX case BlowFish encrypted
        [FieldOffset(0)]
        public UInt32 Magic;
        [FieldOffset(4)]
        public UInt32 Unknown;//possibly some versioning
        [FieldOffset(8)]
        public UInt32 FileCount;
    };

    //following the header are count of FileCount Entry structs, in case of RA2 IDX its this structure
    public struct AudioIDXEntry
    {
        //char Name[16];//someone fucked up at WWP and didn't clear memory tho so make sure to read it to \0
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;
        public uint Offset;
        public uint Size;
        public uint SampleRate;
        public uint Flags;
        public uint ChunkSize;
    };

}
