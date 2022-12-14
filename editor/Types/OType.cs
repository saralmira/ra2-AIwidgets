using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Library;

namespace AIcore.Types
{
    public abstract class TagType : INotifyPropertyChanged
    {
        public string Name = "";
        public string _tag = "<none>";

        public TagType(string tag)
        {
            if (tag != null)
                PTag = tag;
            PropertyChanged += PropertyChangedEvent;
        }

        public string PName
        {
            get { return Name; }
            set
            {
                Name = value;
                PropertyChange(this, "PName");
            }
        }

        public string PTag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                PropertyChange(this, "PTag");
            }
        }

        private bool _ptvisibility = true;
        public bool PTVisibility
        {
            get { return _ptvisibility; }
            set
            {
                _ptvisibility = value;
                PropertyChange(this, "PTVisibility");
            }
        }

        private bool _newtype = true;
        public bool NewType
        {
            get { return _newtype; }
            set
            {
                _newtype = value;
                PropertyChange(this, "NewType");
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

    public abstract class OType : TagType
    {
        public const int MaximumCount = 0xFFFFFF;

        public OType(string tag) : base(tag)
        {
        }

        public abstract void Output(IniClass ini);

        protected abstract void Init(IniClass ini, string tag);

        public virtual void Recover(IniClass ini)
        {
            if (rec_tag != null && rec_tag.Length > 0 && ini.IsSectionExist(rec_tag))
            {
                PTag = rec_tag;
                Init(ini, rec_tag);
            }
        }

        protected string rec_tag;

        private AnalysisResult _AnalysisResult = AnalysisResult.None;
        public AnalysisResult AnalysisResult
        {
            get { if (ShowCompareResult) return _AnalysisResult; return AnalysisResult.None; }
            set { _AnalysisResult = value; PropertyChange(this, "AnalysisResult"); }
        }

        public bool IsOriginalType = true;
        private bool _ShowCompareResult = false;
        public bool ShowCompareResult
        {
            get { return _ShowCompareResult; }
            set { _ShowCompareResult = value; PropertyChange(this, "AnalysisResult"); }
        }
        private bool _SwitchType = false;
        public bool SwitchType
        {
            get { return _SwitchType; }
            set
            {
                if (_SwitchType != value)
                {
                    _SwitchType = value;
                    RA2AI_Editor.MainWindow.SwitchTypeViewEvent?.Invoke(this, value);
                    PropertyChange(this, "SwitchType");
                }
            }
        }
        public OType CompareType = null;

        private bool _HighLight = false;
        public bool HighLight
        {
            get { return _HighLight; }
            set { _HighLight = value; PropertyChange(this, "HighLight"); }
        }
    }
}
