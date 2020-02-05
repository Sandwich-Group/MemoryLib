using HoLLy.Memory.CrossPlatform;

namespace HoLLy.Memory.Windows
{
    public class WindowsMemoryRegion : IMemoryRegion
    {
        public ulong Start { get; }
        public ulong End { get; }
        public ulong BaseAddress { get; }
        public ulong RegionSize { get; }
        public WindowsMemoryState State { get; }
        public WindowsAllocationProtect Protect { get; }
        public WindowsMemoryType Type { get; }
        public bool IsInUse => !State.HasFlag(WindowsMemoryState.MemFree);

        public bool IsReadable => Protect.HasFlag(WindowsAllocationProtect.PageReadonly) ||
            Protect.HasFlag(WindowsAllocationProtect.PageReadWrite) |
            Protect.HasFlag(WindowsAllocationProtect.PageExecuteRead) |
            Protect.HasFlag(WindowsAllocationProtect.PageExecuteReadWrite);

        public bool IsWriteable => Protect.HasFlag(WindowsAllocationProtect.PageReadWrite)
            || Protect.HasFlag(WindowsAllocationProtect.PageWriteCombine)
            || Protect.HasFlag(WindowsAllocationProtect.PageWriteCopy)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteReadWrite)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteWriteCopy);
        public bool IsExecutable => Protect.HasFlag(WindowsAllocationProtect.PageExecute)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteRead)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteReadWrite)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteWriteCopy);

        public bool IsMapped => Type.HasFlag(WindowsMemoryType.MemMapped) || Type.HasFlag(WindowsMemoryType.MemImage);

        internal WindowsMemoryRegion(Native.MemoryBasicInformation32 m)
        {
            BaseAddress = m.BaseAddress;
            RegionSize = m.RegionSize;
            State = m.State;
            Protect = m.Protect;
            Type = m.Type;
        }

        internal WindowsMemoryRegion(Native.MemoryBasicInformation64 m)
        {
            BaseAddress = m.BaseAddress;
            RegionSize = m.RegionSize;
            State = m.State;
            Protect = m.Protect;
            Type = m.Type;
        }
    }
}