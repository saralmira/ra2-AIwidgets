using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Library;
using RA2AI_Editor;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AIcore.Types
{
    public class AITriggerType : AITriggerTypeBase
    {
        public AITriggerType(AI _ai, string tag, TeamType teamtype1 = null, TeamType teamtype2 = null, bool init_ext = true) : base(tag)
        {
            ai = _ai;
            DerivedBase = false;
            if (teamtype1 == null)
            {
                rec_tag = tag;
                NewType = false;
                Init(ai.ini, tag);
                if (init_ext)
                    InitExt(ai.ini, tag);
            }
            else
            {
                Init(ai.ini, tag);
                if (init_ext)
                    InitExt(ai.ini, tag);
                Name = "_AITriggerType_";
                TeamType1 = teamtype1;
                TeamType2 = teamtype2;
                if (TeamType1 != AI.NullTeamType)
                {
                    SetMaxTechLevel();
                }
            }
        }

        protected override void Init(IniClass ini, string tag)
        {
            string[] values = ini.ReadValueWithoutNotes("AITriggerTypes", tag).Split(',');
            if (values.Length > 17)
            {
                PName = values[0].Trim();
                STeamType1 = values[1].Trim();
                SHouse = values[2].Trim();
                TechLevel = GetIntValue(values[3].Trim(), 0);
                Parse(values, 4);
                BaseWeight = Convert.ToUInt32(GetDoubleValue(values[7].Trim(), 50));
                MinWeight = Convert.ToUInt32(GetDoubleValue(values[8].Trim(), 10));
                MaxWeight = Convert.ToUInt32(GetDoubleValue(values[9].Trim(), 70));
                Skirmish = GetBoolValue(values[10].Trim(), true);
                Unknown = GetBoolValue(values[11].Trim(), false);
                SSide = values[12].Trim();
                BaseDefence = GetBoolValue(values[13].Trim(), false);
                STeamType2 = values[14].Trim();
                EasyMode = GetBoolValue(values[15].Trim(), true);
                NormalMode = GetBoolValue(values[16].Trim(), true);
                HardMode = GetBoolValue(values[17].Trim(), true);
            }
        }

        private void InitExt(IniClass ini, string tag)
        {
            // ext
            string ext_tag = GetExtTag(tag);
            CountryExtList = new NotifyList<CountryExt>();
            EnableExt = ini.ReadBoolValue(ext_tag, nameof(EnableExt), false);
            foreach (Country country in Countries.CountryList)  // without <all>
            {
                CountryExtList.Add(new CountryExt { Country = country, trigger = this });
            }
            Ext_SelectedHouses = ini.ReadValueWithoutNotes(ext_tag, nameof(Ext_SelectedHouses), SHouse);
            Ext_ConditionsWeight = ini.ReadBoolValue(ext_tag, nameof(Ext_ConditionsWeight), false);
            Ext_Conditions = new NotifyList<AITriggerTypeBase>();
            foreach (var s in ini.ReadValueWithoutNotes(ext_tag, nameof(Ext_Conditions)).Split('|'))
            {
                string[] ps = s.Split(',');
                if (ps.Length < 3) continue;
                var tb = new AITriggerTypeBase(null) { childtrigger = this };
                tb.Parse(ps);
                Ext_Conditions.Add(tb);
            }
        }

        public override void Recover(IniClass ini)
        {
            if (rec_tag != null && rec_tag.Length > 0 && ini.IsKeyExist("AITriggerTypes", rec_tag))
            {
                PTag = rec_tag;
                Init(ini, rec_tag);
                InitExt(ini, rec_tag);
            }
        }

        public override void RemoveSectionFromIni(IniClass ini)
        {
            ini.WriteValue("AITriggerTypes", _tag, null);
        }

        public AITriggerType CloneType(string tag)
        {
            var ret = new AITriggerType(ai, tag, TeamType1, TeamType2, false)
            {
                PName = this.PName,
                SHouse = this.SHouse,
                TechLevel = this.TechLevel,
                ITriggerType = (int)this.TriggerType.Value,
                STriggerUnit = this.STriggerUnit,
                CompareCount = this.CompareCount,
                EComparison = this.SelectedComparison.CompareTypes,

                Condition2 = this.Condition2,
                Condition3 = this.Condition3,
                Condition4 = this.Condition4,

                BaseWeight = this.BaseWeight,
                MinWeight = this.MinWeight,
                MaxWeight = this.MaxWeight,
                Skirmish = this.Skirmish,
                Unknown = this.Unknown,
                ISide = this.Side.Index,
                BaseDefence = this.BaseDefence,
                EasyMode = this.EasyMode,
                NormalMode = this.NormalMode,
                HardMode = this.HardMode,

                // ext
                EnableExt = this.EnableExt,
                // CountryExtList = this.CountryExtList,
                _ext_selectedhouses = this._ext_selectedhouses,
                Ext_ConditionsWeight = this.Ext_ConditionsWeight,
                // Ext_Conditions = this.Ext_Conditions
            };

            if (EnableExt)
            {
                ret.CountryExtList = new NotifyList<CountryExt>();
                ret.Ext_Conditions = new NotifyList<AITriggerTypeBase>();
                foreach (var ce in this.CountryExtList)  // without <all>
                {
                    ret.CountryExtList.Add(ce.GetCopy());
                }
                foreach (var cd in this.Ext_Conditions)
                {
                    ret.Ext_Conditions.Add(cd.GetCopy(ret));
                }
            }
            return ret;
        }

        private void FromCache(TeamType t, out (TeamType, TeamType, TeamType) out_type)
        {
            if (t == AI.NullTeamType) out_type = (t, t, t);
            else if (!ai.releaseTeamTypes.TryGetValue(t, out out_type))
            {
                // remove totally
                t.RemoveSectionFromIni(ai.ini);
                ai.teamTypes.Delete(t);
                // if extension is enabled, auto cache
                // if not, cache itself
                // which is really simple
                ai.releaseTeamTypes[t] = out_type = (t.Get_Easy_Type, t.Get_Medium_Type, t.Get_Hard_Type);
                ai.teamTypes.TryAdd(out_type.Item1);
                ai.teamTypes.TryAdd(out_type.Item2);
                ai.teamTypes.TryAdd(out_type.Item3);
            }
        }

        public override void Output(IniClass ini, bool release)
        {
            if (release)
            {
                // ext
                if (EnableExt)
                {
                    var extended_houses = OutputExtendHouses(this);
                    foreach(var ext_house in extended_houses)
                    {
                        var extended_conditions = OutputExtendConditions(ext_house);
                        foreach (var ext_cond in extended_conditions)
                        {
                            OutputFinal(ini, ext_cond);
                        }
                    }
                    return;
                }

                OutputFinal(ini, this);
            }
            OutputStr(ini, this, release);
        }

        private List<AITriggerType> OutputExtendHouses(AITriggerType type)
        {
            List<AITriggerType> ret = new List<AITriggerType>();
            // <all>
            if (type.Ext_SelectedHouses.Contains(Countries.All.NameOrAll))
            {
                type.House = Countries.All;
                ret.Add(type);
                return ret;
            }
            // ext
            bool ext_type = false;
            foreach (var c in type.CountryExtList)
            {
                if (c.IsChecked)
                {
                    AITriggerType ext;
                    if (ext_type)
                    {
                        ext = type.CloneType(ai.aITriggerTypes.GetNewTag());
                    }
                    else
                    {
                        ext_type = true;
                        ext = type;
                    }
                    ext.House = c.Country;
                    ext.Side = c.Country.Side;
                    ret.Add(ext);
                }
            }
            return ret;
        }

        private List<AITriggerType> OutputExtendConditions(AITriggerType type)
        {
            List<AITriggerType> ret = new List<AITriggerType>();
            var initial_weight = Math.Max(type.Ext_ConditionsWeight ? (type.BaseWeight / (uint)(type.Ext_Conditions.Count + 1)) : type.BaseWeight, type.MinWeight);

            // first 
            type.BaseWeight = initial_weight;
            ret.Add(type);

            // ext
            foreach (var cond in type.Ext_Conditions)
            {
                AITriggerType ext = type.CloneType(ai.aITriggerTypes.GetNewTag());
                ext.TriggerType = cond.TriggerType;
                ext.STriggerUnit = cond.STriggerUnit;
                ext.CompareCount = cond.CompareCount;
                ext.Condition2 = cond.Condition2;
                ext.Condition3 = cond.Condition3;
                ext.Condition4 = cond.Condition4;
                ext.SelectedComparison = cond.SelectedComparison;
                type.BaseWeight = initial_weight;
                ret.Add(ext);
            }
            return ret;
        }

        private void OutputFinal(IniClass ini, AITriggerType type)
        {
            if (MainWindow.configData.GenerateTriggersForAllSides 
                && type.House == Countries.All 
                && type.TriggerType.Value == TriggerTypeEnum.SelfCondition
                && (type.SelectedComparison.CompareTypes == CompareTypes.GreaterOrEqualThan || type.SelectedComparison.CompareTypes == CompareTypes.GreaterThan))
            {
                foreach (var side in Sides.SidesList)
                {
                    if (type.Side.Index == side.Index || side.Index == Sides.AllSide.Index)
                        continue;
                    AITriggerType ext = type.CloneType(ai.aITriggerTypes.GetNewTag());
                    ext.Side = side;
                    OutputExtendTeamtype(ini, ext);
                }
            }

            OutputExtendTeamtype(ini, type);
        }

        private void OutputExtendTeamtype(IniClass ini, AITriggerType type)
        {
            if (type.TeamType1.EnableExtInRelease || type.TeamType2.EnableExtInRelease)
            {
                bool ext_this = false;
                var ot1 = type.TeamType1;
                var ot2 = type.TeamType2;
                bool em = type.EasyMode, mm = type.NormalMode, hm = type.HardMode;

                FromCache(ot1, out (TeamType, TeamType, TeamType) ot1_type);
                FromCache(ot2, out (TeamType, TeamType, TeamType) ot2_type);

                if (em)
                {
                    var tt1_e = ot1_type.Item1;
                    var tt2_e = ot2_type.Item1;

                    ext_this = true;
                    type.TeamType1 = tt1_e;
                    type.TeamType2 = tt2_e;
                    type.EasyMode = true;
                    type.NormalMode = false;
                    type.HardMode = false;
                    OutputStr(ini, type, true, " - E");
                }
                if (mm)
                {
                    var tt1 = ot1_type.Item2;
                    var tt2 = ot2_type.Item2;

                    AITriggerType ext;

                    if (!ext_this)
                    {
                        ext_this = true;
                        type.TeamType1 = tt1;
                        type.TeamType2 = tt2;
                        type.EasyMode = false;
                        type.NormalMode = true;
                        type.HardMode = false;
                        ext = type;
                    }
                    else
                    {
                        ext = type.CloneType(ai.aITriggerTypes.GetNewTag());
                        ext.TeamType1 = tt1;
                        ext.TeamType2 = tt2;
                        ext.EasyMode = false;
                        ext.NormalMode = true;
                        ext.HardMode = false;
                    }
                    OutputStr(ini, ext, true, " - M");
                }
                if (hm)
                {
                    var tt1 = ot1_type.Item3;
                    var tt2 = ot2_type.Item3;

                    AITriggerType ext;

                    if (!ext_this)
                    {
                        // ext_this = true;
                        type.TeamType1 = tt1;
                        type.TeamType2 = tt2;
                        type.NormalMode = false;
                        type.EasyMode = false;
                        type.HardMode = true;
                        ext = type;
                    }
                    else
                    {
                        ext = type.CloneType(ai.aITriggerTypes.GetNewTag());
                        ext.TeamType1 = tt1;
                        ext.TeamType2 = tt2;
                        ext.EasyMode = false;
                        ext.NormalMode = false;
                        ext.HardMode = true;
                    }
                    OutputStr(ini, ext, true, " - H");
                }
                return;
            }
            OutputStr(ini, type, true);
        }

        private void OutputStr(IniClass ini, AITriggerType type, bool release, string nameappend = "")
        {
            int index = type.Name.IndexOf(",");
            string name = index >= 0 ? type.Name.Substring(0, index) : type.Name;
            // so you can now use any name, case a letter will automatically be added to the front if necessary.
            // this bug is really critical!!!
            string str = (release ? AutoPrefix(NameWithExt(name, nameappend)) : name) + ",";
            str += type.TeamType1.PTag + ",";
            str += type.House.NameOrAll + ",";
            str += type.TechLevel + ",";
            str += type.ConditionToString() + ",";
            str += Convert.ToString(type.BaseWeight) + ".000000,";
            str += Convert.ToString(type.MinWeight) + ".000000,";
            str += Convert.ToString(type.MaxWeight) + ".000000,";
            str += B(type.Skirmish) + ",";
            str += B(type.Unknown) + ",";
            str += type.Side.SIndex + ",";
            str += B(type.BaseDefence) + ",";
            str += type.TeamType2.PTag + ",";
            str += B(type.EasyMode) + ",";
            str += B(type.NormalMode) + ",";
            str += B(type.HardMode);

            ini.WriteValue("AITriggerTypes", type._tag, str);
            if (ai.IsMapFile)
            {
                ini.WriteValue(AI.AITriggerTypesEnable, type._tag, "yes");
            }

            // ext
            string ext_tag = GetExtTag(type._tag);
            if (!release && EnableExt)
            {
                ini.WriteValue(ext_tag, nameof(EnableExt), EnableExt);
                ini.WriteValue(ext_tag, nameof(Ext_SelectedHouses), Ext_SelectedHouses);
                ini.WriteValue(ext_tag, nameof(Ext_ConditionsWeight), Ext_ConditionsWeight);
                ini.WriteValue(ext_tag, nameof(Ext_Conditions), string.Join("|", Ext_Conditions.Select(s => s.ConditionToString())));
            }
            else
            {
                ini.WriteValue(ext_tag, null, null);
            }
        }

        private string B(bool flag)
        {
            return flag ? "1" : "0";
        }

        public static string GetExtTag(string tag)
        {
            return tag + "-Ext";
        }

        public bool CompareWith(AITriggerType a)
        {
            return a.PTag == PTag && a.STeamType1 == STeamType1 && a.STeamType2 == STeamType2 &&
                a.SHouse == SHouse && a.TechLevel == TechLevel && a.TriggerType.CompareWith(TriggerType) &&
                a.STriggerUnit == STriggerUnit && a.CompareCount == CompareCount &&
                a.BaseWeight == BaseWeight && a.MinWeight == MinWeight && a.MaxWeight == MaxWeight &&
                a.Skirmish == Skirmish && a.Unknown == Unknown && a.Side.CompareWith(Side) &&
                a.BaseDefence == BaseDefence && a.EasyMode == EasyMode && a.NormalMode == NormalMode &&
                a.HardMode == HardMode;
        }

        private TeamType _teamType = AI.NullTeamType;
        public TeamType TeamType1
        {
            get
            {
                return _teamType;
            }
            set
            {
                _teamType = value ?? AI.NullTeamType;
                _STeamType1 = _teamType.PTag;
                PropertyChange(this, "STeamType1");
                PropertyChange(this, "STeamType1Name");
            }
        }
        private string _STeamType1 = AI.NullTeamType.PTag;
        public string STeamType1
        {
            get
            {
                return _STeamType1;
            }
            set
            {
                _STeamType1 = value;
                _teamType = ai == null ? AI.NullTeamType : ai.GetTeamType(_STeamType1);
                PropertyChange(this, "STeamType1");
                PropertyChange(this, "STeamType1Name");
            }
        }
        public string STeamType1Name
        {
            get
            {
                if (TeamType1 != null && TeamType1.Name.Length > 0)
                    return TeamType1.PName;
                return AI.NullTeamType.PTag;
            }
        }

        private Country _house = Countries.All;
        public Country House
        {
            get { return _house; }
            set
            {
                _house = value;
                PropertyChange(this, "House");
                PropertyChange(this, "SHouse");
                PropertyChange(this, "SHouseDes");
            }
        }
        public string SHouse
        {
            get { return House.NameOrAll; }
            set
            {
                House = Countries.GetCountryOrAll(value);
            }
        }
        public string SHouseDes
        {
            get { return House.DescriptionOrAll; }
            set
            {
                House = Countries.GetCountryOrNoneFromDes(value);
            }
        }

        private int _TechLevel = 0;
        public int TechLevel { get { return _TechLevel; } set { _TechLevel = value; PropertyChange(this, "TechLevel"); } }
        public void SetMaxTechLevel()
        {
            TechLevel = Math.Max(TeamType1.GetTechLevelMax(), (TeamType2 == null ? 0 : TeamType2.GetTechLevelMax()));
        }

        private UInt32 _BaseWeight = 50;
        private UInt32 _MinWeight = 10;
        private UInt32 _MaxWeight = 70;
        public UInt32 BaseWeight { get { return _BaseWeight; } set { _BaseWeight = value;PropertyChange(this, "BaseWeight"); } }
        public UInt32 MinWeight { get { return _MinWeight; } set { _MinWeight = value; PropertyChange(this, "MinWeight"); } }
        public UInt32 MaxWeight { get { return _MaxWeight; } set { _MaxWeight = value; PropertyChange(this, "MaxWeight"); } }

        private bool _Skirmish = true;
        public bool Skirmish { get { return _Skirmish; } set { _Skirmish = value; PropertyChange(this, "Skirmish"); } }
        private bool _Unknown = false;
        public bool Unknown { get { return _Unknown; } set { _Unknown = value; PropertyChange(this, "Unknown"); } }
        private Side _side = Sides.AllSide;
        public Side Side
        {
            get { return _side; }
            set
            {
                _side = value;
                PropertyChange(this, "Side");
                PropertyChange(this, "SideDes");
            }
        }
        public int ISide
        {
            set
            {
                Side = Sides.GetSideFromIndex(value);
            }
        }
        public string SSide
        {
            set
            {
                Side = Sides.GetSideFromIndex(value);
            }
        }
        public string SideDes
        {
            get
            {
                if (_side == null)
                    return "";
                return _side.Description;
            }
        }
        private bool _BaseDefence = false;
        public bool BaseDefence { get { return _BaseDefence; } set { _BaseDefence = value; PropertyChange(this, "BaseDefence"); } }
        private TeamType _teamType2 = AI.NullTeamType;
        public TeamType TeamType2
        {
            get
            {
                return _teamType2;
            }
            set
            {
                _teamType2 = value ?? AI.NullTeamType;
                _STeamType2 = _teamType2.PTag;
                PropertyChange(this, "STeamType2");
                PropertyChange(this, "STeamType2Name");
            }
        }
        private string _STeamType2 = AI.NullTeamType.PTag;
        public string STeamType2
        {
            get
            {
                return _STeamType2;
            }
            set
            {
                _STeamType2 = value;
                _teamType2 = ai == null ? AI.NullTeamType : ai.GetTeamType(_STeamType2);
                PropertyChange(this, "STeamType2");
                PropertyChange(this, "STeamType2Name");
            }
        }
        public string STeamType2Name
        {
            get
            {
                if (TeamType2 != null && TeamType2.Name.Length > 0)
                    return TeamType2.PName;
                return AI.NullTeamType.PTag;
            }
        }

        private bool _EasyMode = true;
        private bool _NormalMode = true;
        private bool _HardMode = true;
        public bool EasyMode { get => _EasyMode; set { _EasyMode = value; PropertyChange(nameof(EasyMode)); } }
        public bool NormalMode { get => _NormalMode; set { _NormalMode = value; PropertyChange(nameof(NormalMode)); } }
        public bool HardMode { get => _HardMode; set { _HardMode = value; PropertyChange(nameof(HardMode)); } }

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

        private string _ext_selectedhouses;
        public string Ext_SelectedHouses
        {
            get { return _ext_selectedhouses; }
            set { 
                _ext_selectedhouses = value;
                Ext_UpdateSelectedHouses(_ext_selectedhouses);
                PropertyChange(nameof(Ext_SelectedHouses)); 
            }
        }
        public string Ext_SelectedHousesUpdate
        {
            set
            {
                _ext_selectedhouses = value;
                PropertyChange(nameof(Ext_SelectedHouses));
            }
        }
        public string Ext_GetSelectedHouses()
        {
            List<string> selected = new List<string>();
            foreach (var c in CountryExtList)
            {
                if (c.IsChecked)
                    selected.Add(c.Country.NameOrAll);
            }
            return selected.Count == CountryExtList.Count ? Countries.All.NameOrAll :
                (selected.Count == 0 ? Countries.All.NameOrNone : string.Join(",", selected));
        }
        public void Ext_UpdateSelectedHouses(string str)
        {
            var ext_countrylist = str.Split(',');
            bool all = ext_countrylist.Contains(Countries.All.NameOrAll);
            foreach (var country in CountryExtList)  // without <all>
            {
                country.IsChecked = all || ext_countrylist.Contains(country.Country.NameOrAll);
            }
        }

        public bool Ext_ConditionsWeight { get; set; }

        public NotifyList<AITriggerTypeBase> Ext_Conditions { get; set; }

        protected readonly AI ai;

        public class CountryExt : NotifyClass
        {
            private Country _country;
            public Country Country { get { return _country; } set { _country = value; PropertyChange(nameof(Country)); } }

            private bool _ischecked;
            public bool IsChecked { get { return _ischecked; } set { _ischecked = value; PropertyChange(nameof(IsChecked)); trigger.Ext_SelectedHousesUpdate = trigger.Ext_GetSelectedHouses(); } }

            public AITriggerType trigger;

            public CountryExt GetCopy()
            {
                var ce = new CountryExt
                {
                    _country = this.Country,
                    _ischecked = this.IsChecked,
                    trigger = this.trigger
                };
                return ce;
            }
        }

        public NotifyList<CountryExt> CountryExtList { get; set; }

        public AutoCompleteFilterPredicate<object> TeamTypeFilter
        {
            get
            {
                return (searchText, obj) =>
                    !string.IsNullOrEmpty(searchText) && (
                    (obj as TeamType)._tag.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || (obj as TeamType).Name.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0);
            }
        }
        
        public ObservableCollection<Country> CountryListInfo
        {
            get { return Countries.CountryListWithNone; }
            //set { _CountryListInfo = value; PropertyChange(this, "CountryListInfo"); }
        }
        public ObservableCollection<Side> SideListInfo
        {
            get { return Sides.SidesList; }
            //set { _SideListInfo = value; PropertyChange(this, "SideListInfo"); }
        }
        public ObservableCollection<TeamType> TeamTypeInfo
        {
            get { return AI.teamtypes_info; }
            //set { _TeamTypeInfo = value; PropertyChange(this, "TeamTypeInfo"); }
        }
    }

    public class AITriggerTypeBase : OType
    {
        public AITriggerTypeBase(string tag) : base(tag) { DerivedBase = true; }

        private TriggerType triggerType = TriggerTypes.NullTriggerType;
        public TriggerType TriggerType { get { return triggerType; } set { triggerType = value; PropertyChange(this, "TriggerType"); /*PropertyChange(this, "STriggerCondition");*/ } }
        public int ITriggerType
        {
            set
            {
                TriggerType = TriggerTypes.GetTriggerType(value);
            }
        }
        protected Unit _TriggerUnit = Units.NullUnit;
        private string _STriggerUnit = Units.NullUnit.NameOrNone;
        public string STriggerUnit
        {
            get
            {
                return _STriggerUnit;
            }
            set
            {
                _STriggerUnit = value;
                _TriggerUnit = Units.FindUnitFromAll(value);
                PropertyChange(this, "STriggerUnit");
                PropertyChange(this, "STriggerUnitName");
            }
        }
        public string STriggerUnitName
        {
            get
            {
                if (STriggerUnit.Length == 0)
                    return Units.NullUnit.NameOrNone;
                if (_TriggerUnit != null && _TriggerUnit != Units.NullUnit)
                    return _TriggerUnit.Description;
                return STriggerUnit;
            }
        }
        private UInt32 _CompareCount = 0;
        public UInt32 CompareCount { get { return _CompareCount; } set { _CompareCount = value; PropertyChange(this, "CompareCount"); } }
        private CompareInfoClass _SelectedComparison = TriggerTypes.DefaultCompareType;
        public CompareInfoClass SelectedComparison
        {
            get
            {
                return _SelectedComparison;
            }
            set
            {
                _SelectedComparison = value;
                PropertyChange(this, nameof(SelectedComparison));
            }
        }
        public CompareTypes EComparison
        {
            set
            {
                SelectedComparison = TriggerTypes.GetCompareInfo(value);
            }
        }

        public struct AITriggerConditionComparator
        {
            public int ComparatorType;
            public int ComparatorOperand;
        };

        public AITriggerConditionComparator Condition2 = new AITriggerConditionComparator { ComparatorType = 0, ComparatorOperand = 0 };
        public AITriggerConditionComparator Condition3 = new AITriggerConditionComparator { ComparatorType = 0, ComparatorOperand = 0 };
        public AITriggerConditionComparator Condition4 = new AITriggerConditionComparator { ComparatorType = 0, ComparatorOperand = 0 };

        public int ConditionType2 { get { return Condition2.ComparatorType; } set { Condition2.ComparatorType = value; PropertyChange(nameof(ConditionType2)); } }
        public int ConditionOperand2 { get { return Condition2.ComparatorOperand; } set { Condition2.ComparatorOperand = value; PropertyChange(nameof(ConditionOperand2)); } }
        public int ConditionType3 { get { return Condition3.ComparatorType; } set { Condition3.ComparatorType = value; PropertyChange(nameof(ConditionType3)); } }
        public int ConditionOperand3 { get { return Condition3.ComparatorOperand; } set { Condition3.ComparatorOperand = value; PropertyChange(nameof(ConditionOperand3)); } }
        public int ConditionType4 { get { return Condition4.ComparatorType; } set { Condition4.ComparatorType = value; PropertyChange(nameof(ConditionType4)); } }
        public int ConditionOperand4 { get { return Condition4.ComparatorOperand; } set { Condition4.ComparatorOperand = value; PropertyChange(nameof(ConditionOperand4)); } }

        public override void Output(IniClass ini, bool release = false)
        {
            throw new NotImplementedException();
        }

        protected override void Init(IniClass ini, string tag)
        {
            throw new NotImplementedException();
        }

        private bool _derivedbase;
        public bool DerivedBase { get { return _derivedbase; } set { _derivedbase = value; PropertyChange(nameof(DerivedBase)); } }
        public AITriggerType childtrigger;

        public (AITriggerTypeBase, int) Ext_ConditionsAdd()
        {
            var tb = new AITriggerTypeBase(null);
            int id = 1;
            if (this is AITriggerType at)
            {
                tb.childtrigger = at;
                at.Ext_Conditions.Insert(0, tb);
            }
            else if (DerivedBase)
            {
                tb.childtrigger = childtrigger;
                int index = childtrigger.Ext_Conditions.IndexOf(this);
                childtrigger.Ext_Conditions.Insert(index + 1, tb);
                id = index + 2;
            }
            return (tb, id);
        }

        public void Ext_ConditionsDelete(AITriggerTypeBase tb)
        {
            if (DerivedBase)
            {
                childtrigger.Ext_Conditions.Remove(tb);
            }
        }

        public string ConditionToString()
        {
            string str = "";
            str += TriggerType.SValue + ",";
            str += (STriggerUnit.Length > 0 ? STriggerUnit : _TriggerUnit.NameOrNone) + ",";
            str += UInt32BytesToString(BitConverter.GetBytes(CompareCount)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes((int)SelectedComparison.CompareTypes)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes(Condition2.ComparatorType)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes(Condition2.ComparatorOperand)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes(Condition3.ComparatorType)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes(Condition3.ComparatorOperand)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes(Condition4.ComparatorType)).PadLeft(8, '0');
            str += UInt32BytesToString(BitConverter.GetBytes(Condition4.ComparatorOperand)).PadLeft(8, '0');
            return str;
        }

        public void Parse(string[] strs, int startindex = 0)
        {
            if (strs != null && strs.Length > 2)
            {
                TriggerType = TriggerTypes.GetTriggerType(GetIntValue(strs[startindex].Trim(), -1));
                STriggerUnit = strs[startindex + 1].Trim();
                string longstr = strs[startindex + 2].Trim();
                CompareCount = BitConverter.ToUInt32(StringToUInt32Bytes(longstr.Substring(0, 8)), 0);
                SelectedComparison = TriggerTypes.GetCompareInfo((CompareTypes)BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(8, 8)), 0));
                Condition2.ComparatorType = BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(16, 8)), 0);
                Condition2.ComparatorOperand = BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(24, 8)), 0);
                Condition3.ComparatorType = BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(32, 8)), 0);
                Condition3.ComparatorOperand = BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(40, 8)), 0);
                Condition4.ComparatorType = BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(48, 8)), 0);
                Condition4.ComparatorOperand = BitConverter.ToInt32(StringToUInt32Bytes(longstr.Substring(56, 8)), 0);
            }
        }

        public AITriggerTypeBase GetCopy(AITriggerType child_trigger = null)
        {
            return new AITriggerTypeBase(null)
            {
                childtrigger = child_trigger,
                ITriggerType = (int)this.TriggerType.Value,
                STriggerUnit = this.STriggerUnit,
                CompareCount = this.CompareCount,
                Condition2 = this.Condition2,
                Condition3 = this.Condition3,
                Condition4 = this.Condition4,
                EComparison = this.SelectedComparison.CompareTypes
            };
        }

        public AutoCompleteFilterPredicate<object> TechTypeFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Unit).FuzzyLogic(searchText);
            }
        }
        public ObservableCollection<TriggerType> TriggerTypeListInfo
        {
            get { return TriggerTypes.TriggerTypeList; }
            //set { _TriggerTypeListInfo = value; PropertyChange(this, "TriggerTypeListInfo"); }
        }
        public ObservableCollection<CompareInfoClass> CompareTypeListInfo
        {
            get { return TriggerTypes.CompareTypeList; }
            //set { _CompareTypeListInfo = value; PropertyChange(this, "CompareTypeListInfo"); }
        }
        public ObservableCollection<Unit> TechTypeInfo
        {
            get { return Units.AllList; }
            //set { _TechTypeInfo = value; PropertyChange(this, "TechTypeInfo"); }
        }

        protected byte[] StringToUInt32Bytes(string bytes)
        {
            if (bytes.Length == 8)
            {
                byte[] ret = new byte[4];
                for (int i = 0; i < ret.Length; ++i)
                    ret[i] = Convert.ToByte(bytes.Substring(i * 2, 2), 16);
                return ret;
            }
            return null;
        }

        protected string UInt32BytesToString(byte[] bytes)
        {
            if (bytes.Length == 4)
            {
                string ret = "";
                for (int i = 0; i < bytes.Length; ++i)
                    ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
                return ret.ToUpper();
            }
            return null;
        }

        protected int GetIntValue(string value, int dfValue)
        {
            int ret;
            try
            {
                ret = Convert.ToInt32(value);
            }
            catch { return dfValue; }
            return ret;
        }

        protected double GetDoubleValue(string value, double dfValue)
        {
            double ret;
            try
            {
                ret = Convert.ToDouble(value);
            }
            catch { return dfValue; }
            return ret;
        }

        protected bool GetBoolValue(string value, bool dfValue)
        {
            if (value == "1")
                return true;
            if (value == "0")
                return false;
            return dfValue;
        }

        protected byte GetByteFromHex(string hex, byte dfValue)
        {
            try
            {
                return Convert.ToByte(hex, 16);
            }
            catch
            {
                return dfValue;
            }
        }

    }
}
