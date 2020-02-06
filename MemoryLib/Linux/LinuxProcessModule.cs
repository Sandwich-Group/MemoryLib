using HoLLy.Memory.CrossPlatform;

namespace HoLLy.Memory.Linux
{
    public class LinuxProcessModule : IProcessModule
    {
        public string FullPath { get; }
        public ulong BaseAddress { get; }

        public LinuxProcessModule(ulong baseAddress, string fullPath)
        {
            FullPath = fullPath;
            BaseAddress = baseAddress;
        }
    }
}