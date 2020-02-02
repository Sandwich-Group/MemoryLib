using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using EntryPoint;
using HoLLy.Memory.Linux;

namespace TestApp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Cli.Execute<CliCommands>(args);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private class CliCommands : BaseCliCommands
        {
            [Command("regions")]
            [Help("Show the memory regions of a given process")]
            public void ShowRegions(string[] stringArgs)
            {
                var args = Cli.Parse<RegionsCommand>(stringArgs);
                LinuxProcess proc;
                if (args.ProcessId != null) {
                    proc = new LinuxProcess(args.ProcessId.Value);
                }
                else if (args.ProcessName != null) {
                    proc = LinuxProcess.GetProcessesByName(args.ProcessName).Single();
                }
                else {
                    throw new Exception("Please pass a PID with -p");
                }

                Console.WriteLine($"Listing regions for PID {proc.Id}.");
                foreach (var region in proc.GetMemoryRegions()) {
                    Console.WriteLine(region is LinuxMemoryRegion linuxReg && linuxReg.PathName != null
                        ? $"{region.Start:X}-{region.End:X} {region.PermissionString},\t{linuxReg.PathName} @{linuxReg.Offset:X}"
                        : $"{region.Start:X}-{region.End:X} {region.PermissionString}"
                    );
                }
            }

            [Command("dump")]
            [Help("Dumps a memory region of a given linux process")]
            public void DumpRegion(string[] stringArgs)
            {
                var args = Cli.Parse<DumpCommand>(stringArgs);
                LinuxProcess proc;
                if (args.ProcessId != null) {
                    proc = new LinuxProcess(args.ProcessId.Value);
                }
                else if (args.ProcessName != null) {
                    proc = LinuxProcess.GetProcessesByName(args.ProcessName).Single();
                }
                else {
                    throw new Exception("Please pass a PID with -p");
                }

                string regionName = args.RegionName ?? "[heap]";
                var region = proc.GetMemoryRegions().Single(x => (x as LinuxMemoryRegion)?.PathName?.Equals(regionName, StringComparison.OrdinalIgnoreCase) == true);
                Console.WriteLine($"Dumping '{regionName}' region ({region.Start:X}-{region.End:X}) to dump.dat for PID {proc.Id}.");

                byte[] buffer = new byte[0x1000];
                using var file = File.Open("dump.dat", FileMode.Create);
                for (ulong i = region.Start; i < region.End; i += (ulong)buffer.Length) {
                    int len = (int)Math.Min((ulong)buffer.Length, region.End - i);
                    var success = proc.TryRead(new UIntPtr(i), buffer, len);
                    if (!success) {
                        Console.WriteLine($"Read error: {Marshal.GetLastWin32Error()} - {Marshal.GetHRForLastWin32Error()}");
                    }
                    file.Write(buffer, 0, len);
                }
            }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class RegionsCommand : BaseCliArguments
        {
            public RegionsCommand() : base("Memory Regions") { }

            [OptionParameter("pid", 'p')]
            public uint? ProcessId { get; set; }

            [OptionParameter("name", 'n')]
            public string ProcessName { get; set; }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class DumpCommand : BaseCliArguments
        {
            public DumpCommand() : base("Dump Memory") { }

            [OptionParameter("pid", 'p')]
            public uint? ProcessId { get; set; }

            [OptionParameter("name", 'n')]
            public string ProcessName { get; set; }

            [OptionParameter("region", 'r')]
            public string RegionName { get; set; }
        }
    }
}