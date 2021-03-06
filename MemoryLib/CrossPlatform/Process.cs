using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using HoLLy.Memory.Linux;
using HoLLy.Memory.Scanning;
using HoLLy.Memory.Windows;

namespace HoLLy.Memory.CrossPlatform
{
    public abstract class Process
    {
        // TODO: allow big endian?

        private readonly byte[] readBuffer = new byte[16];
        public abstract uint Id { get; }

        public static Process Open(uint pid)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return new WindowsProcess(pid);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return new LinuxProcess(pid);
            }

            throw new NotSupportedException("Current platform is not supported");
        }

        public static IReadOnlyList<Process> OpenAllByName(string name)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                return LinuxProcess.GetProcessesByName(name, false);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return System.Diagnostics.Process.GetProcessesByName(name).Select(x => new WindowsProcess((uint)x.Id, lazy: true)).ToList();
            }

            throw new NotSupportedException("Current platform is not supported");
        }

        public bool Is64Bit => is64Bit ??= CheckIs64Bit();
        private bool? is64Bit;
        protected abstract bool CheckIs64Bit();
        
        public abstract bool TryRead(ulong address, byte[] buffer, int length);
        public abstract bool TryWrite(ulong address, byte[] buffer, int length);
        public abstract IReadOnlyList<IMemoryRegion> GetMemoryRegions();
        public abstract IReadOnlyList<IProcessModule> GetModules();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(ulong address, byte[] buffer, int length)
        {
            if (!TryRead(address, buffer, length)) {
                throw new Exception($"Reading {length} bytes at address {address:X} failed");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ulong address, byte[] buffer, int length)
        {
            if (!TryWrite(address, buffer, length)) {
                throw new Exception($"Writing {length} bytes at address {address:X} failed");
            }
        }

        /// <summary> Read bytes into a new buffer. </summary>
        public byte[] ReadBytes(ulong address, int length)
        {
            var buffer = new byte[length];
            Read(address, buffer, length);
            return buffer;
        }

        public sbyte ReadS8(ulong address) => (sbyte)ReadIntoBuffer(address, 1)[0];
        public short ReadS16(ulong address) => BitConverter.ToInt16(ReadIntoBuffer(address, 2), 0);
        public int ReadS32(ulong address) => BitConverter.ToInt32(ReadIntoBuffer(address, 4), 0);
        public long ReadS64(ulong address) => BitConverter.ToInt64(ReadIntoBuffer(address, 8), 0);

        public byte ReadU8(ulong address) => ReadIntoBuffer(address, 1)[0];
        public ushort ReadU16(ulong address) => BitConverter.ToUInt16(ReadIntoBuffer(address, 2), 0);
        public uint ReadU32(ulong address) => BitConverter.ToUInt32(ReadIntoBuffer(address, 4), 0);
        public ulong ReadU64(ulong address) => BitConverter.ToUInt64(ReadIntoBuffer(address, 8), 0);

        public float ReadF32(ulong address) => BitConverter.ToSingle(ReadIntoBuffer(address, 4), 0);
        public double ReadF64(ulong address) => BitConverter.ToDouble(ReadIntoBuffer(address, 8), 0);
        
        // TODO: read null-terminated string
        /// <summary> Read a string with a specific length and encoding. Uses UTF-8 by default. </summary>
        public string ReadString(ulong address, int length, Encoding encoding = null)
        {
            return (encoding ?? Encoding.UTF8).GetString(length <= readBuffer.Length
                    ? ReadIntoBuffer(address, length)
                    : ReadBytes(address, length),
                0, length);
        }
        
        // TODO: read .NET string and .NET List<T>

        /// <summary> Read bytes into a reused buffer to prevents a heap allocation when reading primitives. </summary>
        private byte[] ReadIntoBuffer(ulong address, int length)
        {
            if (length > readBuffer.Length) {
                throw new IndexOutOfRangeException($"Tried to read {length} bytes into read buffer of size {readBuffer.Length}");
            }

            lock (readBuffer) {
                Read(address, readBuffer, length);
                return readBuffer;
            }
        }

        public void Write(ulong address, byte[] buffer)
        {
            Write(address, buffer, buffer.Length);
        }

        public void Write(ulong address, sbyte value) => Write(address, new[] {(byte)value});
        public void Write(ulong address, short value) => Write(address, BitConverter.GetBytes(value));
        public void Write(ulong address, int value) => Write(address, BitConverter.GetBytes(value));
        public void Write(ulong address, long value) => Write(address, BitConverter.GetBytes(value));

        public void Write(ulong address, byte value) => Write(address, new[] {value});
        public void Write(ulong address, ushort value) => Write(address, BitConverter.GetBytes(value));
        public void Write(ulong address, uint value) => Write(address, BitConverter.GetBytes(value));
        public void Write(ulong address, ulong value) => Write(address, BitConverter.GetBytes(value));

        public void Write(ulong address, float value) => Write(address, BitConverter.GetBytes(value));
        public void Write(ulong address, double value) => Write(address, BitConverter.GetBytes(value));

        /// <summary> Writes the bytes of a string without null terminator, using a given encoding. Uses UTF-8 by default. </summary>
        public void WriteString(ulong address, string value, Encoding encoding = null) => Write(address, (encoding ?? Encoding.UTF8).GetBytes(value));

        public bool Scan(PatternByte[] pattern, bool? mapped, out ulong result) => PatternScanner.Scan(this, pattern, mapped, out result);
        public bool Scan(PatternByte[] pattern, ulong baseAddress, ulong size, out ulong result) => PatternScanner.Scan(this, pattern, baseAddress, size, out result);
    }
}