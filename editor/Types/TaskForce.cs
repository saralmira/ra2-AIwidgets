using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using Library;

namespace AIcore.Types
{
    public class TaskForce : OType, INotifyPropertyChanged
    {
        public TaskForce(string tag) : base(tag)
        {
            if (tag == null)
                Name = "";
            else
                Name = "_TaskForce_";
            Group = -1;
            _clist = new ObservableCollection<TaskForceData>();
            Add("");

            //GroupInfo = AIcore.GroupInfo.Group;
        }

        public TaskForce(IniClass ini, string tag) : base(tag)
        {
            rec_tag = tag;
            NewType = false;
            Init(ini, tag);

            //GroupInfo = AIcore.GroupInfo.Group;
        }

        protected override void Init(IniClass ini, string tag)
        {
            PName = ini.ReadValueWithoutNotes(tag, "Name");
            PGroup = ini.ReadValueWithoutNotes(tag, "Group");
            string value;
            if (_clist == null)
                _clist = new ObservableCollection<TaskForceData>();
            else
                _clist.Clear();
            foreach (string key in ini.GetKeys(tag))
            {
                if (Regex.IsMatch(key, @"^[0-9]+$"))
                {
                    value = ini.ReadValueWithoutNotes(tag, key);
                    if (Regex.IsMatch(value, @"^[0-9]+\,[\s\S]*$"))
                    {
                        Add(value);
                    }
                }
            }
            if (_clist.Count == 0)
                Add("");
        }

        public void Add(int count, string uname)
        {
            int i = _clist.Count > 0 ? (_clist[_clist.Count - 1].Index + 1) : 0;
            _clist.Add(new TaskForceData() { Index = i, Count = count, Name = uname });
        }

        public void Add(string value)
        {
            int index = value.IndexOf(',');
            int ikey = _clist.Count > 0 ? (_clist[_clist.Count - 1].Index + 1) : 0;

            if (index >= 0)
                _clist.Add(new TaskForceData() { Index = ikey, SCount = value.Substring(0, index).Trim(), Name = value.Substring(index + 1).Trim() });
            else
                _clist.Add(new TaskForceData() { Index = ikey, SCount = value.Trim(), Name = "" });
        }

        /// <summary>
        /// 在此之后插入
        /// </summary>
        /// <param name="index"></param>
        public void Insert(int index)
        {
            _clist.Insert(index + 1, new TaskForceData());
            CheckIndex(index + 1);
        }

        public void Move(int oldindex, int index)
        {
            _clist.Move(oldindex, index);
            CheckIndex(Math.Min(oldindex, index));
        }

        public void Delete(int index)
        {
            if (index < 0 || _clist.Count <= index)
                return;
            _clist.RemoveAt(index);
            CheckIndex(index);
            if (_clist.Count == 0)
                Add("");
        }

        public TaskForce CloneType(string tag)
        {
            TaskForce t = new TaskForce(tag)
            {
                PGroup = this.PGroup,
                PName = this.PName
            };
            t._clist.Clear();
            foreach (TaskForceData td in this._clist)
                t.Add(td.Count, td.Name);
            return t;
        }

        private void CheckIndex(int start = 0)
        {
            for (int j = start; j < _clist.Count; ++j)
                _clist[j].Index = j;
        }

        public int GetTechLevelMax()
        {
            int tec = 0;
            foreach (TaskForceData d in _clist)
            {
                Unit u = Units.FindUnit(d.Name);
                if (u.TechLevel > tec)
                    tec = u.TechLevel;
            }
            return tec;
        }

        private bool IsTaskForceDataContained(TaskForceData tfd)
        {
            foreach (TaskForceData d in _clist)
            {
                if (d == tfd)
                    return true;
            }
            return false;
        }

        public override void Output(IniClass ini)
        {
            ini.WriteValue(_tag, null, null);
            ini.WriteValue(_tag, "Name", Name);
            if (_clist.Count > 1 || _clist[0].Count > 0)
            {
                for (int i = 0; i < _clist.Count; ++i)
                {
                    ini.WriteValue(_tag, _clist[i].Index.ToString(), _clist[i].SCount + "," + _clist[i].Name);
                }
            }
            ini.WriteValue(_tag, "Group", PGroup);
        }

        public bool CompareWith(TaskForce a)
        {
            if (a.PTag != PTag)
                return false;
            if (a._clist.Count != _clist.Count)
                return false;

            foreach(TaskForceData d in _clist)
            {
                if (!a.IsTaskForceDataContained(d))
                    return false;
            }
            return true;
        }

        //public ObservableCollection<Unit> Hints { get; set; }
        public int Group;
        public string PGroup
        {
            get { return Group.ToString(); }
            set
            {
                try
                {
                    Group = Convert.ToInt32(value);
                }
                catch
                {
                    Group = -1;
                }
                PropertyChange(this, "PGroup");
            }
        }

        public ObservableCollection<TaskForceData> _clist;

        public ObservableCollection<InfoValueClass> GroupInfo
        {
            get { return AIcore.GroupInfo.Group; }
        }
       
        public const int MaxCount = 6;
        public class TaskForceData : INotifyPropertyChanged
        {
            public TaskForceData()
            {
                PropertyChanged += PropertyChangedEvent;
                Index = 0;
                Count = 0;
                Name = "";
                //TechTypeInfo = Units.UnitsList;
            }

            private int _index;
            public int Index
            {
                get { return _index; }
                set
                {
                    _index = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SIndex"));
                }
            }
            public string SIndex
            {
                get { return _index.ToString(); }
                set
                {
                    try
                    {
                        _index = Convert.ToInt32(value);
                    }
                    catch
                    {
                        _index = 0;
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SIndex"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Index"));
                }
            }

            private int _Count;
            public int Count { get { return _Count; } set { _Count = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count")); } }
            public string SCount
            {
                get { return Count.ToString(); }
                set
                {
                    try
                    {
                        Count = Convert.ToInt32(value);
                    }
                    catch
                    {
                        Count = 0;
                    }
                }
            }

            private string _name;
            public string Name { get { return _name; } set { _name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name")); PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Description")); } }
            public string Description
            {
                get
                {
                    return Units.GetUnitDescription(_name);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {

            }

            public AutoCompleteFilterPredicate<object> TechTypeFilter
            {
                get
                {
                    return (searchText, obj) => Units.FuzzyLogic(obj as Unit, searchText);
                }
            }

            public ObservableCollection<Unit> TechTypeInfo
            {
                get { return Units.UnitsList; }
            }

            public static bool operator ==(TaskForceData a,TaskForceData b)
            {
                return a.Count == b.Count && a.Name == b.Name;
            }

            public static bool operator !=(TaskForceData a, TaskForceData b)
            {
                return !(a == b);
            }
        }
    }
}
