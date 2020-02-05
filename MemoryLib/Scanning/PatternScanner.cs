using System;
using System.Linq;
using HoLLy.Memory.CrossPlatform;

namespace HoLLy.Memory.Scanning
{
    public static class PatternScanner
    {
        private const int ScanStep = 0x1000;

        /// <summary>
        /// Scans a memory section for a pattern, only looking in valid memory regions.
        /// </summary>
        /// <param name="proc">A process to scan</param>
        /// <param name="pattern">The pattern array to scan for</param>
        /// <param name="mapped">Optionally check if region is mapped or not</param>
        /// <param name="result">The address (on success)</param>
        /// <returns>Whether the scan was successful or not</returns>
        public static bool Scan(Process proc, PatternByte[] pattern, bool? mapped, out UIntPtr result)
        {
            result = default;

            foreach (var region in proc.GetMemoryRegions().Where(x => !mapped.HasValue || x.IsMapped == mapped.Value)) {
                if (Scan(proc, pattern, new UIntPtr(region.Start), (uint)(region.End - region.Start), out result))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Scans a memory region for a pattern.
        /// </summary>
        /// <param name="proc">A process to scan</param>
        /// <param name="pattern">The pattern array to scan for</param>
        /// <param name="baseAddress">The address to start at</param>
        /// <param name="size">The size of the region to scan</param>
        /// <param name="result">The address (on success)</param>
        /// <returns>Whether the scan was successful or not</returns>
        public static bool Scan(Process proc, PatternByte[] pattern, UIntPtr baseAddress, ulong size, out UIntPtr result)
        {
            result = default;

            uint step = (uint)Math.Min(size, ScanStep);
            ulong min = baseAddress.ToUInt64();
            ulong max = min + size;
            byte[] buffer = new byte[step + pattern.Length - 1];

            // skip wildcards, since they would always match
            uint firstNonWildcard = PatternByte.FirstNonWildcardByte(pattern);

            for (ulong i = min; i < max; i += step) {
                // read buffer
                // TODO: limit to not go outside region?
                proc.Read(new UIntPtr(i), buffer, buffer.Length);

                // loop through buffer
                for (uint j = 0; j < step; ++j) {
                    bool match = true;

                    // loop through pattern
                    for (uint k = firstNonWildcard; k < pattern.Length; ++k) {
                        if (pattern[k].Match(buffer[j + k])) continue;
                        match = false;
                        break;
                    }

                    if (match) {
                        result = (UIntPtr)(i + j);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}