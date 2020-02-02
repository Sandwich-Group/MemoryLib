using System;
using System.Diagnostics;

namespace HoLLy.Memory.Windows
{
    internal static class NativeHelper
    {
        public static bool Is64BitProcess() => IntPtr.Size == 8;
        public static bool Is64BitProcess(IntPtr handle) => Is64BitMachine() && !IsWow64Process(handle);
        public static bool Is64BitMachine() => Is64BitProcess() || IsWow64Process(Process.GetCurrentProcess().Handle);
        public static bool IsWow64Process(IntPtr handle) => Native.IsWow64Process(handle, out bool wow64) && wow64;
    }
}