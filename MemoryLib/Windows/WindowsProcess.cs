using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HoLLy.Memory.CrossPlatform;
using static HoLLy.Memory.Windows.Native;
using static HoLLy.Memory.Windows.NativeHelper;

namespace HoLLy.Memory.Windows
{
    public class WindowsProcess : Process, IDisposable
    {
        public uint Id { get; }
        public IntPtr Handle => handle ??= OpenProcess(accessFlags, 0, Id);
        private IntPtr? handle;
        private readonly ProcessAccessFlags accessFlags;

        public WindowsProcess(uint pid, ProcessAccessFlags accessFlags = ProcessAccessFlags.All, bool lazy = false)
        {
            Id = pid;
            this.accessFlags = accessFlags;
            if (!lazy) {
                handle = OpenProcess(accessFlags, 0, pid);
            }
        }

        public override bool TryRead(UIntPtr address, byte[] buffer, int length)
        {
            return ReadProcessMemory(Handle, address, buffer, length, out _);
        }

        public override bool TryWrite(UIntPtr address, byte[] buffer, int length)
        {
            return WriteProcessMemory(Handle, address, buffer, length, out _);
        }

        public IEnumerable<WindowsMemoryRegion> EnumerateRegions(ulong maxSize = ulong.MaxValue)
        {
            ulong address = 0;
            bool x64 = Is64BitProcess(Handle);
            do
            {
                WindowsMemoryRegion m;
                // ignoring return values
                if (x64) {
                    VirtualQueryEx64(Handle, (UIntPtr)address, out MemoryBasicInformation64 m64, (uint)Marshal.SizeOf(typeof(MemoryBasicInformation64)));
                    m = new WindowsMemoryRegion(m64);
                } else {
                    VirtualQueryEx32(Handle, (UIntPtr)address, out MemoryBasicInformation32 m32, (uint)Marshal.SizeOf(typeof(MemoryBasicInformation32)));
                    m = new WindowsMemoryRegion(m32);
                }
                yield return m;
                if (address == m.BaseAddress + m.RegionSize)
                    break;
                address = m.BaseAddress + m.RegionSize;
            } while (address <= maxSize);
        }

        #region Dispose implementation
        private void ReleaseUnmanagedResources()
        {
            CloseHandle(Handle);
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~WindowsProcess() {
            ReleaseUnmanagedResources();
        }
        #endregion
    }
}