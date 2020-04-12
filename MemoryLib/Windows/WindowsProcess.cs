using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using HoLLy.Memory.CrossPlatform;
using static HoLLy.Memory.Windows.Native;
using static HoLLy.Memory.Windows.NativeHelper;

namespace HoLLy.Memory.Windows
{
    public class WindowsProcess : Process, IDisposable
    {
        public override uint Id { get; }
        public IntPtr Handle => handle ??= OpenProcess(accessFlags, 0, Id);
        private IntPtr? handle;
        private readonly ProcessAccessFlags accessFlags;
        
        // cached for GetModules
        private static IntPtr[] modulesBuffer;
        private const int StringBuilderCapacity = 1024;
        private readonly StringBuilder stringBuilder = new StringBuilder(StringBuilderCapacity);

        public WindowsProcess(uint pid, ProcessAccessFlags accessFlags = ProcessAccessFlags.All, bool lazy = false)
        {
            Id = pid;
            this.accessFlags = accessFlags;
            if (!lazy) {
                handle = OpenProcess(accessFlags, 0, pid);
            }
        }

        public override bool TryRead(ulong address, byte[] buffer, int length)
        {
            
            return ReadProcessMemory(Handle, Utils.UnsafeConvertToPointer(address), buffer, length, out _);
        }

        public override bool TryWrite(ulong address, byte[] buffer, int length)
        {
            return WriteProcessMemory(Handle, Utils.UnsafeConvertToPointer(address), buffer, length, out _);
        }

        public override IReadOnlyList<IMemoryRegion> GetMemoryRegions()
        {
            var list = new List<WindowsMemoryRegion>();
            const ulong maxSize = ulong.MaxValue;
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

                if (!m.State.HasFlag(WindowsMemoryState.MemFree)) {
                    list.Add(m);
                }

                if (address == m.BaseAddress + m.RegionSize)
                    break;
                address = m.BaseAddress + m.RegionSize;
            } while (address < maxSize);

            return list.AsReadOnly();
        }

        public override IReadOnlyList<IProcessModule> GetModules()
        {
            const int defaultArraySize = 256;
            modulesBuffer ??= new IntPtr[defaultArraySize];
            EnumProcessModulesEx(Handle, modulesBuffer, modulesBuffer.Length * IntPtr.Size, out int found, 3);

            // if our array was too small, try again with correct size
            if (found/IntPtr.Size > modulesBuffer.Length) {
                modulesBuffer = new IntPtr[found/IntPtr.Size];
                EnumProcessModulesEx(Handle, modulesBuffer, found, out found, 3);
            }

            return modulesBuffer.Take(found / IntPtr.Size).Select(hMod => {
                GetModuleFileNameEx(Handle, hMod, stringBuilder, StringBuilderCapacity);
                return new WindowsProcessModule(hMod, stringBuilder.ToString());
            }).ToList().AsReadOnly();
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