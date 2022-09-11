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
            foreach (Side s in sidelist)
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
}
