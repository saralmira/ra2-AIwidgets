using System;

namespace AIcore
{
    public class CompareInfoClass
    {
        public CompareTypes CompareTypes { get; set; }
        public string SCompareTypes { get { return ((int)CompareTypes).ToString(); } set { try { CompareTypes = (CompareTypes)Convert.ToInt32(value); } catch { CompareTypes = CompareTypes.UnequalThan; } } }
        public string Description { get; set; }
    }

    public class InfoValueClass
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public class FileInfoClass
    {
        public string FileOriginalPath { get; set; }
        public string FilePath { get; set; }
        public string MD5 { get; set; }
        public DateTime FileLastWriteTime { get; set; }
    }

    public class TabItemInfo
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }
        public System.Windows.Controls.TabItem TabItem { get; set; }
    }
}
