using System;
using System.Diagnostics;
using HoLLy.Memory.CrossPlatform;

namespace HoLLy.Memory.Linux
{
    public class LinuxMemoryRegion : MemoryRegion
    {
        public override ulong Start { get; }
        public override ulong End { get; }
        public LinuxMemoryPermissions Permissions { get; private set; }

        public ulong Offset { get; private set; }
        public (byte major, byte minor) Device { get; private set; }
        public ulong Inode { get; private set; }
        public string PathName { get; private set; }
        public bool IsSpecialRegion { get; private set; }

        public override bool IsReadable => Permissions.HasFlag(LinuxMemoryPermissions.Readable);
        public override bool IsWriteable => Permissions.HasFlag(LinuxMemoryPermissions.Writable);
        public override bool IsExecutable => Permissions.HasFlag(LinuxMemoryPermissions.Executable);
        public override bool IsMapped => PathName != null && !IsSpecialRegion;

        public LinuxMemoryRegion(ulong start, ulong end)
        {
            Start = start;
            End = end;
        }

        public static LinuxMemoryRegion ParseLine(string line)
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(split.Length >= 5);

            var startEnd = split[0].Split('-');
            Debug.Assert(startEnd.Length == 2);
            
            var reg = new LinuxMemoryRegion(Convert.ToUInt64(startEnd[0], 16),Convert.ToUInt64(startEnd[1], 16)) {
                Permissions = ParseFlags(split[1]),
            };

            // check if file
            if (split.Length > 5) {
                reg.Offset = Convert.ToUInt64(split[2], 16);

                var devSplit = split[3].Split(":");
                Debug.Assert(devSplit.Length == 2);
                reg.Device = (Convert.ToByte(devSplit[0], 16), Convert.ToByte(devSplit[1], 16));
                
                reg.Inode = Convert.ToUInt64(split[4]);

                var path = line[line.GetNthIndex(' ', 5)..].TrimStart(' ');
                reg.PathName = path;
                reg.IsSpecialRegion = path.StartsWith('[') && path.EndsWith('[');
            }

            return reg;
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