# MemoryLib
A cross-platform memory library. MacOS does not exist.

## Usage

### Cross-platform usage
```cs
using HoLLy.Memory.CrossPlatform;

// open a process
Process proc = Process.Open(0x123); // alternatively Process.OpenAllByName("process.exe").Single()

// find a byte pattern
PatternByte[] pattern = PatternByte.Parse("12 34 ?? FF AB");
if (!proc.Scan(pattern, false, out UIntPtr addr)) // second parameter filters mapped sections
    throw new Exception();

// read-write to memory
uint x = proc.ReadU32(addr);
proc.Write(addr, x * 2);
```
### Platform-specific usage
```cs
using HoLLy.Memory.Linux;

// open a process
Process proc = new LinuxProcess(0x123); // alternatively LinuxProcess.GetProcessesByName("process").Single()

// print all regions
foreach (var region in proc.GetMemoryRegions()) {
    Console.WriteLine(region is LinuxMemoryRegion linuxReg && linuxReg.PathName != null
        ? $"{region.Start:X}-{region.End:X} {region.PermissionString},\t{linuxReg.PathName} @{linuxReg.Offset:X}"
        : $"{region.Start:X}-{region.End:X} {region.PermissionString}"
    );
}
```

## Linux note
To read/write memory without root, add the CAP_SYS_PTRACE capability to the executable: `setcap cap_sys_ptrace+ep <executable>`
