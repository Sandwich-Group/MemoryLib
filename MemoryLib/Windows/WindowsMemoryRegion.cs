namespace HoLLy.Memory.Windows
{
    public readonly struct WindowsMemoryRegion
    {
        public ulong BaseAddress { get; }
        public ulong RegionSize { get; }
        public WindowsMemoryState State { get; }
        public WindowsAllocationProtect Protect { get; }
        public WindowsMemoryType Type { get; }

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