using System;
using System.Collections.Generic;
using System.Text;

namespace MixFileClass
{
    internal static class UnsafeClass
    {
        unsafe public static void* memcpy(void* dst, void* src, uint count)
        {
            System.Diagnostics.Debug.Assert(dst != null);
            System.Diagnostics.Debug.Assert(src != null);

            byte* dp = (byte*)dst;
            byte* sp = (byte*)src;

            while (count-- > 0)
            {
                *dp = *sp;
                dp++;
                sp++;
            }

            return dp;
        }

        unsafe public static void* memmove(void* dst, void* src, uint count)
        {
            System.Diagnostics.Debug.Assert(dst != null);
            System.Diagnostics.Debug.Assert(src != null);

            byte* dp = (byte*)dst;
            byte* sp = (byte*)src;

            if (dp <= sp || dp >= (sp + count))
            {
                while (count-- > 0)
                {
                    *dp = *sp;
                    dp++;
                    sp++;
                }
            }
            else
            {
                dp += count - 1;
                sp += count - 1;
                while (count-- > 0)
                {
                    *dp = *sp;
                    dp--;
                    sp--;
                }
            }

            return dp;
        }

        unsafe public static void* memset(void* s, int c, uint n)
        {
            byte* p = (byte*)s;

            while (n > 0)
            {
                *p++ = (byte)c;
                --n;
            }

            return s;
        }
    }
}
