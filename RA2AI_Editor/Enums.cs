using System;
using Library;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.Windows;
using RA2AI_Editor;
using System.Collections.Generic;

namespace AIcore
{
    public enum AnalysisResult
    {
        None,
        Same,
        Different,
        OnlyInSource,
        OnlyInTarget
    }

    public enum UnitType
    {
        InfantryType,
        VehicleType,
        AircraftType,
        BuildingType
    }

    public enum TargetChooseMode
    {
        MinThreat = 0,
        MaxThreat = 65536,
        MinDistance = 131072,
        MaxDistance = 196608
    }

    public enum CompareTypes
    {
        SmallerThan = 0,
        SmallerOrEqualThan,
        EqualThan,
        GreaterOrEqualThan,
        GreaterThan,
        UnequalThan
    }

    //public enum Sides
    //{
    //    GDI = 1,
    //    Nod,
    //    ThirdSide,
    //    ForthSide,
    //
    //    Civilian,
    //    Mutant
    //}

    public enum TriggerTypeEnum
    {
        None = -1,
        EnermyCondition = 0,
        SelfCondition,
        EnermyPower,
        EnermyLackofPower,
        EnermyBonusCondition,
        IronCurtainReady,
        ChronoSphereReady,
        NeutralCondition
    }

    public class TriggerTypes
    {
        public static void Load(string folder = null)
        {
            LoadCompareInfo(folder);
            LoadAITriggerInfo(folder);
            Utils.InitList(ref TriggerTypeList);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\triggertypes.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\triggertypes.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("TriggerTypes/TriggerType");
                foreach (XmlNode xn in xnList)
                {
                    XmlElement ele = (XmlElement)xn;
                    TriggerType tt = new TriggerType
                    {
                        SValue = ele.GetAttribute("Value").Trim(),
                        Name = ele.GetAttribute("Name").Trim(),
                        Description = ele.InnerText.Trim()
                    };
                    TriggerTypeList.Add(tt);
                }
                NullTriggerType = GetTriggerType((int)TriggerTypeEnum.None);
            }
            catch (Exception e)
            {
                MessageBox.Show("TriggerTypes: " + e.Message);
            }
        }

        private static void LoadCompareInfo(string folder = null)
        {
            Utils.InitList(ref CompareTypeList);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\comparetypesinfo.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\comparetypesinfo.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("CompareTypes/CompareType");
                foreach (XmlNode xn in xnList)
                {
                    XmlElement ele = (XmlElement)xn;
                    CompareInfoClass cc = new CompareInfoClass
                    {
                        SCompareTypes = ele.GetAttribute("Value").Trim(),
                        Description = ele.InnerText.Trim()
                    };
                    CompareTypeList.Add(cc);
                }
                if (CompareTypeList.Count > 0)
                    DefaultCompareType = GetCompareInfo(CompareTypes.GreaterOrEqualThan);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private static void LoadAITriggerInfo(string folder = null)
        {
            Utils.InitList(ref AITriggerInfoList);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\aitriggertypeinfo.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\aitriggertypeinfo.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("AITriggerInfo/Item");
                foreach (XmlNode xn in xnList)
                {
                    XmlElement ele = (XmlElement)xn;
                    InfoValueClass iv = new InfoValueClass
                    {
                        Value = ele.GetAttribute("Name").Trim(),
                        Description = ele.InnerText.Trim()
                    };
                    AITriggerInfoList.Add(iv);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public static string GetToolTip(string name)
        {
            foreach (InfoValueClass iv in AITriggerInfoList)
            {
                if (iv.Value == name)
                    return iv.Description;
            }
            return "";
        }

        public static TriggerType GetTriggerType(int value)
        {
            foreach (TriggerType t in TriggerTypeList)
                if ((int)t.Value == value)
                    return t;
            return NullTriggerType;
        }

        public static TriggerType GetTriggerTypeFromDes(string Description)
        {
            foreach (TriggerType t in TriggerTypeList)
                if (t.Description == Description)
                    return t;
            foreach (TriggerType t in TriggerTypeList)
                if (t.SValue == Description)
                    return t;
            return NullTriggerType;
        }

        public static CompareInfoClass GetCompareInfo(CompareTypes ct)
        {
            foreach (CompareInfoClass ci in CompareTypeList)
                if (ci.CompareTypes == ct)
                    return ci;
            return null;
        }

        public static TriggerType NullTriggerType;
        public static CompareInfoClass DefaultCompareType;
        public static ObservableCollection<TriggerType> TriggerTypeList;
        public static ObservableCollection<CompareInfoClass> CompareTypeList;
        public static ObservableCollection<InfoValueClass> AITriggerInfoList;
    }

    public class WaypointClass
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public int Waypoint { get; set; }
        public string SWaypointFormat
        {
            get
            {
                if (IsValidWaypoint())
                {
                    if (Waypoint > 25)
                        return CHARS[Waypoint / 26 - 1].ToString() + CHARS[Waypoint % 26].ToString();
                    else
                        return CHARS[Waypoint].ToString();
                }
                else
                    return null;
            }
            set
            {
                value = value.ToUpper();
                switch (value.Length)
                {
                    case 1:
                        Waypoint = CHARS.IndexOf(value[0]);
                        break;
                    case 2:
                        int i1 = CHARS.IndexOf(value[1]);
                        int i2 = CHARS.IndexOf(value[0]);
                        if (i1 >= 0 && i2 >= 0)
                            Waypoint = i1 + (i2 + 1) * 26;
                        else
                            Waypoint = -1;
                        break;
                    default:
                        Waypoint = -1;
                        break;
                }
            }
        }
        public string SWaypointFormatTransport
        {
            set
            {
                value = value.ToUpper();
                switch (value.Length)
                {
                    case 1:
                        Waypoint = CHARS.IndexOf(value[0]);
                        break;
                    case 2:
                        int i1 = CHARS.IndexOf(value[1]);
                        int i2 = CHARS.IndexOf(value[0]);
                        if (i1 >= 0 && i2 >= 0)
                            Waypoint = i1 + (i2 + 1) * 26;
                        else
                        {
                            Waypoint = -1;
                            _SWaypoint = AI.DoNotUseTransportOrigin;
                        }
                        break;
                    default:
                        Waypoint = -1;
                        _SWaypoint = AI.DoNotUseTransportOrigin;
                        break;
                }
            }
        }
        private string _SWaypoint;
        public string SWaypoint
        {
            get { if (IsValidWaypoint()) return Waypoint.ToString(); return _SWaypoint; }
            set { _SWaypoint = value; try { Waypoint = Convert.ToInt32(value); } catch { Waypoint = -1; } }
        }

        protected bool IsValidWaypoint(int value)
        {
            return value >= 0 && value <= 701;
        }

        protected bool IsValidWaypoint()
        {
            return Waypoint >= 0 && Waypoint <= 701;
        }

        public bool CompareWith(WaypointClass a)
        {
            return a.Waypoint == Waypoint;
        }

        public static WaypointClass InvalidWaypoint = new WaypointClass { Waypoint = -1 };
    }

    public class VeteranLevelInfo
    {
        public static void Load(string folder = null)
        {
            Utils.InitList(ref veteranLevels);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\veteranlevel.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\veteranlevel.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("VeteranLevel/Level");
                foreach (XmlNode xn in xnList)
                {
                    if (xn.Name == "Level")
                    {
                        XmlElement ele = (XmlElement)xn;
                        InfoValueClass tt = new InfoValueClass() { Value = ele.GetAttribute("Value").Trim(), Description = ele.InnerText.Trim() };
                        veteranLevels.Add(tt);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("VeteranLevel: " + e.Message);
            }
        }

        public static ObservableCollection<InfoValueClass> veteranLevels;
    }

    public class TeamTypeInfo
    {
        public static void Load(string folder = null)
        {
            Utils.InitList(ref teamTypeInfos);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\teamtypeinfo.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\teamtypeinfo.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("TeamTypeInfo/Item");
                foreach (XmlNode xn in xnList)
                {
                    if (xn.Name == "Item")
                    {
                        XmlElement ele = (XmlElement)xn;
                        TeamType tt = new TeamType()
                        {
                            Value = ele.GetAttribute("Default").Trim(),
                            Description = ele.InnerText.Trim(),
                            IsEnabled = GetBoolValue(ele.GetAttribute("IsEnabled").Trim(), true),
                            Name = ele.GetAttribute("Name").Trim(),
                            IsUsedInMapOnly = GetBoolValue(ele.GetAttribute("IsUsedInMapOnly").Trim(), false),
                            Summary = ele.GetAttribute("Summary").Trim()
                        };
                        teamTypeInfos.Add(tt);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("TeamTypeInfo: " + e.Message);
            }
        }

        private static bool GetBoolValue(string value, bool def)
        {
            try
            {
                return Convert.ToBoolean(value);
            }
            catch
            {
                return def;
            }
        }

        public static string GetToolTip(string name)
        {
            foreach(TeamType tt in teamTypeInfos)
            {
                if (tt.Name == name)
                    return tt.Description;
            }
            return "";
        }

        public static TeamType GetTeamTypeInfo(string name)
        {
            foreach (TeamType tt in teamTypeInfos)
            {
                if (tt.Name == name)
                    return tt;
            }
            return NullTeamTypeInfo;
        }

        public static ObservableCollection<TeamType> teamTypeInfos;

        public class TeamType : InfoValueClass
        {
            public string Name { get; set; }
            public bool IsUsedInMapOnly { get; set; }
            public string Summary { get; set; }
            public bool IsEnabled { get; set; }
        }

        private static readonly TeamType NullTeamTypeInfo = new TeamType() { IsEnabled = false, IsUsedInMapOnly = false };
    }

    public class MindControlDecision
    {
        public static void Load(string folder = null)
        {
            Utils.InitList(ref MindControlDecisionInfo);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\mindcontroldecisions.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\mindcontroldecisions.xml";
            if (!File.Exists(folder))
                return;

            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("MindControlDecision/Item");
                foreach (XmlNode xn in xnList)
                {
                    if (xn.Name == "Item")
                    {
                        XmlElement ele = (XmlElement)xn;
                        InfoValueClass tt = new InfoValueClass() { Value = ele.GetAttribute("Value").Trim(), Description = ele.InnerText.Trim() };
                        MindControlDecisionInfo.Add(tt);
                    }
                }
                if (MindControlDecisionInfo.Count > 0)
                    DefaultMindControlDecision = MindControlDecisionInfo[0];
            }
            catch (Exception e)
            {
                MessageBox.Show("MindControlDecision: " + e.Message);
            }
        }

        public static InfoValueClass GetMindControlDecision(string value)
        {
            foreach (InfoValueClass c in MindControlDecisionInfo)
                if (c.Value == value)
                    return c;
            return null;
        }

        public static InfoValueClass GetMindControlDecision(int value)
        {
            foreach (InfoValueClass c in MindControlDecisionInfo)
                if (Convert.ToInt32(c.Value) == value)
                    return c;
            return null;
        }

        public static ObservableCollection<InfoValueClass> MindControlDecisionInfo;

        public static InfoValueClass DefaultMindControlDecision;
    }

    public class GroupInfo
    {
        public static void Load(string folder = null)
        {
            Utils.InitList(ref Group);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\groupinfo.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\groupinfo.xml";
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(folder);
                XmlNodeList xnList = xmlDoc.SelectNodes("Group/Item");
                foreach (XmlNode xn in xnList)
                {
                    if (xn.Name == "Item")
                    {
                        XmlElement ele = (XmlElement)xn;
                        InfoValueClass tt = new InfoValueClass() { Value = ele.GetAttribute("Value").Trim(), Description = ele.InnerText.Trim() };
                        Group.Add(tt);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("GroupInfo: " + e.Message);
            }
        }

        public static bool FuzzyLogic(InfoValueClass g, string search)
        {
            return g.Value.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        || g.Description.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public static InfoValueClass FindGroupWithFuzzyLogic(string str)
        {
            foreach (InfoValueClass g in Group)
                if (FuzzyLogic(g, str))
                    return g;
            return Group.Count > 0 ? Group[0] : null;
        }

        public static ObservableCollection<InfoValueClass> Group;
    }

}
