using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using RA2AI_Editor.Model;
using System.IO;
using Library;
using System.Collections.ObjectModel;
using System.Windows.Data;
using AIcore;
using AIcore.Types;

namespace RA2AI_Editor.Data
{
    public class ListBoxDataInit
    {
        private IniClass Config;
        private const int MaxItems = 15;

        public delegate void HistoryFileOpenEvent(string path);
        public delegate void SizeChangedEvent(int width);

        public HistoryFileOpenEvent HistoryFileOpen;
        public SizeChangedEvent sizeChangedEventHandler;

        public ListBoxDataInit(IniClass config, int width)
        {
            Config = config;
            defaultWidth = width;
            ItemList = new ObservableCollection<HistoryItemInfo>();
            sizeChangedEventHandler = SizeChanged;
            string dateformat = Local.Dictionary("DATE_FORMAT");
            emptyItem = new HistoryItemInfo()
            {
                IsEnabled = false,
                UserBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fef9eb")),
                UserBackground2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fef9eb")),
                Header = "",
                Name = "",
                Info = "",
                BtnName = "",
                BtnVisibility = System.Windows.Visibility.Hidden,
                Mark = Local.Dictionary("HISTORY_NOFILES"),
                Width = defaultWidth
            };
            emptyItem.PropertyChanged += Width_PropertyChanged;
            foreach (string key in Config.GetKeys("HistoryFiles"))
            {
                string value = Config.ReadValueWithoutNotes("HistoryFiles", key);
                if (!File.Exists(value))
                    continue;

                HistoryItemInfo h = new HistoryItemInfo()
                {
                    IsEnabled = true,
                    UserBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fceeb9")),
                    UserBackground2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fae388")),
                    Header = key,
                    Name = value,
                    Info = new FileInfo(value).LastWriteTime.ToString(dateformat),
                    BtnName = Local.Dictionary("HISTORY_FILE_OPEN"),
                    BtnVisibility = System.Windows.Visibility.Visible,
                    Mark = "",
                    Width = defaultWidth
                };
                h.PropertyChanged += Width_PropertyChanged;
                ItemList.Add(h);
            }
            if (ItemList.Count == 0)
            {
                ItemList.Add(emptyItem);
                IsEmpty = true;
            }
            else
                IsEmpty = false;
        }

        private void Width_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }

        // 如果用List<>将无法触发ItemList数据源的变化事件
        public ObservableCollection<HistoryItemInfo> ItemList { get; set; }

        public void Save()
        {
            Config.WriteValue("HistoryFiles", null, null);
            for (int i = 0; i < Math.Min(MaxItems, ItemList.Count); ++i)
                if (ItemList[i] != emptyItem)
                    Config.WriteValue("HistoryFiles", ItemList[i].Header, ItemList[i].Name);
        }

        public void Delete(string path)
        {
            for (int i = 0; i < ItemList.Count;)
            {
                if (ItemList[i].Name == path)
                    ItemList.RemoveAt(i);
                else
                    ++i;
            }
            if (ItemList.Count == 0)
            {
                IsEmpty = true;
                if (emptyItem != null)
                    ItemList.Add(emptyItem);
            }
        }

        public void Delete(HistoryItemInfo item)
        {
            ItemList.Remove(item);
            if (ItemList.Count == 0)
            {
                IsEmpty = true;
                if (emptyItem != null)
                    ItemList.Add(emptyItem);
            }
        }

        public void Add(string path)
        {
            if (IsEmpty)
            {
                IsEmpty = false;
                ItemList.Clear();
            }
            int width = defaultWidth, index = -1;
            for (int i = 0; i < ItemList.Count; ++i)
            {
                width = ItemList[i].Width;
                if (ItemList[i].Name == path)
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                if (ItemList.Count >= MaxItems)
                    ItemList.RemoveAt(ItemList.Count - 1);
                HistoryItemInfo h = new HistoryItemInfo()
                {
                    IsEnabled = true,
                    UserBackground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fceeb9")),
                    UserBackground2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fae388")),
                    Header = Path.GetFileName(path),
                    Name = path,
                    Info = new FileInfo(path).LastWriteTime.ToString(Local.Dictionary("DATE_FORMAT")),
                    BtnName = Local.Dictionary("HISTORY_FILE_OPEN"),
                    BtnVisibility = System.Windows.Visibility.Visible,
                    Mark = "",
                    Width = width
                };
                h.PropertyChanged += Width_PropertyChanged;
                ItemList.Insert(0, h);
            }
            else
                ItemList.Move(index, 0);
        }

        public string GetName(string path)
        {
            foreach(HistoryItemInfo hi in ItemList)
            {
                if (hi.Name == path)
                    return "(" + hi.Header + ")";
            }
            return String.Empty;
        }

        public bool IsEmpty { get; private set; }

        private void SizeChanged(int width)
        {
            foreach (HistoryItemInfo h in ItemList)
                h.Width = width;
            if (emptyItem != null)
                emptyItem.Width = width;
        }

        private HistoryItemInfo emptyItem;
        private int defaultWidth;
    }
}
