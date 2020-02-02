namespace HoLLy.Memory.CrossPlatform
{
    public abstract class MemoryRegion
    {
        public abstract ulong Start { get; }
        public abstract ulong End { get; }
        
        public abstract bool IsReadable { get; }
        public abstract bool IsWriteable { get; }
        public abstract bool IsExecutable { get; }
        public abstract bool IsMapped { get; }
    }
}