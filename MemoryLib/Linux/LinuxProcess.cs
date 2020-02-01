using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HoLLy.Memory.Linux
{
    public class LinuxProcess
    {
        public uint Id { get; }
        public IReadOnlyList<LinuxMemoryRegion> MemoryRegions => memoryRegions?.AsReadOnly() ?? RefreshMemoryRegions();
        private List<LinuxMemoryRegion>? memoryRegions;

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