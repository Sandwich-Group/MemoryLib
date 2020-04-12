using System;

namespace HoLLy.Memory
{
    internal static class Utils
    {
        /// <remarks> https://stackoverflow.com/a/2571744 </remarks>
        public static int GetNthIndex(this string s, char t, int n)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++) {
                if (s[i] != t) continue;
                if (++count == n) {
                    return i;
                }
            }

            return -1;
        }

        public static UIntPtr UnsafeConvertToPointer(ulong input)
        {
            if (UIntPtr.Size == 4 && input > uint.MaxValue) {
                throw new Exception($"Tried to make pointer for address 0x{input:X} but source process is only 32-bit!");
            }
            
            return new UIntPtr(input);
        }
    }
}