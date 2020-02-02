using HoLLy.Memory.CrossPlatform;

namespace HoLLy.Memory.Windows
{
    public class WindowsMemoryRegion : MemoryRegion
    {
        public override ulong Start { get; }
        public override ulong End { get; }
        public ulong BaseAddress { get; }
        public ulong RegionSize { get; }
        public WindowsMemoryState State { get; }
        public WindowsAllocationProtect Protect { get; }
        public WindowsMemoryType Type { get; }

        public override bool IsReadable => Protect.HasFlag(WindowsAllocationProtect.PageReadonly) ||
            Protect.HasFlag(WindowsAllocationProtect.PageReadWrite) |
            Protect.HasFlag(WindowsAllocationProtect.PageExecuteRead) |
            Protect.HasFlag(WindowsAllocationProtect.PageExecuteReadWrite);

        public override bool IsWriteable => Protect.HasFlag(WindowsAllocationProtect.PageReadWrite)
            || Protect.HasFlag(WindowsAllocationProtect.PageWriteCombine)
            || Protect.HasFlag(WindowsAllocationProtect.PageWriteCopy)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteReadWrite)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteWriteCopy);
        public override bool IsExecutable => Protect.HasFlag(WindowsAllocationProtect.PageExecute)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteRead)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteReadWrite)
            || Protect.HasFlag(WindowsAllocationProtect.PageExecuteWriteCopy);

        public override bool IsMapped => Type.HasFlag(WindowsMemoryType.MemMapped) || Type.HasFlag(WindowsMemoryType.MemImage);

        internal override bool IsInUse => State.HasFlag(WindowsMemoryState.MemCommit);

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