using System;

namespace HoLLy.Memory.Linux
{
    [Flags]
    public enum LinuxMemoryPermissions
    {
        None = 0,
        Readable = 1, Writable, Executable, Shared, Private,
    }
}