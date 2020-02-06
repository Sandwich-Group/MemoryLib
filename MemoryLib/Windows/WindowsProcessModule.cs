using System;
using HoLLy.Memory.CrossPlatform;

namespace HoLLy.Memory.Windows
{
    public class WindowsProcessModule : IProcessModule
    {
        public IntPtr Handle { get; }
        public string FullPath { get; }
        public ulong BaseAddress => (ulong)Handle.ToInt64(); // NOTE: could be double-checked using GetModuleInformation

        public WindowsProcessModule(IntPtr handle, string path)
        {
            Handle = handle;
            FullPath = path;
        }
    }
}