using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace HoLLy.Memory.Scanning
{
    public readonly struct PatternByte
    {
        public byte Val { get; }
        public bool Skip { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Match(byte b) => Skip || b == Val;

        private static PatternByte Normal(byte b) => new PatternByte(b, false);
        private static PatternByte Wildcard => new PatternByte(0, true);

        public PatternByte(byte val, bool skip)
        {
            Val = val;
            Skip = skip;
        }

        public static PatternByte[] Parse(string pattern)
        {
            // check pattern
            if (pattern.Split(' ').Any(a => a.Length % 2 != 0)) throw new Exception("Bad pattern");

            string withoutSpaces = pattern.Replace(" ", string.Empty);
            if (withoutSpaces.Length % 2 != 0) throw new Exception("Bad pattern");

            int byteCount = withoutSpaces.Length / 2;
            var arr = new PatternByte[byteCount];
            for (int i = 0; i < byteCount; i++)
                arr[i] = byte.TryParse(withoutSpaces.Substring(i * 2, 2), NumberStyles.None, CultureInfo.InvariantCulture, out byte b)
                    ? Normal(b)
                    : Wildcard;

            return arr;
        }

        public static uint FirstNonWildcardByte(PatternByte[] pattern)
        {
            uint firstNonWildcard = 0;
            for (uint i = 0; i < pattern.Length; ++i)
            {
                if (pattern[i].Skip) continue;
                firstNonWildcard = i;
                break;
            }

            return firstNonWildcard;
        }

        public override string ToString() => Skip ? "??" : Val.ToString("X2");
    }
}