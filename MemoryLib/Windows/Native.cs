using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace HoLLy.Memory.Windows
{
    internal static class Native
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out UIntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, UIntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, int bInheritHandle, uint dwProcessId);
        
        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern int VirtualQueryEx32(IntPtr hProcess, UIntPtr lpAddress, out MemoryBasicInformation32 lpBuffer, uint dwLength);

        [DllImport("kernel32.dll", EntryPoint = "VirtualQueryEx")]
        public static extern int VirtualQueryEx64(IntPtr hProcess, UIntPtr lpAddress, out MemoryBasicInformation64 lpBuffer, uint dwLength);

        [DllImport("psapi.dll")]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, [Out] IntPtr[] lphModule, int sz, out int one, int two);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName,  int nSize);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        public static extern bool IsWow64Process(IntPtr processHandle, out bool wow64Process);

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct MemoryBasicInformation32
        {
            public readonly uint BaseAddress;
            public readonly uint AllocationBase;
            /// <summary>
            /// The memory protection option when the region was initially allocated. This member is 0 if the caller
            /// does not have access.
            /// </summary>
            public readonly WindowsAllocationProtect AllocationProtect;
            public readonly uint RegionSize;
            /// <summary> The state of the pages in the region. </summary>
            public readonly WindowsMemoryState State;
            /// <summary> The access protection of the pages in the region. </summary>
            public readonly WindowsAllocationProtect Protect;
            /// <summary> The type of pages in the region. </summary>
            public readonly WindowsMemoryType Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct MemoryBasicInformation64
        {
            public readonly ulong BaseAddress;
            public readonly ulong AllocationBase;
            /// <summary>
            /// The memory protection option when the region was initially allocated. This member is 0 if the caller
            /// does not have access.
            /// </summary>
            public readonly WindowsAllocationProtect AllocationProtect;
            private readonly int alignment1;
            public readonly ulong RegionSize;
            /// <summary> The state of the pages in the region. </summary>
            public readonly WindowsMemoryState State;
            /// <summary> The access protection of the pages in the region. </summary>
            public readonly WindowsAllocationProtect Protect;
            /// <summary> The type of pages in the region. </summary>
            public readonly WindowsMemoryType Type;
            private readonly int alignment2;
        }
    }
}