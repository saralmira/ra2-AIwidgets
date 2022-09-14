using AIcore.Types;
using Library;
using System;
using System.Collections.ObjectModel;

namespace AIcore.TLists
{
    public class TTypeLists<Type> where Type : OType
    {
        public TTypeLists(AI _ai)
        {
            ai = _ai;
            ini = ai.ini;
            tag_post = "";
        }

        protected IniClass ini;
        protected AI ai;
        protected string tag_post;
        protected UInt32 current_tag;
        protected UInt32 base_tag;
        protected ObservableCollection<Type> typelist;

        protected string GenerateTag(ObservableCollection<Type> list)
        {
            string tag;
            do
            {
                if (++current_tag >= 0x0FFFFFFF)
                {
                    current_tag = base_tag;
                    tag_post += "0";
                }
                tag = Convert.ToString(current_tag, 16).ToUpper().PadLeft(8, '0');

                tag += ai.IsMapFile ? tag_post : (tag_post + "-G");
            }
            while (F_Exist(list, tag));
            return tag;
        }

        public void ResetTag()
        {
            current_tag = base_tag;
        }

        public static bool F_Exist(ObservableCollection<Type> list, string tag)
        {
            if (F_FindIndex(list, tag) >= 0)
                return true;
            return false;
        }

        public static Type F_Find(ObservableCollection<Type> list, string tag)
        {
            foreach (Type t in list)
                if (t._tag == tag)
                    return t;
            return null;
        }

        public static int F_FindIndex(ObservableCollection<Type> list, string tag)
        {
            for (int i = 0; i < list.Count; ++i)
                if (list[i]._tag == tag)
                    return i;
            return -1;
        }

        public Type Find(string tag)
        {
            return F_Find(typelist, tag);
        }

        public int GetTypeIndex(Type t)
        {
            return typelist.IndexOf(t);
        }

        public virtual void Delete(string tag) 
        {
            int index = F_FindIndex(typelist, tag);
            if (index >= 0)
                typelist.RemoveAt(index);
        }

        public virtual void Delete(Type t)
        {
            typelist.Remove(t);
        }

        public virtual void Add(Type t, int index = -1)
        {
            // prevent to make new items on the top of the list.
            if (index > 0 && typelist.Count > index)
                typelist.Insert(index, t);
            else
                typelist.Add(t);
        }

        public virtual void TryAdd(Type t, int index = -1)
        {
            if (typelist.Contains(t))
                return;
            Add(t, index);
        }
    }
}
