using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HoLLy.Memory.Linux.PInvokes;

namespace HoLLy.Memory.Linux
{
    public class LinuxProcess
    {
        public uint Id { get; }
        public IReadOnlyList<LinuxMemoryRegion> MemoryRegions => memoryRegions?.AsReadOnly() ?? RefreshMemoryRegions();
        private List<LinuxMemoryRegion> memoryRegions;

        public LinuxProcess(uint pid)
        {
            Id = pid;

            if (!Directory.Exists(Path.Combine("/proc", pid.ToString()))) {
                throw new Exception($"Process with pid {pid} does not exist.");
            }
        }

        public IReadOnlyList<LinuxMemoryRegion> RefreshMemoryRegions()
        {
            memoryRegions ??= new List<LinuxMemoryRegion>();
            memoryRegions.Clear();

            string path = Path.Combine("/proc", Id.ToString(), "maps");
            memoryRegions.AddRange(File.ReadAllLines(path).Select(LinuxMemoryRegion.ParseLine));

            return memoryRegions.AsReadOnly();
        }

        public unsafe bool Read(UIntPtr address, byte[] buffer, int length)
        {
            fixed (byte* ptr = buffer) {
                var localIo = new IoVec((UIntPtr)ptr, length);
                var remoteIo = new IoVec(address, length);

                IntPtr res = process_vm_readv((int)Id, in localIo, 1, in remoteIo, 1, 0);
                return res.ToInt64() != -1;
            }
        }

        public unsafe bool Write(UIntPtr address, byte[] buffer, int length)
        {
            fixed (byte* ptr = buffer) {
                var localIo = new IoVec((UIntPtr)ptr, length);
                var remoteIo = new IoVec(address, length);

                IntPtr res = process_vm_writev((int)Id, in localIo, 1, in remoteIo, 1, 0);
                return res.ToInt64() != -1;
            }
        }

        public static IReadOnlyList<LinuxProcess> GetProcessesById(string name, bool caseSensitive = true)
        {
            return Directory.GetDirectories("/proc")
                .Select(p => {
                    var folderName = p.Substring("/proc/".Length);
                    if (!uint.TryParse(folderName, out uint pid)) return 0u;

                    string procName = File.ReadAllText(Path.Combine(p, "comm")).TrimEnd('\n');
                    if (caseSensitive && procName == name) return pid;
                    if (!caseSensitive && procName.Equals(name, StringComparison.OrdinalIgnoreCase)) return pid;

                    return 0u;
                })
                .Where(x => x != 0)
                .Select(pid => new LinuxProcess(pid))
                .ToList();
        }
    }
}