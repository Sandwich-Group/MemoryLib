using System;

namespace HoLLy.Memory.Windows
{
    [Flags]
    public enum ProcessAccessFlags : uint
    {
        Terminate = 0x0001,
        CreateThread = 0x0002,
        VirtualMemoryOperation = 0x0008,
        VirtualMemoryRead = 0x0010,
        VirtualMemoryWrite = 0x0020,
        DuplicateHandle = 0x0040,
        CreateProcess = 0x0080,
        SetQuota = 0x0100,
        SetInformation = 0x0200,
        QueryInformation = 0x0400,
        QueryLimitedInformation = 0x1000,

        Synchronize = 0x0010_0000,
        All = Terminate | CreateThread | 0x0004 | VirtualMemoryOperation
            | VirtualMemoryRead | VirtualMemoryWrite | DuplicateHandle | CreateProcess
            | SetQuota | SetInformation | QueryInformation | 0x0800
            | Synchronize | 0x000F_0000
    }
}