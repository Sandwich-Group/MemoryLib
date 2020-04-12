using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoLLy.Memory.CrossPlatform;
using static HoLLy.Memory.Linux.PInvokes;

namespace HoLLy.Memory.Linux
{
    public class LinuxProcess : Process
    {
        public override uint Id { get; }

        public LinuxProcess(uint pid)
        {
            Id = pid;

            if (!Directory.Exists(Path.Combine("/proc", pid.ToString()))) {
                throw new Exception($"Process with pid {pid} does not exist.");
            }
        }

        public override unsafe bool TryRead(ulong address, byte[] buffer, int length)
        {
            fixed (byte* ptr = buffer) {
                var localIo = new IoVec((ulong)ptr, length);
                var remoteIo = new IoVec(address, length);

                IntPtr res = process_vm_readv((int)Id, in localIo, 1, in remoteIo, 1, 0);
                return res.ToInt64() != -1;
            }
        }

        public override unsafe bool TryWrite(ulong address, byte[] buffer, int length)
        {
            fixed (byte* ptr = buffer) {
                var localIo = new IoVec((ulong)ptr, length);
                var remoteIo = new IoVec(address, length);

                IntPtr res = process_vm_writev((int)Id, in localIo, 1, in remoteIo, 1, 0);
                return res.ToInt64() != -1;
            }
        }

        public override IReadOnlyList<IMemoryRegion> GetMemoryRegions()
        {
            string path = Path.Combine("/proc", Id.ToString(), "maps");
            return File.ReadAllLines(path).Select(LinuxMemoryRegion.ParseLine).ToList().AsReadOnly();
        }

        public override IReadOnlyList<IProcessModule> GetModules()
        {
            return GetMemoryRegions()
                .Cast<LinuxMemoryRegion>()
                .GroupBy(x => x.PathName)
                .Where(grouping => !grouping.First().IsSpecialRegion && grouping.First().PathName != null)
                .Select(grouping => new LinuxProcessModule(grouping.Min(reg => reg.Start), grouping.Key))
                .ToList().AsReadOnly();
        }

        public static IReadOnlyList<LinuxProcess> GetProcessesByName(string name, bool caseSensitive = true)
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