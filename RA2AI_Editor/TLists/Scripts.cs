using Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AIcore
{
    public class Scripts
    {
        public static void Load(string folder = null)
        {
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
            scriptItems = new Dictionary<int, ScriptItem>();

            if (xn != null && xn.ChildNodes != null)
            {
                var count = xn.ChildNodes.Count;
                for (int i = 0; i < count; ++i)
                {
                    ScriptItem item = InitFromNode(xn.ChildNodes[i]);
                    scriptItems[item.scriptAction] = item;
                }
            }
            
            ScriptInfo = new ObservableCollection<ScriptItem>();
            ScriptInfoOnlySkirmish = new ObservableCollection<ScriptItem>();

            var sq_scripts = scriptItems.OrderBy((v) => v.Key).ToList();
            for (int i = 0; i < sq_scripts.Count; ++i)
            {
                var item = sq_scripts[i].Value;
                if (item.IsAllowedInSkirmish)
                    ScriptInfoOnlySkirmish.Add(item);
                ScriptInfo.Add(item);
            }
        }

        private static ScriptItem InitFromNode(XmlNode node)
        {
            XmlElement xe = node as XmlElement;
            var value = xe.GetAttribute("Value");
            var index = Convert.ToInt32(value);
            if (index < 0)
                throw new ArgumentOutOfRangeException(string.Format("node [{0}] value out of range: {1}", node.Name, index));

            ScriptItem ret = new ScriptItem(index) { Summary = ((XmlElement)node).GetAttribute("Summary") };
            foreach (XmlNode xn0 in node.ChildNodes)
            {
                switch (xn0.Name)
                {
                    case "Description":
                        ret.Description = xn0.InnerText.Trim();
                        if (ret.Summary.Length == 0)
                            ret.Summary = ret.Description;
                        break;
                    case "IsAllowedInSkirmish":
                        ret.IsAllowedInSkirmish = Convert.ToBoolean(xn0.InnerText.Trim());
                        break;
                    case "Parameter":
                        string param = ((XmlElement)xn0).GetAttribute("Value").Trim();
                        switch (param)
                        {
                            case REF_Countries:
                                ret.ParamAllowed = ParamCountries;
                                break;
                            case REF_Waypoints:
                                ret.ParamAllowed = ParamWaypoints;
                                break;
                            case REF_BuildingsID:
                                ret.ParamAllowed = ParamBuildingID;
                                break;
                            case REF_Scripts:
                                ret.ParamAllowed = ParamScripts;
                                break;
                            case REF_Teams:
                                ret.ParamAllowed = ParamTeams;
                                break;
                            case REF_Parameters:
                                if (int.TryParse(((XmlElement)xn0).GetAttribute("Value2").Trim(), out index) && scriptItems[index] != null)
                                    ret.ParamAllowed = scriptItems[index].ParamAllowed;
                                break;
                            default:
                                ret.ParamAllowed.Add(new ScriptItem.Parameter { Param = param, Description = xn0.InnerText.Trim() });
                                break;
                        }
                        break;
                }
            }
            return ret;
        }

        // Currently useless
        //public void Save()
        //{
        //    XmlElement ele = xmlDoc.CreateElement("Scripts");
        //    foreach (ScriptItem item in scriptItems)
        //    {
        //        if (item != null)
        //        {
        //            XmlElement elec = xmlDoc.CreateElement(item.Name);
        //            XmlElement eledes = xmlDoc.CreateElement("Description");
        //            XmlElement eleski = xmlDoc.CreateElement("IsAllowedInSkirmish");
        //            eledes.InnerText = item.Description;
        //            eleski.InnerText = item.IsAllowedInSkirmish.ToString();
        //            foreach (ScriptItem.Parameter p in item.ParamAllowed)
        //            {
        //                XmlElement elepar = xmlDoc.CreateElement("Parameter");
        //                elepar.SetAttribute("Value", p.Param);
        //                elepar.InnerText = p.Description;
        //                ele.AppendChild(elepar);
        //            }
        //            elec.AppendChild(eledes);
        //            elec.AppendChild(eleski);
        //            ele.AppendChild(elec);
        //        }
        //    }
        //    xmlDoc.AppendChild(ele);
        //    xmlDoc.Save(xmlpath);
        //}

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

        public static string GetNameFromAction(int action)
        {
            if (scriptItems != null)
            {
                return scriptItems[action].Name;
            }
            return "";
        }

        public static string GetDesFromAction(int action)
        {
            if (scriptItems != null)
            {
                return scriptItems[action].Description;
            }
            return "";
        }

        public static string GetSummaryFromAction(int action)
        {
            if (scriptItems != null)
            {
                return scriptItems[action].Summary;
            }
            return "";
        }

        public static string GetParamName(int action, string param)
        {
            if (scriptItems != null)
            {
                if (scriptItems[action].ParamAllowed == ParamBuildingID)
                {
                    int i = Utils.GetBuildingSeqIndexFromID(param);
                    foreach (Unit u in Units.BuildingList)
                        if (u.SequenceIndex == i)
                            return u.Description;
                    return "";
                }
                else
                    return scriptItems[action].GetParamDescription(param);
            }
            return "";
        }

        public static ObservableCollection<ScriptItem.Parameter> GetParametersAllowedFromAction(int action)
        {
            return scriptItems[action].ParamAllowed;
        }

        private static string xmlpath;
        private static XmlDocument xmlDoc;
        private static Dictionary<int, ScriptItem> scriptItems;
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

        public const string REF_Countries = "#REF:Countries#";
        public const string REF_Waypoints = "#REF:Waypoints#";
        public const string REF_BuildingsID = "#REF:BuildingsID#";
        public const string REF_Scripts = "#REF:Scripts#";
        public const string REF_Teams = "#REF:Teams#";
        public const string REF_Parameters = "#REF:Parameters#";
    }
}
