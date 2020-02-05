namespace HoLLy.Memory.CrossPlatform
{
    public interface IMemoryRegion
    {
        ulong Start { get; }
        ulong End { get; }
        
        bool IsReadable { get; }
        bool IsWriteable { get; }
        bool IsExecutable { get; }
        bool IsMapped { get; }

        public string PermissionString => (IsReadable ? "R" : "") + (IsWriteable ? "W" : "") + (IsExecutable ? "X" : "");
    }
}