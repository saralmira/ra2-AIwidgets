using AIcore;
using AIcore.Types;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RA2AI_Editor.Data
{
    public abstract class GeneralDataInit<Type> where Type : OType
    {
        public GeneralDataInit(AI _ai)
        {
            ai = _ai;
            del_hint = true;
            fakemode = false;
        }

        protected AI ai;
        protected IniAnalyse iniAnalyse;
        protected bool fakemode;
        protected bool del_hint;

        protected CommandStack GlobalCommandStack { get { return Local.GlobalCommandStack; } }
        public ObservableCollection<Type> ItemList { get; set; }
        protected ObservableCollection<Type> TempList = new ObservableCollection<Type>();

        protected void CopyList<T>(ObservableCollection<T> src, ObservableCollection<T> tar)
        {
            foreach (T t in src)
                tar.Add(t);
        }
        protected void CopyList<T>(List<T> src, ObservableCollection<T> tar)
        {
            foreach (T t in src)
                tar.Add(t);
        }

        public void SetAnalysis(IniAnalyse ana, bool initial = true)
        {
            if (ana == null)
                return;

            if (fakemode)
                ClearAnalysisData();

            iniAnalyse = ana;
            if (initial)
                SwitchMode(true);
        }
        protected void ShowAnalysis(bool show)
        {
            foreach (Type t in ItemList)
                t.ShowCompareResult = show;
        }
        public abstract void SwitchMode(bool fake);
        public void ClearAnalysisData()
        {
            if (iniAnalyse != null)
                iniAnalyse = null;

            if (fakemode)
                SwitchMode(false);

            foreach (Type t in ItemList)
            {
                t.AnalysisResult = AnalysisResult.None;
                t.CompareType = null;
                t.SwitchType = false;
            }
        }

        public Type GetTypeOfCurrent(Type type)
        {
            if (type == null)
                return null;
            foreach (Type t in ItemList)
                if (t._tag == type._tag)
                    return t;
            return null;
        }
        public Type GetTypeOfCurrent(string tag)
        {
            foreach (Type t in ItemList)
                if (t._tag == tag)
                    return t;
            return null;
        }

        public abstract Type Duplicate(Type t, int index = -1);
        public abstract Type Add(int index = -1);
        public abstract void Delete(string tag, bool nohint = false);
        public abstract void Delete(Type taskforce, bool nohint = false);
    }
}
