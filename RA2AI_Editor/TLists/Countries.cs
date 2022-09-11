using Library;
using RA2AI_Editor;
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
            foreach (Country c in CountryList)
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
}
