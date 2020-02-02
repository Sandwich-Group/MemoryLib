using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace HoLLy.Memory.Linux
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class PInvokes
    {
        [DllImport("libc", SetLastError = true)]
        public static extern IntPtr process_vm_readv(int pid,
                                                     ref IoVec local_iov,
                                                     ulong liovcnt,
                                                     ref IoVec remote_iov,
                                                     ulong riovcnt,
                                                     ulong flags);

        [DllImport("libc", SetLastError = true)]
        public static extern IntPtr process_vm_writev(int pid,
                                                      ref IoVec local_iov,
                                                      ulong liovcnt,
                                                      ref IoVec remote_iov,
                                                      ulong riovcnt,
                                                      ulong flags);

        [StructLayout(LayoutKind.Sequential)]
        public struct IoVec
        {
            public UIntPtr iov_base;
            public int iov_len;

            public IoVec(UIntPtr iovBase, int iovLen)
            {
                iov_base = iovBase;
                iov_len = iovLen;
            }
        }
    }
}