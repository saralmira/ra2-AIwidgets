using System;

namespace AIcore
{
    public class Side
    {
        public bool IsEnabled { get; set; }
        public int Index { get; set; }
        public string SIndex { get { return Index.ToString(); } set { try { Index = Convert.ToInt32(value); IsEnabled = true; } catch { IsEnabled = false; } } }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool CompareWith(Side a)
        {
            return a.Index == Index;
        }
    }

}
