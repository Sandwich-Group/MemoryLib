using System;
using System.Diagnostics.CodeAnalysis;
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
                /*
                else if (args.ProcessName != null) {
                    throw new NotImplementedException();
                }
                */
                else {
                    throw new Exception("Please pass a PID with -p");
                }

                Console.WriteLine($"Listing regions for PID {proc.Id}.");
                foreach (var region in proc.MemoryRegions) {
                    Console.WriteLine($"{region.Start:X}-{region.End:X} {region.Permissions}");
                }
            }
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class RegionsCommand : BaseCliArguments
        {
            public RegionsCommand() : base("Memory Regions") { }

            [OptionParameter("pid", 'p')]
            public uint? ProcessId { get; set; }

            /*
            [OptionParameter("name", 'n')]
            public string? ProcessName { get; set; }
            */
        }
    }
}