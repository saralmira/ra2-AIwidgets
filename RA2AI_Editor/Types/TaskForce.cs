using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using AIcore.TLists;
using Library;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AIcore.Types
{
    public class TaskForce : TaskForceBase
    {
        public TaskForce(string tag) : base(tag)
        {
            if (tag == null)
                Name = "";
            else
                Name = "_TaskForce_";
            Group = -1;

            Ext_EasyMode_Type = new TaskForceBase(null);
            Ext_MediumMode_Type = new TaskForceBase(null);
            Ext_HardMode_Type = new TaskForceBase(null);
            //GroupInfo = AIcore.GroupInfo.Group;
        }

        public TaskForce(IniClass ini, string tag) : base(tag)
        {
            rec_tag = tag;
            NewType = false;
            Ext_EasyMode_Type = new TaskForceBase(null);
            Ext_MediumMode_Type = new TaskForceBase(null);
            Ext_HardMode_Type = new TaskForceBase(null);

            Init(ini, tag);

            //GroupInfo = AIcore.GroupInfo.Group;
        }

        protected override void Init(IniClass ini, string tag)
        {
            PName = ini.ReadValueWithoutNotes(tag, "Name");
            PGroup = ini.ReadValueWithoutNotes(tag, "Group");
            base.Init(ini, tag);

            EnableExt = ini.ReadBoolValue(tag, nameof(EnableExt), false);
            Ext_EasyMode_Type.Parse(ini.ReadValueWithoutNotes(tag, nameof(Ext_EasyMode_Type)), _clist);
            Ext_MediumMode_Type.Parse(ini.ReadValueWithoutNotes(tag, nameof(Ext_MediumMode_Type)), _clist);
            Ext_HardMode_Type.Parse(ini.ReadValueWithoutNotes(tag, nameof(Ext_HardMode_Type)), _clist);
        }

        public TaskForce CloneType(string tag)
        {
            TaskForce t = new TaskForce(tag)
            {
                PGroup = this.PGroup,
                PName = this.PName
            };
            t.CloneFrom(_clist);
            return t;
        }

        public override void Output(IniClass ini, bool release)
        {
            ini.WriteValue(_tag, null, null);
            ini.WriteValue(_tag, "Name", Name);
            base.Output(ini, release);
            ini.WriteValue(_tag, "Group", PGroup);

            // ext
            if (!release)
            {
                ini.WriteValue(_tag, nameof(EnableExt), EnableExt);
                ini.WriteValue(_tag, nameof(Ext_EasyMode_Type), Ext_EasyMode_Type.toString());
                ini.WriteValue(_tag, nameof(Ext_MediumMode_Type), Ext_MediumMode_Type.toString());
                ini.WriteValue(_tag, nameof(Ext_HardMode_Type), Ext_HardMode_Type.toString());
            }
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

        public ObservableCollection<InfoValueClass> GroupInfo
        {
            get { return AIcore.GroupInfo.Group; }
        }
       
        public const int MaxCount = 6;

        // Extension
        private bool _EnableExt = false;
        public bool EnableExt
        {
            get => _EnableExt;
            set
            {
                _EnableExt = value;
                PropertyChange(nameof(EnableExt));
            }
        }
        public TaskForceBase Ext_EasyMode_Type { get; set; }
        public TaskForceBase Ext_MediumMode_Type { get; set; }
        public TaskForceBase Ext_HardMode_Type { get; set; }

        public override int GetTechLevelMax()
        {
            if (EnableExt)
            {
                return Math.Max(Math.Max(Ext_EasyMode_Type.GetTechLevelMax(),
                    Ext_MediumMode_Type.GetTechLevelMax()),
                    Ext_HardMode_Type.GetTechLevelMax());
            }
            else
            {
                return base.GetTechLevelMax();
            }
        }

        public TaskForce CloneFromBase(TaskForceBase tfb, string tag, string modefix = "")
        {
            var ext = this.CloneType(tag);
            ext.BindList = tfb.BindList;
            ext.PName = NameWithExt(this.PName, modefix);
            return ext;
        }
    }

    public class TaskForceData : NotifyClass
    {
        public TaskForceData()
        {
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
                PropertyChange(nameof(Index));
                PropertyChange(nameof(SIndex));
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
                PropertyChange(nameof(SIndex));
                PropertyChange(nameof(Index));
            }
        }

        private int _Count;
        public int Count { get { return _Count; } set { _Count = value; PropertyChange(nameof(Count)); } }
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
        public string Name { get { return _name; } set { _name = value; PropertyChange(nameof(Name)); PropertyChange(nameof(Description)); PropertyChange(nameof(Translation)); } }
        public string Description
        {
            get { return Units.GetUnitDescription(_name); }
        }
        public string Translation
        {
            get { return Units.GetUnitTranslation(_name); }
        }

        public AutoCompleteFilterPredicate<object> TechTypeFilter => (searchText, obj) => Units.FuzzyLogic(obj as Unit, searchText);

        public ObservableCollection<Unit> TechTypeInfo
        {
            get { return Units.UnitsList; }
        }

        public static bool operator ==(TaskForceData a, TaskForceData b)
        {
            return a.Count == b.Count && a.Name == b.Name;
        }

        public static bool operator !=(TaskForceData a, TaskForceData b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    
    public class TaskForceBase : OType
    {
        public TaskForceBase(string tag) : base(tag)
        {
            _clist = new NotifyList<TaskForceData>();
            Add("");
        }

        public NotifyList<TaskForceData> _clist;
        public NotifyList<TaskForceData> BindList { get => _clist; set { _clist = value; PropertyChange(nameof(BindList)); } }

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

        public void CloneFrom(NotifyList<TaskForceData> clist)
        {
            _clist.Clear();
            foreach (var td in clist)
                Add(td.Count, td.Name);
        }

        public override void Output(IniClass ini, bool release)
        {
            if (_clist.Count > 1 || _clist[0].Count > 0)
            {
                for (int i = 0; i < _clist.Count; ++i)
                {
                    ini.WriteValue(_tag, _clist[i].Index.ToString(), _clist[i].SCount + "," + _clist[i].Name);
                }
            }
        }

        public string toString()
        {
            List<string> parts = new List<string>();
            if (_clist.Count > 1 || _clist[0].Count > 0)
            {
                for (int i = 0; i < _clist.Count; ++i)
                {
                    parts.Add(_clist[i].SCount + "," + _clist[i].Name);
                }
            }
            return string.Join("/", parts.ToArray());
        }

        public void Parse(string tflist, NotifyList<TaskForceData> defaultlist)
        {
            if (_clist == null)
                _clist = new NotifyList<TaskForceData>();
            else
                _clist.Clear();
            if (string.IsNullOrEmpty(tflist))
            {
                CloneFrom(defaultlist);
            }
            else
            {
                foreach (var s in tflist.Split('/'))
                {
                    if (CanInitFromValue(s))
                        Add(s);
                }
            }
            if (_clist.Count == 0)
                Add("");
        }

        protected bool CanInitFromValue(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]+\,[\s\S]*$");
        }

        protected override void Init(IniClass ini, string tag)
        {
            string value;
            if (_clist == null)
                _clist = new NotifyList<TaskForceData>();
            else
                _clist.Clear();
            foreach (string key in ini.GetKeys(tag))
            {
                if (Regex.IsMatch(key, @"^[0-9]+$"))
                {
                    value = ini.ReadValueWithoutNotes(tag, key);
                    if (CanInitFromValue(value))
                    {
                        Add(value);
                    }
                }
            }
            if (_clist.Count == 0)
                Add("");
        }

        /// <summary>
        /// 在此之后插入
        /// </summary>
        /// <param name="index"></param>
        public bool Insert(int index)
        {
            if (_clist.Count >= TaskForce.MaxCount)
                return false;
            _clist.Insert(index + 1, new TaskForceData());
            CheckIndex(index + 1);
            return true;
        }

        /// <summary>
        /// 在此之后插入
        /// </summary>
        /// <param name="data"></param>
        public bool InsertAfter(TaskForceData data)
        {
            int index = _clist.IndexOf(data);
            return Insert(index);
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

        public void Delete(TaskForceData data)
        {
            int index = _clist.IndexOf(data);
            Delete(index);
        }

        protected void CheckIndex(int start = 0)
        {
            for (int j = start; j < _clist.Count; ++j)
                _clist[j].Index = j;
        }

        public virtual int GetTechLevelMax()
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

        protected bool IsTaskForceDataContained(TaskForceData tfd)
        {
            foreach (TaskForceData d in _clist)
            {
                if (d == tfd)
                    return true;
            }
            return false;
        }

        public bool IsUnitContained(string unitname)
        {
            foreach (TaskForceData d in _clist)
            {
                if (d.Name == unitname)
                    return true;
            }
            return false;
        }

        public int ReplaceUnitWith(string srcunit, string destunit)
        {
            int c = 0;
            foreach (TaskForceData d in _clist)
            {
                if (d.Name == srcunit)
                {
                    d.Name = destunit;
                    c++;
                }
            }
            return c;
        }

    }
}
