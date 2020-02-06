namespace HoLLy.Memory.CrossPlatform
{
    public interface IProcessModule
    {
        string FullPath { get; }
        ulong BaseAddress { get; }
    }
}