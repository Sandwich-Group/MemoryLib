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

        /// <summary>
        /// Should this region be scanned using a pattern scanner?
        /// </summary>
        internal virtual bool IsInUse => true;

        public string PermissionString => (IsReadable ? "R" : "") + (IsWriteable ? "W" : "") + (IsExecutable ? "X" : "");
    }
}