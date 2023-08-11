using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace RA2AI_Editor
{
    public class CsfClass
    {
        public CsfClass(string path)
        {
            IsValidCSF = false;
            try
            {
                if (!File.Exists(path))
                    return;
                int length = Convert.ToInt32(new FileInfo(path).Length);
                if (length < 24)
                    return;
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader br = new BinaryReader(fs);
                header = new Header(br);
                if (header.IsValid)
                {
                    elements = new Dictionary<string, Element>();
                    for (int i = 0; i < header.TagCount; ++i)
                    {
                        Element ele = new Element(br);
                        if (ele.TagName != null)
                            elements[ele.TagName] = ele;
                    }
                    IsValidCSF = true;
                }
                br.Close();
                fs.Close();
                
            }
            catch (Exception e)
            {
                if (elements == null || elements.Count == 0)
                    IsValidCSF = false;
            
                MessageBox.Show(e.Message);
            }
        }

        public bool IsValidCSF { get; private set; }
        private readonly Header header;
        private readonly Dictionary<string, Element> elements;
        public int ElementCount { get { return elements.Count; } }

        public string GetString(string tag)
        {
            if (tag != null)
            {
                tag = tag.ToUpper();
                if (elements.ContainsKey(tag))
                    return elements[tag].GetString();
            }
            return null;
        }

        public class Header
        {
            public Header(BinaryReader stream, int offset = 0)
            {
                IsValid = false;
                byte[] bytes = new byte[24];
                stream.Read(bytes, 0, bytes.Length);
                if (bytes[offset] == 0x20
                    && bytes[offset + 1] == 0x46
                    && bytes[offset + 2] == 0x53
                    && bytes[offset + 3] == 0x43)
                {
                    TagCount = BitConverter.ToUInt32(bytes, 8);
                    StringCount = BitConverter.ToUInt32(bytes, 12);
                    IsValid = true;
                }
            }
            public bool IsValid { get; private set; }
            public UInt32 TagCount;
            public UInt32 StringCount;
        }

        public class Element
        {
            public Element(BinaryReader stream)
            {
                byte[] bytes = new byte[4];

                while (LengthLargerOrEqualThan(stream, 12))
                {
                    stream.Read(bytes, 0, bytes.Length);
                    if (IsValidTagHeader(bytes))
                    {
                        int c = (int)BitConverter.ToUInt32(stream.ReadBytes(4), 0);
                        int len = (int)BitConverter.ToUInt32(stream.ReadBytes(4), 0);
                        TagName = Encoding.ASCII.GetString(stream.ReadBytes(len), 0, len).ToUpper();
                        Strings = GetStrings(stream, c);
                        break;
                    }
                }

                if (Strings == null || Strings.Count == 0)
                    Strings = new List<string>() { "" };
            }

            private bool IsValidTagHeader(byte[] bytes, int offset = 0)
            {
                return (bytes[offset] == 0x20 && bytes[offset + 1] == 0x4c && bytes[offset + 2] == 0x42 && bytes[offset + 3] == 0x4c);
            }

            private List<string> GetStrings(BinaryReader stream, int count)
            {
                List<string> list = new List<string>();
                byte[] bytes = new byte[4];
                for (int i = 0; i < count; ++i)
                {
                    while (LengthLargerOrEqualThan(stream, 8))
                    {
                        stream.Read(bytes, 0, bytes.Length);
                        // RTS
                        if (bytes[0] == 0x20 && bytes[1] == 0x52 && bytes[2] == 0x54 && bytes[3] == 0x53)
                        {
                            int len = (int)BitConverter.ToUInt32(stream.ReadBytes(4), 0);
                            list.Add(Encoding.Unicode.GetString(Decode(stream.ReadBytes(len * 2))));
                            break;
                        }
                        // WRTS
                        else if (bytes[0] == 0x57 && bytes[1] == 0x52 && bytes[2] == 0x54 && bytes[3] == 0x53)
                        {
                            int len = (int)BitConverter.ToUInt32(stream.ReadBytes(4), 0);
                            list.Add(Encoding.Unicode.GetString(Decode(stream.ReadBytes(len * 2))));
                            UInt32 len2 = BitConverter.ToUInt32(stream.ReadBytes(4), 0);
                            stream.BaseStream.Position += len2 * 2;
                            break;
                        }
                    }
                }
                return list;
            }

            private bool LengthLargerOrEqualThan(BinaryReader br, int len)
            {
                return br.BaseStream.Length - br.BaseStream.Position >= len;
            }

            private byte[] Decode(byte[] bytes)
            {
                for (int i = 0; i < bytes.Length; ++i)
                    //bytes[i] = (byte)~bytes[i];
                    bytes[i] = (byte)(0xFF - bytes[i]);
                return bytes;
            }

            public string TagName;
            public List<string> Strings;

            public string GetString()
            {
                return Strings[0];
            }
        }
    }
}
