using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Library
{
    /// <summary>
    /// Ini读写类
    /// </summary>
    public class IniClass
    {
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="inipath">Ini文件路径</param>
        public IniClass(string inipath)
        {
            path = inipath;
            if (!File.Exists(path))
                _createfile(path);
            StreamReader sr = new StreamReader(path, Encoding.Default);
            lines = new List<string>(sr.ReadToEnd().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
            sr.Close();
            readsections();
        }

        /// <summary>
        /// 读取键值（包含注释），若不存在则返回空字符串
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <returns>返回键值（包含注释），若不存在则返回空字符串</returns>
        public string ReadValue(string section, string key)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0 && !sectioninf[index].isnote[index2])
                    return sectioninf[index].values[index2];
            }
            return String.Empty;
        }

        public string ReadValue(string section, string key, string defstr)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0 && !sectioninf[index].isnote[index2])
                    return sectioninf[index].values[index2];
            }
            return defstr;
        }


        /// <summary>
        /// 读取键值（包含注释），若不存在则返回null
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <returns>返回键值（包含注释），若不存在则返回null</returns>
        public string ReadValueTest(string section, string key)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0 && !sectioninf[index].isnote[index2])
                    return sectioninf[index].values[index2];
            }
            return null;
        }

        /// <summary>
        /// 读取键值（不包含注释），若不存在则返回空字符串
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <returns>返回键值（不包含注释），若不存在则返回空字符串</returns>
        public string ReadValueWithoutNotes(string section, string key)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0)
                {
                    if (!sectioninf[index].isnote[index2])
                    {
                        int index3 = sectioninf[index].values[index2].IndexOf(';');
                        if (index3 < 0)
                            return sectioninf[index].values[index2];
                        else
                            return sectioninf[index].values[index2].Substring(0, index3).TrimEnd();
                    }
                }
            }
            return String.Empty;
        }

        public string ReadValueWithoutNotes(string section, string key, string defstr)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0)
                {
                    if (!sectioninf[index].isnote[index2])
                    {
                        int index3 = sectioninf[index].values[index2].IndexOf(';');
                        if (index3 < 0)
                            return sectioninf[index].values[index2];
                        else
                            return sectioninf[index].values[index2].Substring(0, index3).TrimEnd();
                    }
                }
            }
            return defstr;
        }

        /// <summary>
        /// 读取键值（不包含注释），若不存在则返回null
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <returns>返回键值（不包含注释），若不存在则返回null</returns>
        public string ReadValueWithoutNotesTest(string section, string key)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0)
                {
                    if (!sectioninf[index].isnote[index2])
                    {
                        int index3 = sectioninf[index].values[index2].IndexOf(';');
                        if (index3 < 0)
                            return sectioninf[index].values[index2];
                        else
                            return sectioninf[index].values[index2].Substring(0, index3).TrimEnd();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 写入键值
        /// </summary>
        /// <param name="section">Section</param>
        /// <param name="key">Key</param>
        /// <param name="value">写入的键值</param>
        public void WriteValue(string section, string key, string value)
        {
            if (section == null)
                return;
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                if (key == null)
                {
                    sections.RemoveAt(index);
                    sectioninf.RemoveAt(index);
                    return;
                }
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0)
                {
                    if (value == null)
                    {
                        sectioninf[index].keys.RemoveAt(index2);
                        sectioninf[index].values.RemoveAt(index2);
                        sectioninf[index].isnote.RemoveAt(index2);
                    }
                    else
                        sectioninf[index].values[index2] = value;
                }
                else if (value != null)
                {
                    sectioninf[index].keys.Add(key);
                    sectioninf[index].values.Add(value);
                    sectioninf[index].isnote.Add(false);
                }
            }
            else if (key != null)
            {
                sections.Add(section);
                sectioninf.Add(new Section { keys = new List<string> { key }, values = new List<string> { value }, isnote = new List<bool> { false } });
            }
        }

        public void WriteValue(string section, string key, bool bvalue)
        {
            if (section == null)
                return;
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                if (key == null)
                {
                    sections.RemoveAt(index);
                    sectioninf.RemoveAt(index);
                    return;
                }
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0)
                {
                    sectioninf[index].values[index2] = bvalue ? "true" : "false";
                }
                else
                {
                    sectioninf[index].keys.Add(key);
                    sectioninf[index].values.Add(bvalue ? "true" : "false");
                    sectioninf[index].isnote.Add(false);
                }
            }
            else if (key != null)
            {
                sections.Add(section);
                sectioninf.Add(new Section { keys = new List<string> { key }, values = new List<string> { bvalue ? "true" : "false" }, isnote = new List<bool> { false } });
            }
        }

        public void WriteValue(string section, string key, int bvalue)
        {
            if (section == null)
                return;
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                if (key == null)
                {
                    sections.RemoveAt(index);
                    sectioninf.RemoveAt(index);
                    return;
                }
                int index2 = sectioninf[index].keys.FindLastIndex(s => s == key);
                if (index2 >= 0)
                {
                    sectioninf[index].values[index2] = bvalue.ToString();
                }
                else
                {
                    sectioninf[index].keys.Add(key);
                    sectioninf[index].values.Add(bvalue.ToString());
                    sectioninf[index].isnote.Add(false);
                }
            }
            else if (key != null)
            {
                sections.Add(section);
                sectioninf.Add(new Section { keys = new List<string> { key }, values = new List<string> { bvalue.ToString() }, isnote = new List<bool> { false } });
            }
        }

        public Section ReadSection(string section)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                return sectioninf[index];
            }
            return new Section() { keys = new List<string>(), values = new List<string>() };
        }

        /// <summary>
        /// 合并相同项
        /// </summary>
        /// <param name="backorforth">True为合并至后一项，False为合并至前一项</param>
        public void Merge(bool backorforth = false)
        {
            int count = sections.Count, find, i, j;
            if (backorforth)
            {
                sections.Reverse();
                sectioninf.Reverse();
            }
            for (i = 0; i < count; i++)
            {
                while ((find = sections.FindIndex(i + 1, s => s == sections[i])) >= 0)
                {
                    sections.RemoveAt(find);
                    count--;
                    for (j = 0; j < sectioninf[find].keys.Count; j++)
                    {
                        if (!sectioninf[i].keys.Contains(sectioninf[find].keys[j]))
                        {
                            sectioninf[i].keys.Add(sectioninf[find].keys[j]);
                            sectioninf[i].values.Add(sectioninf[find].values[j]);
                        }
                    }
                    sectioninf.RemoveAt(find);
                }
            }
            if (backorforth)
            {
                sections.Reverse();
                sectioninf.Reverse();
            }
        }
        
        /// <summary>
        /// 返回Ini文件路径
        /// </summary>
        /// <returns>返回Ini文件路径</returns>
        public string GetPath()
        {
            return path;
        }

        public struct Section
        {
            //public string section;
            public List<string> keys;
            public List<string> values;
            public List<bool> isnote;
            public string GetValue(string key)
            {
                int tmp = keys.FindLastIndex(s => { return s == key; });
                if (tmp < 0)
                    return null;
                return values[tmp];
            }
        }

        public void Save()
        {
            if (!File.Exists(path))
                _createfile(path);
            int i, j;
            StreamWriter sw = new StreamWriter(path, false, Encoding.Default);
            if (sectionindex.Count > 0)
            {
                for (i = 0; i < sectionindex[0]; i++)
                    sw.WriteLine(lines[i]);
            }
            for (i = 0; i < sectioninf.Count; i++)
            {
                sw.WriteLine("[" + sections[i] + "]");
                for (j = 0; j < sectioninf[i].keys.Count; j++)
                {
                    if (sectioninf[i].isnote[j])
                        sw.WriteLine(sectioninf[i].keys[j]);
                    else
                        sw.WriteLine(sectioninf[i].keys[j] + "=" + sectioninf[i].values[j]);
                }
                sw.WriteLine(String.Empty);
            }
            sw.Close();
        }

        public void SaveAs(string newpath)
        {
            path = newpath;
            CopyAs(newpath);
        }

        public void CopyAs(string newpath)
        {
            if (File.Exists(newpath))
                File.Delete(newpath);
            _createfile(newpath);
            int i, j;
            StreamWriter sw = new StreamWriter(newpath, false, Encoding.Default);
            if (sectionindex.Count > 0)
            {
                for (i = 0; i < sectionindex[0]; i++)
                    sw.WriteLine(lines[i]);
            }
            for (i = 0; i < sectioninf.Count; i++)
            {
                sw.WriteLine("[" + sections[i] + "]");
                for (j = 0; j < sectioninf[i].keys.Count; j++)
                {
                    if (sectioninf[i].isnote[j])
                        sw.WriteLine(sectioninf[i].keys[j]);
                    else
                        sw.WriteLine(sectioninf[i].keys[j] + "=" + sectioninf[i].values[j]);
                }
                sw.WriteLine(String.Empty);
            }
            sw.Close();
        }

        /// <summary>
        /// 获取当前所有section的只读
        /// </summary>
        /// <returns>返回当前所有section的只读</returns>
        public IList<string> GetSections()
        {
            return sections.AsReadOnly();
            //return new List<string>(sections);
        }

        /// <summary>
        /// 获取某个section下的所有键值
        /// </summary>
        /// <param name="section">Section</param>
        /// <returns>返回某个section下的所有键值</returns>
        public IList<string> GetValues(string section)
        {
            List<string> list = new List<string>();
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                for (int i = 0; i < sectioninf[index].values.Count; i++)
                {
                    if (!sectioninf[index].isnote[i])
                    {
                        list.Add(sectioninf[index].values[i]);
                    }
                }
                // return sectioninf[index].values.AsReadOnly();
            }
            return list;
        }

        /// <summary>
        /// 获取某个section下的所有键值（不包含注释）
        /// </summary>
        /// <param name="section">Section</param>
        /// <returns>返回某个section下的所有键值（不包含注释）</returns>
        public IList<string> GetValuesWithoutNotes(string section)
        {
            List<string> list = new List<string>();
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                int id;
                for (int i = 0; i < sectioninf[index].values.Count; i++)
                {
                    if(!sectioninf[index].isnote[i])
                    {
                        id = sectioninf[index].values[i].IndexOf(';');
                        if (id < 0)
                            list.Add(sectioninf[index].values[i]);
                        else
                            list.Add(sectioninf[index].values[i].Substring(0, id).TrimEnd());
                    }
                }
            }
            return list;
        }
        
        /// <summary>
        /// 获取某个section下的所有键
        /// </summary>
        /// <param name="section">Section</param>
        /// <returns>返回某个section下的所有键</returns>
        public IList<string> GetKeys(string section)
        {
            List<string> list = new List<string>();
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
            {
                for (int i = 0; i < sectioninf[index].keys.Count; i++)
                {
                    if (!sectioninf[index].isnote[i])
                        list.Add(sectioninf[index].keys[i]);
                }
                //return sectioninf[index].keys.AsReadOnly();
            }
            return list;
        }

        public int ReadIntValue(string section, string key, int dfValue)
        {
            int ret;
            try
            {
                ret = Convert.ToInt32(this.ReadValueWithoutNotes(section, key));
            }
            catch
            {
                return dfValue;
            }
            return ret;
        }

        public T ReadEnumValue<T>(string section, string key, T dfValue) where T : Enum
        {
            int tmp = ReadIntValue(section, key, Convert.ToInt32(dfValue));
            if (Enum.IsDefined(typeof(T), tmp))
                return (T)Enum.ToObject(typeof(T), tmp);
            return dfValue;
        }

        public uint ReadUIntValue(string section, string key, uint dfValue)
        {
            try
            {
                return Convert.ToUInt32(this.ReadValueWithoutNotes(section, key));
            }
            catch
            {
                return dfValue;
            }
        }

        public bool ReadBoolValue(string section, string key, bool dfValue)
        {
            string vstr = this.ReadValueWithoutNotes(section, key).ToLower();
            if (vstr == "false" || vstr == "no")
                return false;
            if (vstr == "true" || vstr == "yes")
                return true;
            return dfValue;
        }

        public bool IsSectionExist(string section)
        {
            return sections.Contains(section);
        }

        public bool IsKeyExist(string section, string key)
        {
            int index = sections.FindLastIndex(s => s == section);
            if (index >= 0)
                return sectioninf[index].keys.Contains(key);
            return false;
        }

        /*
        private void readsections()
        {
            endpos = read.Length - 1;
            Kmp kmp = new Kmp(1);
            List<int> left = kmp.SearchList(ref read, "[");
            List<int> right = kmp.SearchList(ref read, "]");
            int i, j = -1;
            int coul = left.Count, cour = right.Count;
            string sec;
            if (0 < cour)
            {
                for (i = 0; i < coul; i++)
                {
                    j++;
                    while (right[j] < left[i])
                    {
                        j++;
                        if (j < cour)
                            continue;
                        else
                        { i = coul; break; }
                    }
                    if (i == coul)
                        break;

                    sectionindex.Add(left[i]);
                    left[i]++;
                    sec = read.Substring(left[i], right[j] - left[i]).Trim();
                    sections.Add(sec);
                    if (i < (coul - 1))
                        sectioninf.Add(GetSection(ref read, left[i], _getleft(ref read, ref left, i + 1)));
                    else
                        sectioninf.Add(GetSection(ref read, left[i]));
                }
            }
        }
        */
        private void readsections()
        {
            int i, left, right;
            for (i = 0; i < lines.Count; i++)
            {
                if (((left = lines[i].IndexOf('[')) >= 0) &&
                    ((right = lines[i].IndexOf(']')) >= 0))
                {
                    if (left < right && (lines[i].TrimStart()).StartsWith("["))
                    {
                        sectionindex.Add(i);
                        left++;
                        sections.Add(lines[i].Substring(left, right - left).Trim());
                    }
                }
            }
            _getsection();
            // 节省内存
            if (sectionindex.Count > 0)
            {
                lines.RemoveRange(sectionindex[0], lines.Count - sectionindex[0]);
            }
        }

        /*
        /// <summary>
        /// 获取一个Section的全部内容
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="left">Section的左定位点，必须位于']'之前</param>
        /// <param name="right">Section的右定位点，即位于下一个'['之前</param>
        /// <returns>返回Section信息</returns>
        private Section GetSection(ref string input, int left, int right)
        {
            Section section = new Section() { keys = new List<string>(), values = new List<string>() };
            string[] tmp = input.Substring(left, right - left + 1).Split(']')[1].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tmp2;
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp2 = tmp[i].Split('=');
                section.keys.Add(tmp2[0].Trim());
                if (tmp2.Length > 1)
                    section.values.Add(tmp2[1].Trim());
                else
                    section.values.Add(null);
            }
            return section;
        }

        private Section GetSection(ref string input, int left)
        {
            Section section = new Section() { keys = new List<string>(), values = new List<string>() };
            string[] tmp = input.Substring(left).Split(']')[1].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tmp2;
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp2 = tmp[i].Split('=');
                section.keys.Add(tmp2[0].Trim());
                if (tmp2.Length > 1)
                    section.values.Add(tmp2[1].Trim());
                else
                    section.values.Add(null);
            }
            return section;
        }
        */

        private void _getsection()
        {
            int i, j, left, next, note;
            for (i = 0; i < sectionindex.Count; i++)
            {
                Section section = new Section
                {
                    keys = new List<string>(),
                    values = new List<string>(),
                    isnote = new List<bool>()
                };
                if (i == (sectionindex.Count - 1))
                    next = lines.Count;
                else
                    next = sectionindex[i + 1];
                for (j = (sectionindex[i] + 1); j < next; j++)
                {
                    if ((note = lines[j].IndexOf(';')) >= 0)
                    {
                        if (((left = lines[j].IndexOf('=')) >= 0) && (left < note))
                        {
                            section.keys.Add(lines[j].Substring(0, left).Trim());
                            section.values.Add(lines[j].Substring(left + 1).Trim());
                            section.isnote.Add(false);
                        }
                        else
                        {
                            section.keys.Add(lines[j]);
                            section.values.Add(null);
                            section.isnote.Add(true);
                        }
                    }
                    else
                    {
                        if ((left = lines[j].IndexOf('=')) >= 0)
                        {
                            section.keys.Add(lines[j].Substring(0, left).Trim());
                            left++;
                            if (lines[j].Length == left)
                                section.values.Add(String.Empty);
                            else
                                section.values.Add(lines[j].Substring(left).Trim());
                            section.isnote.Add(false);
                        }
                        else
                        {
                            section.keys.Add(lines[j]);
                            section.values.Add(null);
                            section.isnote.Add(true);
                        }
                    }
                }
                sectioninf.Add(section);
            }
        }

        /*
        private int _getleft(ref string input, ref List<int> left, int curindex)
        {
            int len = left.Count;
            int i;
            bool boo;
            while (true)
            {
                boo = false;
                i = left[curindex] - 1;
                while ((i >= 0) && (input[i] != '\r') && (input[i] != '\n'))
                {
                    if (input[i] != ' ')
                    {
                        boo = true;
                        break;
                    }
                    i--;
                }
                if (boo)
                    curindex++;
                else
                    return i;
                if (curindex == len)
                    break;
            }
            return (input.Length - 1);
        }
        */

        private void _createfile(string fpath)
        {
            PathClass.CreateFile(fpath);
           //string dir = Path.GetDirectoryName(fpath);
			//if (!Directory.Exists(dir))
			//	_createdir(dir);
           //File.Create(fpath).Close();
        }

        //private void _createdir(string dir)
        //{
        //    string ddir = Path.GetDirectoryName(dir);
        //    if (Directory.Exists(ddir))
        //        Directory.CreateDirectory(dir);
        //    else
        //        _createdir(ddir);
        //}

        private List<string> sections = new List<string>();
        private List<Section> sectioninf = new List<Section>();
        //int firstsectionindex;
        private List<int> sectionindex = new List<int>();
        private string path = String.Empty;
        private List<string> lines;
        //private string read = String.Empty;
        //private int endpos = -1;
    }
}
