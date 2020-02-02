using System;

namespace HoLLy.Memory.Windows
{
    [Flags]
    public enum WindowsMemoryType : uint
    {
        /// <summary> Indicates that the memory pages within the region are private (that is, not shared by other processes). </summary>
        MemPrivate = 0x20000,

        /// <summary> Indicates that the memory pages within the region are mapped into the view of a section. </summary>
        MemMapped = 0x40000,

        /// <summary> Indicates that the memory pages within the region are mapped into the view of an image section. </summary>
        MemImage = 0x1000000,
    }
}