using System;

namespace HoLLy.Memory.Linux
{
    [Flags]
    public enum LinuxMemoryPermissions
    {
        None = 0,
        Readable = 1,
        Writable = 2,
        Executable = 4,
        Shared = 8,
        Private = 16,
    }
}