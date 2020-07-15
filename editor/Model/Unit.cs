using Library;
using System;
using System.ComponentModel;
using System.Windows;
using System.Xml;

namespace AIcore
{
    public class Unit : INotifyPropertyChanged
    {
        public Unit(UnitType type, XmlElement ele)
        {
            UType = type;
            if (ele == null)
            {
                Name = null;
                IsEnabled = false;
                TechLevel = -1;
            }
            else
            {
                SSequenceIndex = ele.GetAttribute("SequenceNumber").Trim();
                UIName = ele.GetAttribute("UIName").Trim();
                Name = ele.GetAttribute("Name").Trim();
                SIndex = ele.GetAttribute("Index").Trim();
                Description = ele.InnerText.Trim();
                SIsEnabled = ele.GetAttribute("IsEnabled").Trim();
                SCost = ele.GetAttribute("Cost").Trim();
                STechLevel = ele.GetAttribute("TechLevel").Trim();
            }
            UIVisibility = Visibility.Visible;
        }

        public Unit(UnitType type, int seq, string name, string key, IniClass rules)
        {
            UType = type;
            SequenceIndex = seq;
            Name = name;
            if (rules != null)
            {
                SIndex = key;
                UIName = rules.ReadValueWithoutNotes(Name, "UIName");
                Description = rules.ReadValueWithoutNotes(Name, "Name");
                SCost = rules.ReadValueWithoutNotes(Name, "Cost");
                STechLevel = rules.ReadValueWithoutNotes(Name, "TechLevel");
                IsEnabled = rules.IsSectionExist(Name) && TechLevel >= 0 && Cost >= 0;
            }
            UIVisibility = Visibility.Visible;
        }

        public Unit(UnitType type, int seq)
        {
            UType = type;
            SequenceIndex = seq;
            UIVisibility = Visibility.Visible;
        }

        public UnitType UType { get; set; }

        public int Index;
        public string SIndex
        {
            get { return Index.ToString(); }
            set { try { Index = Convert.ToInt32(value); } catch { Index = -1; IsEnabled = false; } }
        }

        // 顺序号
        public int SequenceIndex { get; private set; }
        public string SSequenceIndex
        {
            get { return SequenceIndex.ToString(); }
            private set { try { SequenceIndex = Convert.ToInt32(value); } catch { SequenceIndex = -1; IsEnabled = false; } }
        }

        public string UIName { get; set; }

        public string Name { get; set; }
        public string NameOrNone
        {
            get
            {
                if (Name == null || Name.Length == 0) return "<none>";
                return Name;
            }
            set
            {
                Name = value;
            }
        }

        public string Description { get; set; }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                PropertyChange(this, "IsEnabled");
                PropertyChange(this, "SIsEnabled");
            }
        }
        public string SIsEnabled
        {
            get { return IsEnabled.ToString(); }
            set { try { IsEnabled = Convert.ToBoolean(value); } catch { IsEnabled = false; } }
        }

        public int Cost { get; set; }
        public string SCost
        {
            get { return Cost.ToString(); }
            set { try { Cost = Convert.ToInt32(value); } catch { Cost = -1; } }
        }

        public int TechLevel { get; set; }
        public string STechLevel
        {
            get { return TechLevel.ToString(); }
            set { try { TechLevel = Convert.ToInt32(value); } catch { TechLevel = -1; } }
        }

        private Visibility _UIVisibility;
        public Visibility UIVisibility
        {
            get { return _UIVisibility; }
            set
            {
                _UIVisibility = value;
                PropertyChange(this, "UIVisibility");
            }
        }

        protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
        {

        }
        protected void PropertyChange(object sender, string name)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(sender, new PropertyChangedEventArgs(name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
