using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MixFileClass
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MixFileEntry
    {
        public UInt32 ID;
        public Int32 Offset;
        public Int32 Size;
    }

    public class MixFileClass
    {
        const int mix_checksum = 0x00010000;
        const int mix_encrypted = 0x00020000;

        public MixFileClass(string filepath) 
        { 
            mixFilePath = filepath;
            if (!File.Exists(filepath))
                return;

            mixFile = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (!(IsValid = ParseHeader(mixFile)))
                return;
        }

        ~MixFileClass()
        { 
            if (mixFile == null) return;
            mixFile.Dispose();
            mixFile = null;
        }

        public bool GetFileEntry(string filename, out MixFileEntry entry)
        {
            var id = GetID(filename);
            if (MixFileEntries.TryGetValue(id, out entry))
            {
                return true;
            }
            return false;
        }

        public bool ReadFile(string filename, out byte[] bytes)
        {
            if (GetFileEntry(filename, out var entry))
            {
                bytes = new byte[entry.Size];
                mixFile.Seek(BodyOffset + entry.Offset, SeekOrigin.Begin);
                mixFile.Read(bytes, 0, bytes.Length);
                return true;
            }
            bytes = Array.Empty<byte>();
            return false;
        }

        unsafe bool ParseHeader(Stream stream)
        {
            var len = stream.Length - 10;
            if (len <= 0) return false;
            BinaryReader br = new BinaryReader(stream);
            IsEncrypted = (br.ReadUInt32() & mix_encrypted) != 0;
            int entry_len = Marshal.SizeOf<MixFileEntry>();
            if (IsEncrypted)
            {
                len -= (88 - 6);
                if (len <= 0) return false;
                byte[] key_source = br.ReadBytes(80);
                byte[] key_output = new byte[56];

                fixed (byte* srckey = key_source)
                {
                    fixed (byte* outputkey = key_output)
                    {
                        WSkeyCal wskeyCal = new WSkeyCal();
                        wskeyCal.get_blowfish_key(srckey, outputkey);

                        Cblowfish cblowfish = new Cblowfish();
                        cblowfish.set_key(key_output, key_output.Length);

                        byte[] decrypted = new byte[8];
                        List<byte> decrypted_entry = new List<byte>();
                        fixed (byte* decrypted_p = decrypted)
                        {
                            byte[] encrypted_segment = br.ReadBytes(8);
                            fixed (byte* encrypted_seg_p = encrypted_segment)
                            {
                                cblowfish.decipher(encrypted_seg_p, decrypted_p, 8);
                            }
                        }
                        FilesCount = BitConverter.ToInt16(decrypted, 0);
                        BodySize = BitConverter.ToInt32(decrypted, 2);
                        int enc_seg_res = (int)Math.Ceiling((double)(6 + 12 * FilesCount) / 8) - 1;
                        BodyOffset = 92 + enc_seg_res * 8;
                        if (FilesCount > 0)
                        {
                            decrypted_entry.AddRange(decrypted);
                            enc_seg_res *= 8;
                            len -= enc_seg_res;
                            if (len < 0) return false;
                            byte[] encrypted_segment = br.ReadBytes(enc_seg_res);
                            decrypted = new byte[enc_seg_res];
                            fixed (byte* encrypted_seg_p = encrypted_segment)
                            {
                                fixed (byte* decrypted_p = decrypted)
                                {
                                    cblowfish.decipher(encrypted_seg_p, decrypted_p, enc_seg_res);
                                }
                            }
                            decrypted_entry.AddRange(decrypted);
                            decrypted = decrypted_entry.ToArray();
                            BytesToEntry(decrypted, 6);
                        }
                        if (FilesCount < 0)
                            return false;
                    }
                }
            }
            else
            {
                FilesCount = br.ReadInt16();
                BodySize = br.ReadInt32();

                int entry_off = FilesCount * entry_len;
                len -= entry_off;
                if (len < 0) return false;
                BodyOffset = 10 + entry_off;

                byte[] entry_buffer = br.ReadBytes(FilesCount * entry_len);
                BytesToEntry(entry_buffer);
            }
            return true;
        }

        unsafe void BytesToEntry(byte[] entry_buffer, int index = 0, int length = 0)
        {
            fixed (byte* entry_p = entry_buffer)
            {
                byte* ent_ptr = entry_p + index;
                length = length <= 0 ? (entry_buffer.Length - index) : length;
                int entry_len = Marshal.SizeOf<MixFileEntry>();
                while (length >= entry_len)
                {
                    length -= entry_len;
                    var entry = Marshal.PtrToStructure<MixFileEntry>((IntPtr)ent_ptr);
                    MixFileEntries.Add(entry.ID, entry);
                    ent_ptr += entry_len;
                }
            }
        }

        protected string mixFilePath;

        public Int16 FilesCount { get; protected set; }
        public Int32 BodySize { get; protected set; }
        public bool IsValid { get; protected set; }
        public bool IsEncrypted { get; protected set; }

        protected int BodyOffset;

        protected Dictionary<UInt32, MixFileEntry> MixFileEntries = new Dictionary<UInt32, MixFileEntry>();

        protected FileStream? mixFile;

        public unsafe UInt32 GetID(string filename)
        {
            string name = filename.Replace('/', '\\').ToUpper();
            int l = name.Length;
            int a = l & ~3;
            if ((l & 3) != 0)
            {
                name += (char)(l - a);
                int fix = 3 - (l & 3);
                while (fix > 0)
                {
                    --fix;
                    name += name[a];
                }
            }
            byte[] nbytes = Encoding.Default.GetBytes(name);
            fixed (byte* p = nbytes)
            {
                Ccrc ccrc = new Ccrc();
                ccrc.init();
                ccrc.do_block(p, nbytes.Length);
                return ccrc.get_crc();
            }
        }
    }
}
