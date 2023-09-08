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

            void* ret = dst;

            while (count-- > 0)
            {
                *(char*)dst = *(char*)src;
                dst = (char*)dst + 1;
                src = (char*)src + 1;
            }

            return (ret);
        }

        unsafe public static void* memmove(void* dst, void* src, uint count)
        {
            System.Diagnostics.Debug.Assert(dst != null);
            System.Diagnostics.Debug.Assert(src != null);

            void* ret = dst;

            if (dst <= src || (char*)dst >= ((char*)src + count))
            {
                while (count-- > 0)
                {
                    *(char*)dst = *(char*)src;
                    dst = (char*)dst + 1;
                    src = (char*)src + 1;
                }
            }
            else
            {
                dst = (char*)dst + count - 1;
                src = (char*)src + count - 1;
                while (count-- > 0)
                {
                    *(char*)dst = *(char*)src;
                    dst = (char*)dst - 1;
                    src = (char*)src - 1;
                }
            }

            return (ret);
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
