using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AIcore
{
    public static class Utils
    {
        private static string Md5ByteToString(byte[] bytes)
        {
            string s = "";
            for (int i = 0; i < bytes.Length; ++i)
                s += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            return s.ToString();
        }

        public static string GetMd5(string fpath)
        {
            FileStream file = new FileStream(fpath, FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            md5.Dispose();
            return Md5ByteToString(retVal);
        }

        public static string GetMd5OfString(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(Encoding.Unicode.GetBytes(str));
            md5.Dispose();
            return Md5ByteToString(retVal);
        }

        public static void Sort<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sortedList = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sortedList.Count(); i++)
            {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }
        
        public static void CopyToList<T>(ObservableCollection<T> source, ref ObservableCollection<T> target) where T : class
        {
            InitList(ref target);
            if (source == null)
                return;
            for (int i = 0; i < source.Count; i++)
                target.Add(source[i]);
        }

        public static void AppendToList<T>(ObservableCollection<T> source, ref ObservableCollection<T> target)
        {
            if (target == null)
                target = new ObservableCollection<T>();
            for (int i = 0; i < source.Count; i++)
            {
                target.Add(source[i]);
            }
        }

        public static void AppendToList<T>(Dictionary<string, T> source, ref ObservableCollection<T> target)
        {
            if (target == null)
                target = new ObservableCollection<T>();
            foreach (T t in source.Values)
            {
                target.Add(t);
            }
        }

        public static int GetBuildingSeqIndexFromID(string id)
        {
            try
            {
                int r = Convert.ToInt32(id);
                if (r >= (int)TargetChooseMode.MaxDistance)
                    r -= (int)TargetChooseMode.MaxDistance;
                else if (r >= (int)TargetChooseMode.MinDistance)
                    r -= (int)TargetChooseMode.MinDistance;
                else if (r >= (int)TargetChooseMode.MaxThreat)
                    r -= (int)TargetChooseMode.MaxThreat;
                else
                    r -= (int)TargetChooseMode.MinThreat;
                return r;
            }
            catch
            {
            }
            return -1;
        }

        public static TargetChooseMode GetBuildingModeFromID(string id)
        {
            try
            {
                int r = Convert.ToInt32(id);
                if (r >= (int)TargetChooseMode.MaxDistance)
                    return TargetChooseMode.MaxDistance;
                else if (r >= (int)TargetChooseMode.MinDistance)
                    return TargetChooseMode.MinDistance;
                else if (r >= (int)TargetChooseMode.MaxThreat)
                    return TargetChooseMode.MaxThreat;
                else
                    return TargetChooseMode.MinThreat;
            }
            catch
            {
            }
            return TargetChooseMode.MinThreat;
        }

        public static void FileCopy(string oldpath, string newpath, bool overwrite)
        {
            PathClass.CreateDir(Path.GetDirectoryName(newpath));
            File.Copy(oldpath, newpath, overwrite);
        }

        public static void FileDelete(string filepath, bool DelDirIfEmpty = true)
        {
            if (File.Exists(filepath))
                File.Delete(filepath);
            if (DelDirIfEmpty)
            {
                string dir = Path.GetDirectoryName(filepath);
                if (Directory.Exists(dir) && Directory.GetFiles(dir).Length == 0)
                    Directory.Delete(dir);
            }
        }

        public static TraversalRequest NextTraversalRequest = new TraversalRequest(FocusNavigationDirection.Next);
        public static TraversalRequest DownTraversalRequest = new TraversalRequest(FocusNavigationDirection.Down);
        public static TraversalRequest UpTraversalRequest = new TraversalRequest(FocusNavigationDirection.Up);
        public static TraversalRequest LeftTraversalRequest = new TraversalRequest(FocusNavigationDirection.Left);
        public static TraversalRequest RightTraversalRequest = new TraversalRequest(FocusNavigationDirection.Right);
        public static void SwitchToNextItem<T>(T tab, bool direction = true) where T : Selector
        {
            if (tab.Items.Count <= 0)
                return;

            int itv = direction ? 1 : -1;
            int index = tab.SelectedIndex;
            do
            {
                index += itv;
                if (index >= tab.Items.Count)
                    index = 0;
                else if (index < 0)
                    index = tab.Items.Count - 1;

                if (tab.Items[index] is Control c)
                {
                    if (c.IsEnabled && c.IsVisible)
                    {
                        tab.SelectedIndex = index;
                        break;
                    }
                }
                else
                {
                    tab.SelectedIndex = index;
                    // see how fucked up they are.
                    (tab.ItemContainerGenerator.ContainerFromIndex(index) as UIElement).Focus();
                    break;
                }
            }
            while (index != tab.SelectedIndex);
        }

        public static void MoveFocusWithKey(FrameworkElement ele, KeyEventArgs e)
        {
            if (!ele.IsFocused)
                return;
            switch (e.Key)
            {
                case Key.Tab:
                    ele.MoveFocus(NextTraversalRequest);
                    break;
                case Key.Up:
                    ele.MoveFocus(UpTraversalRequest);
                    break;
                case Key.Down:
                    ele.MoveFocus(DownTraversalRequest);
                    break;
                case Key.Left:
                    ele.MoveFocus(LeftTraversalRequest);
                    break;
                case Key.Right:
                    ele.MoveFocus(RightTraversalRequest);
                    break;
            }
        }

        public static T SwitchToNextItem<T>(UIElementCollection col, int init, bool direction = true) where T : Control
        {
            if (col.Count <= 0)
                return null;

            int itv = direction ? 1 : -1;
            int index = init;
            do
            {
                index += itv;
                if (index >= col.Count)
                    index = 0;
                else if (index < 0)
                    index = col.Count - 1;

                if (col[index] is T c)
                {
                    if (c.IsEnabled && c.IsVisible)
                    {
                        return col[index] as T;
                    }
                }
            }
            while (index != init);
            if (col[init] is T)
                return col[init] as T;
            return null;
        }

        public static void InitList<T>(ref ObservableCollection<T> list, T init = null) where T : class
        {
            if (list == null)
                list = init == null ? new ObservableCollection<T>() : new ObservableCollection<T> { init };
            else
            { 
                list.Clear();
                if (init != null)
                    list.Add(init);
            }
        }
    }

    public abstract class NotifyClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void PropertyChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void PropertyChange(object sender, string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(sender, new PropertyChangedEventArgs(name));
        }
    }

    public class NotifyList<T> : ObservableCollection<T>
    {
        public NotifyList() : base() { }
        public NotifyList(List<T> list) : base(list) { }
        public NotifyList(IEnumerable<T> collection) : base(collection) { }

        public void AddRange(ICollection<T> list)
        {
            foreach (T t in list)
                this.Add(t);
        }

        public void TryAddRange(ICollection<T> list)
        {
            foreach (T t in list)
                this.TryAdd(t);
        }

        public void TryAdd(T list)
        {
            if (!this.Contains(list))
                this.Add(list);
        }
    }
}
