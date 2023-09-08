using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using word = System.UInt16;
using dword = System.UInt32;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MixFileClass
{
    internal class WSkeyCal
    {
        const string pubkey_str = "AihRvNoIbTn85FZRYNZRcT+i6KpU+maCsEqr3Q5q+LDB5tH7Tz2qQ38V";

        static sbyte[] char2num = new sbyte[]{
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 62, -1, -1, -1, 63,
            52, 53, 54, 55, 56, 57, 58, 59, 60, 61, -1, -1, -1, -1, -1, -1,
            -1,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,
            15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, -1, -1, -1, -1, -1,
            -1, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,
            41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1};

        class pubkey
        {
            public static dword[] key1 = new dword[64];
            public static dword[] key2 = new dword[64];
            public static dword len;
        }

        dword[] glob1 = new dword[64];
        dword glob1_bitlen, glob1_len_x2;
        dword[] glob2 = new dword[130];
        dword[] glob1_hi = new dword[4], glob1_hi_inv = new dword[4];
        dword glob1_hi_bitlen;
        dword glob1_hi_inv_lo, glob1_hi_inv_hi;

        bool public_key_initialized = false;

        unsafe void init_bignum(dword* n, dword val, dword len)
        {
            UnsafeClass.memset((void*)n, 0, len * 4);
            n[0] = val;
        }

        void init_bignum(dword[] n, dword val, dword len)
        {
            Array.Clear(n, 0, (int)len);
            n[0] = val;
        }

        unsafe void move_key_to_big(dword* n, byte* key, dword klen, dword blen)
        {
            dword sign;
            int i;

            if ((key[0] & 0x80) != 0) sign = 0xff;
            else sign = 0;

            for (i = (int)blen * 4; i > klen; i--)
                ((byte*)n)[i - 1] = (byte)sign;
            for (; i > 0; i--)
                ((byte*)n)[i - 1] = key[klen - i];
        }

        unsafe void key_to_bignum(dword* n, byte* key, dword len)
        {
            dword keylen;
            int i;

            if (key[0] != 2) return;
            key++;

            if ((key[0] & 0x80) != 0)
            {
                keylen = 0;
                for (i = 0; i < (key[0] & 0x7f); i++) keylen = (keylen << 8) | key[i + 1];
                key += (key[0] & 0x7f) + 1;
            }
            else
            {
                keylen = key[0];
                key++;
            }
            if (keylen <= len * 4)
                move_key_to_big(n, key, keylen, len);
        }

        unsafe dword len_bignum(dword* n, dword len)
        {
            int i;
            i = (int)len - 1;
            while ((i >= 0) && (n[i] == 0)) i--;
            return (dword)(i + 1);
        }

        unsafe dword bitlen_bignum(dword* n, dword len)
        {
            dword ddlen, bitlen, mask;
            ddlen = len_bignum(n, len);
            if (ddlen == 0) return 0;
            bitlen = ddlen * 32;
            mask = 0x80000000;
            while ((mask & n[ddlen - 1]) == 0)
            {
                mask >>= 1;
                bitlen--;
            }
            return bitlen;
        }

        unsafe void init_pubkey()
        {
            int i;
            dword i2, tmp;
            byte[] keytmp = new byte[256];

            init_bignum(pubkey.key2, 0x10001, 64);

            i = 0;
            i2 = 0;
            while (i < pubkey_str.Length)
            {
                tmp = (byte)char2num[pubkey_str[i++]];
                tmp <<= 6; tmp |= (byte)char2num[pubkey_str[i++]];
                tmp <<= 6; tmp |= (byte)char2num[pubkey_str[i++]];
                tmp <<= 6; tmp |= (byte)char2num[pubkey_str[i++]];
                keytmp[i2++] = (byte)((tmp >> 16) & 0xff);
                keytmp[i2++] = (byte)((tmp >> 8) & 0xff);
                keytmp[i2++] = (byte)(tmp & 0xff);
            }
            fixed (dword* key1 = pubkey.key1)
            {
                fixed (byte* keyt = keytmp)
                {
                    key_to_bignum(key1, keyt, 64);
                }
                pubkey.len = bitlen_bignum(key1, 64) - 1;
            }
        }

        dword len_predata()
        {
            dword a = (pubkey.len - 1) / 8;
            return (55 / a + 1) * (a + 1);
        }

        unsafe int cmp_bignum(dword* n1, dword* n2, dword len)
        {
            n1 += len - 1;
            n2 += len - 1;
            while (len > 0)
            {
                if (*n1 < *n2) return -1;
                if (*n1 > *n2) return 1;
                n1--;
                n2--;
                len--;
            }
            return 0;
        }

        unsafe void mov_bignum(dword* dest, dword* src, dword len)
        {
            UnsafeClass.memmove(dest, src, len * 4);
        }

        void mov_bignum(dword[] dest, dword[] src, dword len)
        {
            Array.Copy(src, 0, dest, 0, len);
        }

        void mov_bignum(dword[] dest, dword i_dest, dword[] src, dword i_src, dword len)
        {
            Array.Copy(src, i_src, dest, i_dest, len);
        }

        unsafe void shr_bignum(dword* n, dword bits, int len)
        {
            dword i, i2;

            i2 = bits / 32;
            if (i2 > 0)
            {
                for (i = 0; i < len - i2; i++) n[i] = n[i + i2];
                for (; i < len; i++) n[i] = 0;
                bits = bits % 32;
            }
            if (bits == 0) return;
            for (i = 0; i < len - 1; i++) n[i] = (n[i] >> (int)bits) | (n[i + 1] << (int)(32 - bits));
            n[i] = n[i] >> (int)bits;
        }

        unsafe void shl_bignum(dword* n, dword bits, dword len)
        {
            dword i, i2;

            i2 = bits / 32;
            if (i2 > 0)
            {
                for (i = len - 1; i > i2; i--) n[i] = n[i - i2];
                for (; i > 0; i--) n[i] = 0;
                bits = bits % 32;
            }
            if (bits == 0) return;
            for (i = len - 1; i > 0; i--) n[i] = (n[i] << (int)bits) | (n[i - 1] >> (int)(32 - bits));
            n[0] <<= (int)bits;
        }

        unsafe dword sub_bignum(dword* dest, dword* src1, dword* src2, dword carry, dword len)
        {
            dword i1, i2;

            int ilen = (int)len + (int)len;
            while (--ilen != -1)
            {
                i1 = *(word*)src1;
                i2 = *(word*)src2;
                *(word*)dest = (word)(i1 - i2 - carry);
                src1 = (dword*)(((word*)src1) + 1);
                src2 = (dword*)(((word*)src2) + 1);
                dest = (dword*)(((word*)dest) + 1);
                if (((i1 - i2 - carry) & 0x10000) != 0) carry = 1; else carry = 0;
            }
            return carry;
        }

        unsafe void inv_bignum(dword* n1, dword* n2, dword len)
        {
            dword* n_tmp = stackalloc dword[64];
            dword n2_bytelen, bit;
            int n2_bitlen;

            init_bignum(n_tmp, 0, len);
            init_bignum(n1, 0, len);
            n2_bitlen = (int)bitlen_bignum(n2, len);
            bit = ((dword)1) << (n2_bitlen % 32);
            n1 += ((n2_bitlen + 32) / 32) - 1;
            n2_bytelen = ((dword)(n2_bitlen - 1) / 32) * 4;
            n_tmp[n2_bytelen / 4] |= ((dword)1) << ((n2_bitlen - 1) & 0x1f);

            while (n2_bitlen > 0)
            {
                n2_bitlen--;
                shl_bignum(n_tmp, 1, len);
                if (cmp_bignum(n_tmp, n2, len) != -1)
                {
                    sub_bignum(n_tmp, n_tmp, n2, 0, len);
                    *n1 |= bit;
                }
                bit >>= 1;
                if (bit == 0)
                {
                    n1--;
                    bit = 0x80000000;
                }
            }
            init_bignum(n_tmp, 0, len);
        }

        unsafe void inc_bignum(dword* n, dword len)
        {
            while ((++*n == 0) && (--len > 0)) n++;
        }

        unsafe void init_two_dw(dword* n, dword len)
        {
            fixed(dword* glob1_p = glob1)
            {
                fixed (dword* glob1_hi_p = glob1_hi)
                {
                    fixed (dword* glob1_hi_inv_p = glob1_hi_inv)
                    {
                        mov_bignum(glob1_p, n, len);
                        glob1_bitlen = bitlen_bignum(glob1_p, len);
                        glob1_len_x2 = (glob1_bitlen + 15) / 16;
                        mov_bignum(glob1_hi_p, glob1_p + len_bignum(glob1_p, len) - 2, 2);
                        glob1_hi_bitlen = bitlen_bignum(glob1_hi_p, 2) - 32;
                        shr_bignum(glob1_hi_p, glob1_hi_bitlen, 2);
                        inv_bignum(glob1_hi_inv_p, glob1_hi_p, 2);
                        shr_bignum(glob1_hi_inv_p, 1, 2);
                        glob1_hi_bitlen = (glob1_hi_bitlen + 15) % 16 + 1;
                        inc_bignum(glob1_hi_inv_p, 2);
                        if (bitlen_bignum(glob1_hi_inv_p, 2) > 32)
                        {
                            shr_bignum(glob1_hi_inv_p, 1, 2);
                            glob1_hi_bitlen--;
                        }
                        glob1_hi_inv_lo = *(word*)glob1_hi_inv_p;
                        glob1_hi_inv_hi = *(((word*)glob1_hi_inv_p) + 1);
                    }
                }
            }
        }

        unsafe void mul_bignum_word(dword* n1, dword* n2, dword mul, dword len)
        {
            dword i, tmp;

            tmp = 0;
            for (i = 0; i < len; i++)
            {
                tmp = mul * (*(word*)n2) + *(word*)n1 + tmp;
                *(word*)n1 = (word)tmp;
                n1 = (dword*)(((word*)n1) + 1);
                n2 = (dword*)(((word*)n2) + 1);
                tmp >>= 16;
            }
            *(word*)n1 += (word)tmp;
        }

        unsafe void mul_bignum(dword* dest, dword* src1, dword* src2, dword len)
        {
            dword i;

            init_bignum(dest, 0, len * 2);
            for (i = 0; i < len * 2; i++)
            {
                mul_bignum_word(dest, src1, *(word*)src2, len * 2);
                src2 = (dword*)(((word*)src2) + 1);
                dest = (dword*)(((word*)dest) + 1);
            }
        }

        unsafe void not_bignum(dword* n, dword len)
        {
            dword i;
            for (i = 0; i < len; i++) *(n) = ~*n++;
        }

        unsafe void neg_bignum(dword* n, dword len)
        {
            not_bignum(n, len);
            inc_bignum(n, len);
        }

        word get_word(dword[]n, dword halfoffset)
        {
            if ((halfoffset & 0x01) == 0)
            {
                return (word)(n[halfoffset / 2] & 0xffff);
            }
            else
            {
                return (word)(n[halfoffset / 2] >> 16);
            }
        }

        void set_word(dword[] n, dword halfoffset, word value)
        {
            if ((halfoffset & 0x01) == 0)
            {
                n[halfoffset / 2] = (n[halfoffset / 2] & 0xffff0000) | value;
            }
            else
            {
                n[halfoffset / 2] = (n[halfoffset / 2] & 0xffff) | ((dword)value << 16);
            }
        }

        void set_word(dword[] n, dword halfoffset, dword value)
        {
            if ((halfoffset & 0x01) == 0)
            {
                n[halfoffset / 2] = (n[halfoffset / 2] & 0xffff0000) | (value & 0xffff);
            }
            else
            {
                n[halfoffset / 2] = (n[halfoffset / 2] & 0xffff) | (value << 16);
            }
        }

        unsafe dword get_mulword(dword* n)
        {
            dword i;
            word* wn;

            wn = (word*)n;
            i = (dword)((((((((((*(wn - 1) ^ 0xffff) & 0xffff) * glob1_hi_inv_lo + 0x10000) >> 1)
                + (((*(wn - 2) ^ 0xffff) * glob1_hi_inv_hi + glob1_hi_inv_hi) >> 1) + 1)
                >> 16) + ((((*(wn - 1) ^ 0xffff) & 0xffff) * glob1_hi_inv_hi) >> 1) +
                (((*wn ^ 0xffff) * glob1_hi_inv_lo) >> 1) + 1) >> 14) + glob1_hi_inv_hi
                * (*wn ^ 0xffff) * 2) >> (int)glob1_hi_bitlen);
            if (i > 0xffff) i = 0xffff;
            return i & 0xffff;
        }

        unsafe void dec_bignum(dword* n, dword len)
        {
            while ((--*n == 0xffffffff) && (--len > 0))
                n++;
        }

        unsafe void calc_a_bignum(dword* n1, dword* n2, dword* n3, dword len)
        {
            dword g2_len_x2, len_diff;
            word tmp;

            fixed (dword* glob2_p = glob2)
            {
                fixed (dword* glob1_p = glob1)
                {
                    word* esi, edi;

                    mul_bignum(glob2_p, n2, n3, len);

                    glob2_p[len * 2] = 0;
                    g2_len_x2 = len_bignum(glob2_p, len * 2 + 1) * 2;

                    if (g2_len_x2 >= glob1_len_x2)
                    {
                        inc_bignum(glob2_p, len * 2 + 1);
                        neg_bignum(glob2_p, len * 2 + 1);

                        len_diff = g2_len_x2 + 1 - glob1_len_x2;
                        esi = ((word*)glob2_p) + (1 + g2_len_x2 - glob1_len_x2);
                        edi = ((word*)glob2_p) + (g2_len_x2 + 1);
                        for (; len_diff != 0; len_diff--)
                        {
                            edi--;
                            tmp = (word)get_mulword((dword*)edi);
                            esi--;
                            if (tmp > 0)
                            {
                                mul_bignum_word((dword*)esi, glob1_p, tmp, 2 * len);
                                if ((*edi & 0x8000) == 0)
                                {
                                    if (sub_bignum((dword*)esi, (dword*)esi, glob1_p, 0, len) != 0) (*edi)--;
                                }
                            }
                        }
                        neg_bignum(glob2_p, len);
                        dec_bignum(glob2_p, len);
                    }
                }
                mov_bignum(n1, glob2_p, len);
            }
        }

        void clear_tmp_vars(dword len)
        {
            init_bignum(glob1, 0, len);
            init_bignum(glob2, 0, len);
            init_bignum(glob1_hi_inv, 0, 4);
            init_bignum(glob1_hi, 0, 4);
            glob1_bitlen = 0;
            glob1_hi_bitlen = 0;
            glob1_len_x2 = 0;
            glob1_hi_inv_lo = 0;
            glob1_hi_inv_hi = 0;
        }

        unsafe void calc_a_key(dword* n1, dword* n2, dword* n3, dword* n4, dword len)
        {
            dword* n_tmp = stackalloc dword[64];
            dword n3_len, n4_len, n3_bitlen, bit_mask;

            init_bignum(n1, 1, len);
            n4_len = len_bignum(n4, len);
            init_two_dw(n4, n4_len);
            n3_bitlen = bitlen_bignum(n3, n4_len);
            n3_len = (n3_bitlen + 31) / 32;
            bit_mask = (((dword)1) << (int)((n3_bitlen - 1) % 32)) >> 1;
            n3 += n3_len - 1;
            int n3_bl_i = (int)n3_bitlen - 1;
            mov_bignum(n1, n2, n4_len);
            while (--n3_bl_i != -1)
            {
                if (bit_mask == 0)
                {
                    bit_mask = 0x80000000;
                    n3--;
                }

                calc_a_bignum(n_tmp, n1, n1, n4_len);

                if ((*n3 & bit_mask) != 0)
                    calc_a_bignum(n1, n_tmp, n2, n4_len);
                else
                    mov_bignum(n1, n_tmp, n4_len);

                bit_mask >>= 1;
            }
            init_bignum(n_tmp, 0, n4_len);
            clear_tmp_vars(len);
        }

        void memmove(dword[] dest, byte[] src, dword len, dword i_dest, dword i_src)
        {
            dword i;
            for (i = 0; i < len; )
            {
                dest[i_dest + i / 4] = ((dword)src[i_src + i] | 
                                       (i + 1 < len ? ((dword)src[i_src + i + 1] << 8) : 0) | 
                                       (i + 2 < len ? ((dword)src[i_src + i + 2] << 16) : 0) | 
                                       (i + 3 < len ? ((dword)src[i_src + i + 3] << 24) : 0));
                i += 4;
            }
        }

        void memmove(byte[] dest, dword[] src, dword len, dword i_dest, dword i_src)
        {
            dword i;
            for (i = 0; i < len;)
            {
                dword sv = src[i_src + i / 4];
                dest[i_dest + i] = (byte)(sv & 0xFF);
                if (i + 1 < len)
                    dest[i_dest + i + 1] = (byte)(sv & 0xFF00);
                if (i + 2 < len)
                    dest[i_dest + i + 2] = (byte)(sv & 0xFF0000);
                if (i + 3 < len)
                    dest[i_dest + i + 3] = (byte)(sv & 0xFF000000);
                i += 4;
            }
        }

        unsafe void process_predata(byte* pre, dword pre_len, byte* buf)
        {
            dword* n2 = stackalloc dword[64];
            dword* n3 = stackalloc dword[64];
            dword a = (pubkey.len - 1) / 8;

            fixed (dword* key2 = pubkey.key2)
            {
                fixed (dword* key1 = pubkey.key1)
                {
                    while (a + 1 <= pre_len)
                    {
                        init_bignum(n2, 0, 64);
                        UnsafeClass.memmove(n2, pre, a + 1);
                        calc_a_key(n3, n2, key2, key1, 64);

                        UnsafeClass.memmove(buf, n3, a);

                        pre_len -= a + 1;
                        pre += a + 1;
                        buf += a;
                    }
                }
            }
        }

        public unsafe void get_blowfish_key(byte* s, byte* d)
        {
            if (!public_key_initialized)
            {
                init_pubkey();
                public_key_initialized = true;
            }
            byte* key = stackalloc byte[256];
            process_predata(s, len_predata(), key);
            UnsafeClass.memcpy(d, key, 56);
        }
    }
}
