using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using Library;

namespace AIcore.Types
{
    public class AITriggerType : OType
    {
        public AITriggerType(AI _ai, string tag, TeamType teamtype1, TeamType teamtype2 = null) : base(tag)
        {
            ai = _ai;
            Name = "_AITriggerType_";
            TeamType1 = teamtype1;
            TeamType2 = teamtype2;
            if (TeamType1 != AI.NullTeamType)
            {
                TechLevel = TeamType1.GetTechLevelMax();
            }
        }

        public AITriggerType(AI _ai, string tag) : base(tag)
        {
            rec_tag = tag;
            ai = _ai;
            NewType = false;
            Init(ai.ini, tag);
        }

        protected override void Init(IniClass ini, string tag)
        {
            string[] values = ini.ReadValueWithoutNotes("AITriggerTypes", tag).Split(',');
            if (values.Length < 18)
                return;
            PName = values[0].Trim();
            STeamType1 = values[1].Trim();
            SHouse = values[2].Trim();
            TechLevel = GetIntValue(values[3].Trim(), 0);
            TriggerType = TriggerTypes.GetTriggerType(GetIntValue(values[4].Trim(), -1));
            STriggerUnit = values[5].Trim();
            string longstr = values[6].Trim();
            CompareCount = BitConverter.ToUInt32(StringToUInt32Bytes(longstr.Substring(0, 8)), 0);
            SelectedComparison = TriggerTypes.GetCompareInfo((CompareTypes)GetIntValue(longstr.Substring(9, 1), (int)CompareTypes.GreaterOrEqualThan));
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

            // ext
            //TypeExt_EasyMode.TeamType=EasyMode?TeamType1:
        }

        public override void Recover(IniClass ini)
        {
            if (rec_tag != null && rec_tag.Length > 0 && ini.IsKeyExist("AITriggerTypes", rec_tag))
            {
                PTag = rec_tag;
                Init(ini, rec_tag);
            }
        }

        public override void RemoveSectionFromIni(IniClass ini)
        {
            ini.WriteValue("AITriggerTypes", _tag, null);
        }

        public AITriggerType CloneType(string tag)
        {
            return new AITriggerType(ai, tag, TeamType1, TeamType2)
            {
                PName = this.PName,
                SHouse = this.SHouse,
                TechLevel = this.TechLevel,
                ITriggerType = (int)this.TriggerType.Value,
                STriggerUnit = this.STriggerUnit,
                CompareCount = this.CompareCount,
                EComparison = this.SelectedComparison.CompareTypes,
                BaseWeight = this.BaseWeight,
                MinWeight = this.MinWeight,
                MaxWeight = this.MaxWeight,
                Skirmish = this.Skirmish,
                Unknown = this.Unknown,
                ISide = this.Side.Index,
                BaseDefence = this.BaseDefence,
                EasyMode = this.EasyMode,
                NormalMode = this.NormalMode,
                HardMode = this.HardMode
            };
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
                if (TeamType1.EnableExtInRelease || TeamType2.EnableExtInRelease)
                {
                    bool ext_this = false;
                    var ot1 = TeamType1;
                    var ot2 = TeamType2;
                    bool em = EasyMode, mm = NormalMode, hm = HardMode;

                    FromCache(ot1, out (TeamType, TeamType, TeamType) ot1_type);
                    FromCache(ot2, out (TeamType, TeamType, TeamType) ot2_type);

                    if (em)
                    {
                        var tt1_e = ot1_type.Item1;
                        var tt2_e = ot2_type.Item1;

                        ext_this = true;
                        this.TeamType1 = tt1_e;
                        this.TeamType2 = tt2_e;
                        this.EasyMode = true;
                        this.NormalMode = false;
                        this.HardMode = false;
                        OutputStr(ini, this, release, " - E");
                    }
                    if (mm)
                    {
                        var tt1 = ot1_type.Item2;
                        var tt2 = ot2_type.Item2;

                        AITriggerType ext;

                        if (!ext_this)
                        {
                            ext_this = true;
                            this.TeamType1 = tt1;
                            this.TeamType2 = tt2;
                            this.EasyMode = false;
                            this.NormalMode = true;
                            this.HardMode = false;
                            ext = this;
                        }
                        else
                        {
                            ext = this.CloneType(ai.aITriggerTypes.GetNewTag());
                            ext.TeamType1 = tt1;
                            ext.TeamType2 = tt2;
                            ext.EasyMode = false;
                            ext.NormalMode = true;
                            ext.HardMode = false;
                        }
                        OutputStr(ini, ext, release, " - M");
                    }
                    if (hm)
                    {
                        var tt1 = ot1_type.Item3;
                        var tt2 = ot2_type.Item3;

                        AITriggerType ext;

                        if (!ext_this)
                        {
                            ext_this = true;
                            this.TeamType1 = tt1;
                            this.TeamType2 = tt2;
                            this.NormalMode = false;
                            this.EasyMode = false;
                            this.HardMode = true;
                            ext = this;
                        }
                        else
                        {
                            ext = this.CloneType(ai.aITriggerTypes.GetNewTag());
                            ext.TeamType1 = tt1;
                            ext.TeamType2 = tt2;
                            ext.EasyMode = false;
                            ext.NormalMode = false;
                            ext.HardMode = true;
                        }
                        OutputStr(ini, ext, release, " - H");
                    }
                    
                    return;
                }
            }
            OutputStr(ini, this, release);
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
            str += type.triggerType.SValue + ",";
            str += (type.STriggerUnit.Length > 0 ? type.STriggerUnit : type._TriggerUnit.NameOrNone) + ",";
            str += UInt32BytesToString(BitConverter.GetBytes(type.CompareCount)).PadLeft(8, '0');
            str += Convert.ToString((byte)type.SelectedComparison.CompareTypes, 16).PadLeft(2, '0') + "000000000000000000000000000000000000000000000000000000,";
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
        }

        private string B(bool flag)
        {
            return flag ? "1" : "0";
        }

        private byte[] StringToUInt32Bytes(string bytes)
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

        private string UInt32BytesToString(byte[] bytes)
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

        private int GetIntValue(string value, int dfValue)
        {
            int ret;
            try
            {
                ret = Convert.ToInt32(value);
            }
            catch { return dfValue; }
            return ret;
        }

        private double GetDoubleValue(string value, double dfValue)
        {
            double ret;
            try
            {
                ret = Convert.ToDouble(value);
            }
            catch { return dfValue; }
            return ret;
        }

        private bool GetBoolValue(string value, bool dfValue)
        {
            if (value == "1")
                return true;
            if (value == "0")
                return false;
            return dfValue;
        }

        private byte GetByteFromHex(string hex, byte dfValue)
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

        private TriggerType triggerType = TriggerTypes.NullTriggerType;
        public TriggerType TriggerType { get { return triggerType; } set { triggerType = value; PropertyChange(this, "TriggerType"); /*PropertyChange(this, "STriggerCondition");*/ } }
        public int ITriggerType
        {
            set
            {
                TriggerType = TriggerTypes.GetTriggerType(value);
            }
        }
        private Unit _TriggerUnit = Units.NullUnit;
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
                PropertyChange(this, "SelectedComparison");
            }
        }
        public CompareTypes EComparison
        { 
            set
            {
                SelectedComparison = TriggerTypes.GetCompareInfo(value);
            }
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

        private readonly AI ai;

        public AutoCompleteFilterPredicate<object> TeamTypeFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as TeamType)._tag.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
                    || (obj as TeamType).Name.IndexOf(searchText, StringComparison.CurrentCultureIgnoreCase) >= 0;
            }
        }
        public AutoCompleteFilterPredicate<object> TechTypeFilter
        {
            get
            {
                return (searchText, obj) =>
                    (obj as Unit).FuzzyLogic(searchText);
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
        public ObservableCollection<TeamType> TeamTypeInfo
        {
            get { return AI.teamtypes_info; }
            //set { _TeamTypeInfo = value; PropertyChange(this, "TeamTypeInfo"); }
        }
    }
}
