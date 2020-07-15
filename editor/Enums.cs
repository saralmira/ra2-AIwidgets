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

    public enum ScriptAction
    {
        Attack = 0,

        AttackWaypoint = 1,
        GoBerzerk = 2,
        MoveToWaypoint = 3,
        MoveToCell = 4,

        Guard = 5,
        JumpToLine = 6,

        PlayerWins = 7,

        Unload = 8,
        Deploy = 9,
        FollowFriendlies = 10,
        DoThis,
        SetGlobal,
        IdleAnim,
        LoadOntoTransport,
        SpyOnBldg,
        PatrolToWaypoint,
        ChangeScript,
        ChangeTeam,
        Panic,
        ChangeHouse,
        Scatter,
        GotoNearbyShroud,
        PlayerLoses,
        PlaySpeech,
        PlaySound,
        PlayMovie,
        PlayMusic,
        ReduceTiberium,
        BeginProduction,
        FireSale,
        SelfDestruct,
        IonStormStartIn,
        IonStornEnd,
        CenterViewOnTeam,
        ReshroudMap,
        RevealMap,
        DeleteTeamMembers,
        ClearGlobal,
        SetLocal,
        ClearLocal,
        Unpanic,
        ForceFacing,
        WaitTillFullyLoaded,
        TruckUnload,
        TruckLoad,
        AttackEnemyBuilding,
        MovetoEnemyBuilding,
        Scout,
        Success,
        Flash,
        PlayAnim,
        TalkBubble,
        GatherAtEnemy,
        GatherAtBase,
        IronCurtainMe,
        ChronoPrepforABwP,
        ChronoPrepforAQ,
        MoveToOwnBuilding,
        //AttackBuildingAtWaypoint,

        SendToGrinder = 60,
        SendToTankBunker,
        SendToBioReactor,
        SendToBunker,
        MoveToNearestCivilBuilding
    }

    public class Sides
    {
        public static void Load(string folder = null)
        {
            Utils.InitList(ref SidesList, AllSide);

            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\sides.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\sides.xml";

            xmlpath = folder;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlpath);
                XmlNodeList xnList = xmlDoc.SelectNodes("Sides/Side");
                foreach (XmlNode xn in xnList)
                {
                    XmlElement ele = (XmlElement)xn;
                    Side s = new Side
                    {
                        SIndex = ele.GetAttribute("Value").Trim(),
                        Name = ele.GetAttribute("Name").Trim(),
                        IsEnabled = Convert.ToBoolean(ele.GetAttribute("IsEnabled").Trim()),
                        Description = ele.InnerText.Trim()
                    };
                    if (s.IsEnabled)
                        SidesList.Add(s);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Sides: " + e.Message);
            }
        }

        public static void Update(ObservableCollection<Side> sidelist)
        {
            SaveFlag = true;
            Save(sidelist);
            SidesList.Clear();
            SidesList.Add(AllSide);
            foreach(Side s in sidelist)
                if (s.IsEnabled)
                    SidesList.Add(s);
        }

        public static void Save(ObservableCollection<Side> savelist = null)
        {
            if (!SaveFlag)
                return;
            SaveFlag = false;

            XmlDocument xmlDoc = XmlClass.CreateNew(xmlpath);
            XmlElement root = xmlDoc.CreateElement("Sides");
            foreach (Side s in (savelist ?? SidesList))
            {
                if (s == AllSide)
                    continue;
                XmlElement elec = xmlDoc.CreateElement("Side");
                elec.SetAttribute("Name", s.Name);
                elec.SetAttribute("Value", s.SIndex);
                elec.SetAttribute("IsEnabled", s.IsEnabled.ToString());
                elec.InnerText = s.Description;
                root.AppendChild(elec);
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(xmlpath);
        }
        
        public static Side GetSideFromDes(string description)
        {
            if (SidesList != null)
            {
                foreach (Side s in SidesList)
                    if (s.Description == description)
                        return s;
            }
            return AllSide;
        }

        public static Side GetSide(string name)
        {
            if (SidesList != null)
            {
                foreach (Side s in SidesList)
                    if (s.Name == name)
                        return s;
            }
            return AllSide;
        }

        public static Side GetSide(int index)
        {
            if (SidesList != null)
            {
                foreach (Side s in SidesList)
                    if (s.Index == index)
                        return s;
            }
            return AllSide;
        }

        public static Side GetSide(string name, ObservableCollection<Side> sidelist)
        {
            if (sidelist != null)
            {
                foreach (Side s in sidelist)
                    if (s.Name == name)
                        return s;
            }
            Side ret = new Side();
            sidelist.Add(ret);
            return ret;
        }

        public static Side GetSideFromIndex(int index)
        {
            if (SidesList != null)
            {
                foreach (Side s in SidesList)
                    if (s.Index == index)
                        return s;
            }
            return AllSide;
        }

        public static Side GetSideFromIndex(string sindex)
        {
            if (SidesList != null)
            {
                foreach (Side s in SidesList)
                    if (s.SIndex == sindex)
                        return s;
            }
            return AllSide;
        }

        private static bool SaveFlag = false;
        private static string xmlpath;
        public static ObservableCollection<Side> SidesList;
        public static Side AllSide = new Side() { IsEnabled = true, Index = 0, Name = "<all>", Description = "<all>" };
    }

    public class Countries
    {
        public static void Load(string folder = null)
        {
            All = new Country();
            Utils.InitList(ref MapCountryList);
            Utils.InitList(ref OrgCountryList);
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\countries.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\countries.xml";

            xmlpath = folder;
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlpath);
                XmlNodeList xnList = xmlDoc.SelectNodes("Countries/Country");
                foreach (XmlNode xn in xnList)
                {
                    XmlElement ele = (XmlElement)xn;
                    Country con = new Country
                    {
                        SIndex = ele.GetAttribute("Value").Trim(),
                        Name = ele.GetAttribute("Name").Trim(),
                        SSide = ele.GetAttribute("Side").Trim(),
                        UIName = ele.GetAttribute("UIName").Trim(),
                        IsEnabled = Convert.ToBoolean(ele.GetAttribute("IsEnabled").Trim()),
                        Description = ele.InnerText.Trim()
                    };
                    OrgCountryList.Add(con);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Countries: " + e.Message);
            }
            GenCountryList();
        }

        public static void LoadMapCountries(IniClass map)
        {
            Utils.InitList(ref MapCountryList);
            if (map == null)
                return;

            IList<string> clist = map.GetValuesWithoutNotes("Countries");
            for (int i = 0; i < clist.Count; ++i)
            {
                MapCountryList.Add(new Country()
                {
                    Index = i,
                    Name = clist[i],
                    UIName = map.ReadValueWithoutNotes(clist[i], "UIName"),
                    Description = map.ReadValueWithoutNotes(clist[i], "Name"),
                    Side = Sides.GetSide(map.ReadValueWithoutNotes(clist[i], "Side")),
                    IsEnabled = map.ReadBoolValue(clist[i], "Multiplay", false),
                });
            }
            GenCountryList();
        }

        public static void Update(ObservableCollection<Country> houselist)
        {
            SaveFlag = true;
            Utils.CopyToList(houselist, ref OrgCountryList);
            Save();
            GenCountryList();
        }

        public static void Save()
        {
            if (!SaveFlag)
                return;
            SaveFlag = false;

            XmlDocument xmlDoc = XmlClass.CreateNew(xmlpath);
            XmlElement root = xmlDoc.CreateElement("Countries");
            foreach (Country c in OrgCountryList)
            {
                XmlElement elec = xmlDoc.CreateElement("Country");
                elec.SetAttribute("Name", c.Name);
                elec.SetAttribute("Value", c.SIndex);
                elec.SetAttribute("Side", c.SSide);
                elec.SetAttribute("UIName", c.UIName);
                elec.SetAttribute("IsEnabled", c.IsEnabled.ToString());
                elec.InnerText = c.Description;
                root.AppendChild(elec);
            }
            xmlDoc.AppendChild(root);
            xmlDoc.Save(xmlpath);
        }

        public static void LoadCsfName(CsfClass csf)
        {
            foreach (Country c in CountryList)
            {
                string tmp = csf.GetString(c.UIName);
                if (tmp != null)
                    c.Description = tmp;
            }
            SaveFlag = true;
        }

        private static void GenCountryList()
        {
            Utils.InitList(ref CountryList);
            Utils.InitList(ref CountryListWithNone, All);
            int index = -1;
            foreach (Country c in OrgCountryList)
            {
                if (c.Index > index)
                    index = c.Index;
                if (!c.IsEnabled)
                    continue;
                CountryList.Add(c);
                CountryListWithNone.Add(c);
            }
            for (int i = 0; i < MapCountryList.Count; ++i)
            {
                MapCountryList[i].Index = index + i + 1;
                CountryList.Add(MapCountryList[i]);
                CountryListWithNone.Add(MapCountryList[i]);
            }
        }

        private static string xmlpath;
        private static bool SaveFlag = false;
        private static ObservableCollection<Country> MapCountryList;
        private static ObservableCollection<Country> OrgCountryList;

        public static ObservableCollection<Country> CountryList;
        public static ObservableCollection<Country> CountryListWithNone;

        public static Country GetCountryOrNone(string name)
        {
            foreach(Country c in CountryList)
            {
                if (c.NameOrNone == name)
                    return c;
            }
            return All;
        }

        public static Country GetCountryOrNoneFromDes(string description)
        {
            foreach (Country c in CountryList)
            {
                if (c.Description == description || c.Name == description)
                    return c;
            }
            return All;
        }

        public static Country GetCountryOrAll(string name)
        {
            foreach (Country c in CountryList)
            {
                if (c.NameOrAll == name)
                    return c;
            }
            return All;
        }

        public static Country All { get; private set; }
    }

    public class Units
    {
        public Units(string folder = null)
        {
            IsSaved = true;
            Clear();
            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\units.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\units.xml";
            xmlpath = folder;
            xmlDoc = XmlClass.XmlOpen(xmlpath);

            try
            {
                XmlNodeList xnList = xmlDoc.SelectNodes("Units/Buildings/Building");
                foreach (XmlElement xn in xnList)
                {
                    Unit u = new Unit(UnitType.BuildingType, (XmlElement)xn);
                    AllList.Add(u);
                    BuildingList.Add(u);
                }
                xnList = xmlDoc.SelectNodes("Units/Infantries/Infantry");
                foreach (XmlElement xn in xnList)
                {
                    Unit u = new Unit(UnitType.InfantryType, (XmlElement)xn);
                    AllList.Add(u);
                    UnitsList.Add(u);
                }
                xnList = xmlDoc.SelectNodes("Units/Vehicles/Vehicle");
                foreach (XmlElement xn in xnList)
                {
                    Unit u = new Unit(UnitType.VehicleType, (XmlElement)xn);
                    AllList.Add(u);
                    UnitsList.Add(u);
                }
                xnList = xmlDoc.SelectNodes("Units/Aircrafts/Aircraft");
                foreach (XmlElement xn in xnList)
                {
                    Unit u = new Unit(UnitType.AircraftType, (XmlElement)xn);
                    AllList.Add(u);
                    UnitsList.Add(u);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Units: " + e.Message);
            }
        }

        public void Update(ObservableCollection<Unit> alllist,
            ObservableCollection<Unit> buildinglist,
            ObservableCollection<Unit> unitlist, bool KeepExistedUnits)
        {
            IsSaved = false;
            if (!KeepExistedUnits)
            {
                Clear();
                foreach (Unit u in alllist)
                    AllList.Add(u);
                foreach (Unit u in buildinglist)
                    BuildingList.Add(u);
                foreach (Unit u in unitlist)
                    UnitsList.Add(u);
            }
            else
            {
                foreach (Unit u in alllist)
                    if (!Contains(AllList,u.Name))
                        AllList.Add(u);
                foreach (Unit u in buildinglist)
                    if (!Contains(BuildingList, u.Name))
                        BuildingList.Add(u);
                foreach (Unit u in unitlist)
                    if (!Contains(UnitsList, u.Name))
                        UnitsList.Add(u);
            }
        }

        private void Clear()
        {
            Utils.InitList(ref AllList);
            Utils.InitList(ref BuildingList);
            Utils.InitList(ref UnitsList);
        }

        public void Save(bool SaveEnabledOnly = false)
        {
            if (IsSaved)
                return;
            IsSaved = true;
            xmlDoc = XmlClass.CreateNew(xmlpath);
            XmlElement u_ele = xmlDoc.CreateElement("Units");
            XmlElement b_ele = xmlDoc.CreateElement("Buildings");
            XmlElement i_ele = xmlDoc.CreateElement("Infantries");
            XmlElement v_ele = xmlDoc.CreateElement("Vehicles");
            XmlElement a_ele = xmlDoc.CreateElement("Aircrafts");
            foreach (Unit u in AllList)
            {
                if (SaveEnabledOnly && !u.IsEnabled)
                    continue;
                XmlElement ele_m;
                XmlElement elec;
                switch (u.UType)
                {
                    case UnitType.BuildingType:
                        ele_m = b_ele;
                        elec = xmlDoc.CreateElement("Building");
                        break;
                    case UnitType.InfantryType:
                        ele_m = i_ele;
                        elec = xmlDoc.CreateElement("Infantry");
                        break;
                    case UnitType.VehicleType:
                        ele_m = v_ele;
                        elec = xmlDoc.CreateElement("Vehicle");
                        break;
                    case UnitType.AircraftType:
                        ele_m = a_ele;
                        elec = xmlDoc.CreateElement("Aircraft");
                        break;
                    default:
                        continue;
                }
                elec.SetAttribute("Name", u.Name);
                elec.SetAttribute("UIName", u.UIName);
                elec.SetAttribute("Index", u.SIndex);
                elec.SetAttribute("IsEnabled", u.SIsEnabled);
                elec.SetAttribute("Cost", u.SCost);
                elec.SetAttribute("SequenceNumber", u.SSequenceIndex);
                elec.SetAttribute("TechLevel", u.STechLevel);
                elec.InnerText = u.Description;
                ele_m.AppendChild(elec);
            }
            u_ele.AppendChild(b_ele);
            u_ele.AppendChild(i_ele);
            u_ele.AppendChild(v_ele);
            u_ele.AppendChild(a_ele);
            xmlDoc.AppendChild(u_ele);
            xmlDoc.Save(xmlpath);
        }

        public static void StaticSave(bool SaveEnabledOnly = false)
        {
            XmlDocument xmlDoc = XmlClass.CreateNew(xmlpath);
            XmlElement u_ele = xmlDoc.CreateElement("Units");
            XmlElement b_ele = xmlDoc.CreateElement("Buildings");
            XmlElement i_ele = xmlDoc.CreateElement("Infantries");
            XmlElement v_ele = xmlDoc.CreateElement("Vehicles");
            XmlElement a_ele = xmlDoc.CreateElement("Aircrafts");
            foreach (Unit u in AllList)
            {
                if (SaveEnabledOnly && !u.IsEnabled)
                    continue;
                XmlElement ele_m;
                XmlElement elec;
                switch (u.UType)
                {
                    case UnitType.BuildingType:
                        ele_m = b_ele;
                        elec = xmlDoc.CreateElement("Building");
                        break;
                    case UnitType.InfantryType:
                        ele_m = i_ele;
                        elec = xmlDoc.CreateElement("Infantry");
                        break;
                    case UnitType.VehicleType:
                        ele_m = v_ele;
                        elec = xmlDoc.CreateElement("Vehicle");
                        break;
                    case UnitType.AircraftType:
                        ele_m = a_ele;
                        elec = xmlDoc.CreateElement("Aircraft");
                        break;
                    default:
                        continue;
                }
                elec.SetAttribute("Name", u.Name);
                elec.SetAttribute("UIName", u.UIName);
                elec.SetAttribute("Index", u.SIndex);
                elec.SetAttribute("IsEnabled", u.SIsEnabled);
                elec.SetAttribute("Cost", u.SCost);
                elec.SetAttribute("SequenceNumber", u.SSequenceIndex);
                elec.SetAttribute("TechLevel", u.STechLevel);
                elec.InnerText = u.Description;
                ele_m.AppendChild(elec);
            }
            u_ele.AppendChild(b_ele);
            u_ele.AppendChild(i_ele);
            u_ele.AppendChild(v_ele);
            u_ele.AppendChild(a_ele);
            xmlDoc.AppendChild(u_ele);
            xmlDoc.Save(xmlpath);
        }

        public static Unit FindUnit(string name)
        {
            if (name.Length == 0)
                return NullUnit;
            foreach (Unit u in UnitsList)
                if (u.Name == name)
                    return u;
            return NullUnit;
        }

        public static Unit FindUnitWithFuzzyLogic(string str)
        {
            foreach (Unit u in UnitsList)
                if (FuzzyLogic(u, str))
                    return u;
            return NullUnit;
        }

        public static Unit FindUnitFromAllWithFuzzyLogic(string str)
        {
            foreach (Unit u in AllList)
                if (FuzzyLogic(u, str))
                    return u;
            return NullUnit;
        }

        public static bool FuzzyLogic(Unit u, string search)
        {
            return u.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        || u.Description.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        public static Unit FindUnitFromAll(string name)
        {
            if (name.Length == 0)
                return NullUnit;
            foreach (Unit u in AllList)
                if (u.Name == name)
                    return u;
            return NullUnit;
        }

        public static Unit FindUnitFromBuildings(int seq)
        {
            foreach (Unit u in BuildingList)
                if (u.SequenceIndex == seq)
                    return u;
            return NullUnit;
        }

        public static string GetUnitDescription(string name)
        {
            if (name.Length == 0)
                return "";
            foreach (Unit u in UnitsList)
                if (u.Name == name)
                    return u.Description;
            return "";
        }

        public static bool Contains(ObservableCollection<Unit> list, string name)
        {
            foreach (Unit u in list)
                if (u.Name == name)
                    return true;
            return false;
        }

        private bool IsSaved;
        private static string xmlpath;
        private XmlDocument xmlDoc;

        public static Unit NullUnit = new Unit(UnitType.BuildingType, null);

        public static ObservableCollection<Unit> AllList;
        public static ObservableCollection<Unit> BuildingList;
        public static ObservableCollection<Unit> UnitsList;

        public static ObservableCollection<Unit> GetCleanBuildingList()
        {
            foreach (Unit u in BuildingList) u.UIVisibility = Visibility.Visible; return BuildingList;
        }


    }

    public class Scripts
    {
        public static void Load(string folder = null)
        {
            scriptItems = new ScriptItem[(int)ScriptAction.MoveToNearestCivilBuilding + 1];
            Utils.InitList(ref ParamCountries);
            Utils.InitList(ref ParamWaypoints);
            Utils.InitList(ref ParamBuildingID);
            Utils.InitList(ref ParamScripts);
            Utils.InitList(ref ParamTeams);
            foreach (Country c in Countries.CountryList)
                ParamCountries.Add(new ScriptItem.Parameter { Param = c.SIndex, Description = c.Name });
            foreach (string c in AI.waypoints_info)
                ParamWaypoints.Add(new ScriptItem.Parameter { Param = c });

            if (folder == null || folder.Length == 0)
                folder = Environment.CurrentDirectory + @"\scripts.xml";
            else
                folder = Environment.CurrentDirectory + @"\" + folder + @"\scripts.xml";
            xmlpath = folder;
            xmlDoc = XmlClass.XmlOpen(xmlpath);
            
            XmlNode xn = xmlDoc.SelectSingleNode("Scripts");
            if (xn != null && xn.ChildNodes != null)
            {
                int index;
                foreach (XmlNode node in xn.ChildNodes)
                {
                    index = GetIndexFromNode(node.Name);
                    if (index >= 0 && Enum.IsDefined(typeof(ScriptAction), node.Name))
                    {
                        scriptItems[index] = new ScriptItem((ScriptAction)Enum.Parse(typeof(ScriptAction), node.Name)) { Summary = ((XmlElement)node).GetAttribute("Summary") };
                        
                        foreach (XmlNode xn0 in ((XmlElement)node).ChildNodes)
                        {
                            try
                            {
                                if (xn0.Name == "Description")
                                {
                                    scriptItems[index].Description = xn0.InnerText.Trim();
                                    if (scriptItems[index].Summary == null || scriptItems[index].Summary.Length == 0)
                                        scriptItems[index].Summary = scriptItems[index].Description;
                                }
                                else if (xn0.Name == "IsAllowedInSkirmish")
                                    scriptItems[index].IsAllowedInSkirmish = Convert.ToBoolean(xn0.InnerText.Trim());
                                else if (xn0.Name == "Parameter")
                                {
                                    string param = ((XmlElement)xn0).GetAttribute("Value").Trim();
                                    if (param == "#REF:Countries#")
                                        scriptItems[index].ParamAllowed = ParamCountries;
                                    else if (param == "#REF:Waypoints#")
                                        scriptItems[index].ParamAllowed = ParamWaypoints;
                                    else if (param == "#REF:BuildingsID#")
                                        scriptItems[index].ParamAllowed = ParamBuildingID;
                                    else if (param == "#REF:Scripts#")
                                        scriptItems[index].ParamAllowed = ParamScripts;
                                    else if (param == "#REF:Teams#")
                                        scriptItems[index].ParamAllowed = ParamTeams;
                                    else
                                        scriptItems[index].ParamAllowed.Add(new ScriptItem.Parameter { Param = param, Description = xn0.InnerText.Trim() });
                                }
                            }
                            catch
                            {
                                scriptItems[index] = null;
                                break;
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < scriptItems.Length; ++i)
            {
                if (Enum.IsDefined(typeof(ScriptAction), i))
                {
                    if (scriptItems[i] == null)
                        scriptItems[i] = new ScriptItem((ScriptAction)i);
                }
            }

            ScriptInfo = new ObservableCollection<ScriptItem>();
            ScriptInfoOnlySkirmish = new ObservableCollection<ScriptItem>();
            for (int i = 0; i < scriptItems.Length; ++i)
            {
                if (Enum.IsDefined(typeof(ScriptAction), i) && scriptItems[i] != null)
                {
                    if (scriptItems[i].IsAllowedInSkirmish)
                        ScriptInfoOnlySkirmish.Add(scriptItems[i]);
                    ScriptInfo.Add(scriptItems[i]);
                }
            }
        }

        // Currently useless
        public void Save()
        {
            XmlElement ele = xmlDoc.CreateElement("Scripts");
            foreach (ScriptItem item in scriptItems)
            {
                if (item != null)
                {
                    XmlElement elec = xmlDoc.CreateElement(item.Name);
                    XmlElement eledes = xmlDoc.CreateElement("Description");
                    XmlElement eleski = xmlDoc.CreateElement("IsAllowedInSkirmish");
                    eledes.InnerText = item.Description;
                    eleski.InnerText = item.IsAllowedInSkirmish.ToString();
                    foreach(ScriptItem.Parameter p in item.ParamAllowed)
                    {
                        XmlElement elepar = xmlDoc.CreateElement("Parameter");
                        elepar.SetAttribute("Value", p.Param);
                        elepar.InnerText = p.Description;
                        ele.AppendChild(elepar);
                    }
                    elec.AppendChild(eledes);
                    elec.AppendChild(eleski);
                    ele.AppendChild(elec);
                }
            }
            xmlDoc.AppendChild(ele);
            xmlDoc.Save(xmlpath);
        }

        private static int GetIndexFromNode(string node)
        {
            ScriptAction ns = (ScriptAction)Enum.Parse(typeof(ScriptAction), node);
            if (Enum.IsDefined(typeof(ScriptAction), ns))
            {
                return (int)ns;
            }
            return -1;
        }

        public static ScriptItem IndexOf(int index)
        {
            return scriptItems[index];
        }

        public static ScriptItem Find(string name)
        {
            if (scriptItemsList != null)
            {
                foreach (ScriptItem si in scriptItemsList)
                {
                    if (si.SAction == name)
                        return si;
                }
                foreach (ScriptItem si in scriptItemsList)
                {
                    if (si.SAction.Contains(name))
                        return si;
                }
            }
            return null;
        }

        public static string GetNameFromAction(ScriptAction action)
        {
            if (scriptItems != null)
            {
                return scriptItems[(int)action].Name;
            }
            return "";
        }

        public static string GetDesFromAction(ScriptAction action)
        {
            if (scriptItems != null)
            {
                return scriptItems[(int)action].Description;
            }
            return "";
        }

        public static string GetSummaryFromAction(ScriptAction action)
        {
            if (scriptItems != null)
            {
                return scriptItems[(int)action].Summary;
            }
            return "";
        }

        public static string GetParamName(ScriptAction action, string param)
        {
            if (scriptItems != null)
            {
                if (scriptItems[(int)action].ParamAllowed == ParamBuildingID)
                {
                    int i = Utils.GetBuildingSeqIndexFromID(param);
                    foreach (Unit u in Units.BuildingList)
                        if (u.SequenceIndex == i)
                            return u.Description;
                    return "";
                }
                else
                    return scriptItems[(int)action].GetParamDescription(param);
            }
            return "";
        }

        public static ObservableCollection<ScriptItem.Parameter> GetParametersAllowedFromAction(ScriptAction action)
        {
            return scriptItems[(int)action].ParamAllowed;
        }

        private static string xmlpath;
        private static XmlDocument xmlDoc;
        private static ScriptItem[] scriptItems;
        public static bool HideScripts = true;

        public static ObservableCollection<ScriptItem> scriptItemsList;
        public static ObservableCollection<ScriptItem> ScriptList { get { return HideScripts ? ScriptInfoOnlySkirmish : ScriptInfo; } }
        public static ObservableCollection<ScriptItem> ScriptInfo;
        public static ObservableCollection<ScriptItem> ScriptInfoOnlySkirmish;

        public static ObservableCollection<ScriptItem.Parameter> ParamCountries;
        public static ObservableCollection<ScriptItem.Parameter> ParamWaypoints;
        public static ObservableCollection<ScriptItem.Parameter> ParamBuildingID;
        public static ObservableCollection<ScriptItem.Parameter> ParamScripts;
        public static ObservableCollection<ScriptItem.Parameter> ParamTeams;
    }

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
