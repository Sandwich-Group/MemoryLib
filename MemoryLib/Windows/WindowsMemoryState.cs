using System;

namespace HoLLy.Memory.Windows
{
    [Flags]
    public enum WindowsMemoryState : uint
    {
        /// <summary>
        /// Indicates committed pages for which physical storage has been allocated, either in memory or in the
        /// paging file on disk.
        /// </summary>
        MemCommit = 0x1000,

        /// <summary>
        /// Indicates reserved pages where a range of the process's virtual address space is reserved without any
        /// physical storage being allocated. For reserved pages, the information in the
        /// <see cref="Native.MemoryBasicInformation32.Protect"/> member is undefined.
        /// </summary>
        MemReserved = 0x2000,

        /// <summary>
        /// Indicates free pages not accessible to the calling process and available to be allocated. For free
        /// pages, the information in the <see cref="Native.MemoryBasicInformation32.AllocationBase"/>,
        /// <see cref="WindowsAllocationProtect"/>, <see cref="Native.MemoryBasicInformation32.Protect"/>,
        /// and <see cref="Type"/> members is undefined.
        /// </summary>
        MemFree = 0x10000,
    }
}