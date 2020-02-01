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
    }
}