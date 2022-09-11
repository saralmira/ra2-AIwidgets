using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace AIcore
{
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
                    if (!Contains(AllList, u.Name))
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
                elec.SetAttribute("Translation", u.Translation);
                elec.InnerText = u.GetDescription();
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
                elec.SetAttribute("Translation", u.Translation);
                elec.InnerText = u.GetDescription();
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

        public static bool ContainsUnit(string name)
        {
            if (name.Length == 0)
                return false;
            foreach (Unit u in UnitsList)
                if (u.Name == name)
                    return true;
            return false;
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
            if (u == null)
                return false;
            return u.FuzzyLogic(search);
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
                    return u.GetDescription();
            return "";
        }

        public static string GetUnitTranslation(string name)
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
}
