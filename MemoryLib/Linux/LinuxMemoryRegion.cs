using System;
using System.Diagnostics;

namespace HoLLy.Memory.Linux
{
    public struct LinuxMemoryRegion
    {
        public ulong Start { get; private set; }
        public ulong End { get; private set; }
        public LinuxMemoryPermissions Permissions { get; private set; }

        // TODO: could include info about mapped files

        public static LinuxMemoryRegion ParseLine(string line)
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(split.Length >= 5);

            var startEnd = split[0].Split('-');
            Debug.Assert(startEnd.Length == 2);

            return new LinuxMemoryRegion {
                Start = Convert.ToUInt64(startEnd[0], 16),
                End = Convert.ToUInt64(startEnd[1], 16),
                Permissions = ParseFlags(split[1]),
            };
        }

        private static LinuxMemoryPermissions ParseFlags(string line)
        {
            var flags = LinuxMemoryPermissions.None;

            if (line.Contains("r")) flags |= LinuxMemoryPermissions.Readable;
            if (line.Contains("w")) flags |= LinuxMemoryPermissions.Writable;
            if (line.Contains("x")) flags |= LinuxMemoryPermissions.Executable;
            if (line.Contains("s")) flags |= LinuxMemoryPermissions.Shared;
            if (line.Contains("p")) flags |= LinuxMemoryPermissions.Private;

            return flags;
        }
    }
}