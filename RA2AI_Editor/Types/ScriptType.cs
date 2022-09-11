using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using Library;

namespace AIcore.Types
{
    public class ScriptType : OType, INotifyPropertyChanged
    {
        public ScriptType(string tag) : base(tag)
        {
            if (tag == null)
                Name = "";
            else
                Name = "_ScriptType_";
            _clist = new ObservableCollection<ScriptTypeData>();
            Add("");
        }

        public ScriptType(IniClass ini, string tag) : base(tag)
        {
            rec_tag = tag;
            NewType = false;
            Init(ini, tag);
        }

        protected override void Init(IniClass ini, string tag)
        {
            PName = ini.ReadValueWithoutNotes(tag, "Name");
            string value;
            if (_clist == null)
                _clist = new ObservableCollection<ScriptTypeData>();
            else
                _clist.Clear();
            foreach (string key in ini.GetKeys(tag))
            {
                if (Regex.IsMatch(key, @"^[0-9]+$"))
                {
                    value = ini.ReadValueWithoutNotes(tag, key);
                    if (Regex.IsMatch(value, @"^[0-9]+\,[0-9]+$"))
                    {
                        Add(value);
                    }
                }
            }
            if (_clist.Count == 0)
                Add("");
        }

        public void Add(int action, string param)
        {
            int i = _clist.Count > 0 ? (_clist[_clist.Count - 1].Index + 1) : 0;
            _clist.Add(new ScriptTypeData() { Index = i, Action = action, Parameter = param });
        }

        public void Add(string value)
        {
            int index = value.IndexOf(',');
            int ikey = _clist.Count > 0 ? (_clist[_clist.Count - 1].Index + 1) : 0;
            
            if (index >= 0)
                _clist.Add(new ScriptTypeData() { Index = ikey, SAction = value.Substring(0, index).Trim(), Parameter = value.Substring(index + 1).Trim() });
            else
                _clist.Add(new ScriptTypeData() { Index = ikey, SAction = value.Trim(), Parameter = "" });
        }

        /// <summary>
        /// 在此之前插入
        /// </summary>
        /// <param name="index"></param>
        public void Insert(int index)
        {
            _clist.Insert(index + 1, new ScriptTypeData());
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

        public ScriptType CloneType(string tag)
        {
            ScriptType t = new ScriptType(tag)
            {
                PName = this.PName
            };
            t._clist.Clear();
            foreach (ScriptTypeData sd in this._clist)
                t.Add(sd.Action, sd.Parameter);
            return t;
        }

        private void CheckIndex(int start = 0)
        {
            for (int j = start; j < _clist.Count; ++j)
                _clist[j].Index = j;
        }

        public override void Output(IniClass ini, bool release)
        {
            ini.WriteValue(_tag, null, null);
            ini.WriteValue(_tag, "Name", Name);
            if (_clist.Count > 1 || _clist[0].Parameter.Length > 0)
            {
                for (int i = 0; i < _clist.Count; ++i)
                {
                    ini.WriteValue(_tag, _clist[i].SIndex, _clist[i].SAction + "," + _clist[i].Parameter);
                }
            }
        }

        public bool CompareWith(ScriptType a)
        {
            if (a.PTag != PTag)
                return false;
            if (a._clist.Count != _clist.Count)
                return false;
            for (int i = 0; i < a._clist.Count; ++i)
            {
                if (a._clist[i].SIndex != _clist[i].SIndex ||
                   a._clist[i].Parameter != _clist[i].Parameter)
                    return false;
            }
            return true;
        }

        public ObservableCollection<ScriptTypeData> _clist;

        public class ScriptTypeData : NotifyClass
        {
            public ScriptTypeData()
            {
                Index = 0;
                Action = 0;
                Parameter = "";
                IsActionValid = true;
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

            public int Action;
            private string _saction;
            public string SAction
            {
                get
                {
                    if (IsActionValid)
                        return ((int)Action).ToString();
                    else
                        return _saction;
                }
                set
                {
                    try
                    {
                        Action = Convert.ToInt32(value);
                        IsActionValid = true;
                        ObservableCollection<ScriptItem.Parameter> p = GetParametersAllowed();
                        if (p != null && p.Count == 1 && p != Scripts.ParamBuildingID && p[0].Param.Length > 0)
                            Parameter = p[0].Param;
                    }
                    catch
                    {
                        IsActionValid = false;
                    }
                    
                    _saction = value;
                    PropertyChange(nameof(SAction));
                    PropertyChange(nameof(SActionName));
                }
            }

            public string SActionName
            {
                get
                {
                    if (IsActionValid)
                        return Scripts.GetSummaryFromAction(Action);
                    else
                        return "";
                }
            }

            public bool IsActionValid { get; private set; }

            private string _param;
            public string Parameter
            {
                get { return _param; }
                set
                {
                    _param = value;
                    PropertyChange(nameof(Parameter));
                    PropertyChange(nameof(ParameterName));
                }
            }

            public string ParameterName
            {
                get
                {
                    if (IsActionValid)
                        return Scripts.GetParamName(Action, _param);
                    else
                        return "";
                }
            }

            public string Description
            {
                get
                {
                    if (IsActionValid)
                        return Scripts.GetDesFromAction(Action);
                    else
                        return "Invalid script action!";
                }
            }

            public ObservableCollection<ScriptItem.Parameter> GetParametersAllowed()
            {
                return IsActionValid ? Scripts.GetParametersAllowedFromAction(Action) : null;
            }

            public ObservableCollection<ScriptItem> ScriptInfo { get { return Scripts.ScriptList; } }

        }
    }
}
