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
    }
}