using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Library;
using AIcore.Types;
using System.ComponentModel;

namespace AIcore
{
    public class AI : INotifyPropertyChanged
    {
        public AI(string inipath, bool fakemode = false) : this(new IniClass(inipath), fakemode)
        {
        }

        public AI(IniClass ai_ini, bool fakemode = false)
        {
            ini = ai_ini;
            IsFakeAI = fakemode;

            if (!fakemode)
            {
                InitializeParam();
                Init_Hints();

                TagPost = ini.ReadValueWithoutNotes("MarkSection", "TagPost");
                RA2AI_Editor.MainWindow.ChangeGame(GetLastGameType());
            }

            taskForces = new TLists.TaskForces(this);
            scriptTypes = new TLists.ScriptTypes(this);
            if (!fakemode)
            {
                taskforces_info = taskForces.taskForces;
                scripttypes_info = scriptTypes.scriptTypes;
            }

            teamTypes = new TLists.TeamTypes(this);
            if (!fakemode)
                teamtypes_info = teamTypes.teamTypes;

            aITriggerTypes = new TLists.AITriggerTypes(this);
            if (!fakemode)
                aitriggers_info = aITriggerTypes.aITriggerTypes;

            Scripts.ParamScripts.Clear();
            for (int i = 0; i < scripttypes_info.Count; ++i)
                Scripts.ParamScripts.Add(new ScriptItem.Parameter() { Param = i.ToString(), Description = scripttypes_info[i].PName });
            Scripts.ParamTeams.Clear();
            for (int i = 0; i < teamtypes_info.Count; ++i)
                Scripts.ParamTeams.Add(new ScriptItem.Parameter() { Param = i.ToString(), Description = teamtypes_info[i].PName });

        }

        private void Init_Hints()
        {
            if (IsMapFile)
                Countries.LoadMapCountries(ini);

            hint_units = Units.UnitsList;

            waypoints_info.Clear();
            transwaypoints_info.Clear();
            transwaypoints_info.Add(DoNotUseTransportOrigin);
            foreach (string key in ini.GetKeys("Waypoints"))
            {
                waypoints_info.Add(key);
                transwaypoints_info.Add(key);
            }
        }

        /// <summary>
        /// 试图解析出section的信息
        /// </summary>
        /// <param name="section"></param>
        private OType TrySection(string section)
        {
            IList<string> keys = ini.GetKeys(section);
            bool isTaskForce = true;
            bool isTeamType = true;
            bool isScriptType = true;
            foreach (string key in keys)
            {
                if (!Regex.IsMatch(key, @"^[0-9]+$"))
                {
                    if (key == "Name")
                        continue;
                    else if (key == "Group")
                        isScriptType = false;
                    else if (Types.TeamType.IsKeyOfTeamType(key))
                        isTaskForce = isScriptType = false;
                    else
                    { isTeamType = isTaskForce = isScriptType = false; break; }
                }
                else
                    isTeamType = false;
            }
            if (isScriptType && isTaskForce)
            {
                foreach (string key in keys)
                {
                    if (key == "Name")
                        continue;
                    string value = ini.ReadValueWithoutNotes(section, key).Replace(" ", "").Replace("\t", "");
                    if (Regex.IsMatch(value, @"^[0-9]+\,[0-9]+$"))
                        isTaskForce = false;
                    else if (Regex.IsMatch(value, @"^[0-9]+\,[\s\S]*$"))
                        isScriptType = false;
                    else
                    { isTaskForce = isScriptType = false; break; }
                }
            }
            if (isTaskForce)
                return new Types.TaskForce(taskForces.GetNewTag());
            else if (isScriptType)
                return new Types.ScriptType(scriptTypes.GetNewTag());
            else if (isTeamType)
                return new Types.ScriptType(teamTypes.GetNewTag());
            return null;
        }

        private bool IsMapExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return false;
            ext = ext.ToLower();
            return ext == ".map" || ext == ".mpr" || ext == ".yrm";
        }

        public void SaveAI(string path)
        {
            aITriggerTypes.SaveIni(false);
            taskForces.SaveIni(false);
            scriptTypes.SaveIni(false);
            teamTypes.SaveIni(false);
            SaveAIMark();
            ini.SaveAs(path);
            InitializeParam(path);
        }

        public void ReleaseAI(string path)
        {
            SaveAI(FilePath);
            ini.CopyAs(path);
            AI release = new AI(path, true);
            release.IsMapFile = IsMapFile;
            release.ini.WriteValue("MarkSection", null, null);
            release.aITriggerTypes.SaveIni(true);
            release.taskForces.SaveIni(true);
            release.scriptTypes.SaveIni(true);
            release.teamTypes.SaveIni(true);
            release.ini.Save();
            releaseTaskForces.Clear();
            releaseTeamTypes.Clear();
        }

        private Game.GameTypeClass GetLastGameType()
        {
            string game = ini.ReadValueWithoutNotes("MarkSection", "LastGame");
            if (game.Length > 0)
            {
                Game.GameTypeClass lastgame = Game.GetGameTypeWithDigest(game);
                if (lastgame != null)
                    return lastgame;
            }
            Game.GameType tmp = ini.ReadEnumValue("MarkSection", "LastGameType", Game.GameType.Unknown);
            //if (tmp == Game.GameType.Unknown)
            //{
            //    if (ContainDigest(Game.DefaultYRGame.Digest))
            //        tmp = Game.GameType.YR;
            //    else if (ContainDigest(Game.DefaultRAGame.Digest))
            //        tmp = Game.GameType.RA;
            //    if (tmp == Game.CurrentGame.GameType)
            //        return Game.CurrentGame;
            //}
            switch(tmp)
            {
                case Game.GameType.RA:
                    return Game.DefaultRAGame;
                case Game.GameType.YR:
                    return Game.DefaultYRGame;
                default:
                    return Game.DefaultUnknownGame;
            }
        }

        private void SaveAIMark()
        {
            ini.WriteValue("MarkSection", "LastGameType", (int)Game.CurrentGame.GameType);
            if (Game.IsCustomGameType())
                ini.WriteValue("MarkSection", "LastGame", Game.CurrentGame.Digest);
            else
                ini.WriteValue("MarkSection", "LastGame", null);
            ini.WriteValue("MarkSection", "TagPost", TagPost);
        }

        private bool ContainDigest(string digest)
        {
            foreach(string value in ini.GetValues("Digest"))
            {
                if (value == digest)
                    return true;
            }
            return false;
        }

        private void InitializeParam(string path = null)
        {
            IsMapFile = IsMapExtension(Path.GetExtension(string.IsNullOrEmpty(path) ? ini.GetPath() : path));
        }

        public TaskForce GetTaskForce(string tag)
        {
            if (taskForces != null)
            {
                TaskForce ret = taskForces.GetTaskForce(tag);
                return ret ?? NullTaskForce;
            }
            return NullTaskForce;
        }

        public ScriptType GetScriptType(string tag)
        {
            if (scriptTypes != null)
            {
                ScriptType ret = scriptTypes.GetScriptType(tag);
                return ret ?? NullScriptType;
            }
            return NullScriptType;
        }

        public TeamType GetTeamType(string tag)
        {
            if (teamTypes != null)
            {
                TeamType ret = teamTypes.GetTeamType(tag);
                return ret ?? NullTeamType;
            }
            return NullTeamType;
        }

        public AITriggerType GetAITriggerType(string tag)
        {
            if (aITriggerTypes != null)
            {
                return aITriggerTypes.GetAITriggerType(tag);
            }
            return null;
        }

        public void RefreshAnalysisResult()
        {
            foreach (TaskForce tf in taskForces.taskForces)
                tf.AnalysisResult = tf.AnalysisResult;
            foreach (ScriptType st in scriptTypes.scriptTypes)
                st.AnalysisResult = st.AnalysisResult;
            foreach (TeamType tt in teamTypes.teamTypes)
                tt.AnalysisResult = tt.AnalysisResult;
            foreach (AITriggerType at in aITriggerTypes.aITriggerTypes)
                at.AnalysisResult = at.AnalysisResult;
        }

        public static TaskForce FindTaskForceWithFuzzyLogic(string str)
        {
            foreach (TaskForce t in taskforces_info)
                if (FuzzyLogic(t, str))
                    return t;
            return NullTaskForce;
        }

        public static ScriptType FindScriptTypeWithFuzzyLogic(string str)
        {
            foreach (ScriptType t in scripttypes_info)
                if (FuzzyLogic(t, str))
                    return t;
            return NullScriptType;
        }

        public static TeamType FindTeamTypeWithFuzzyLogic(string str)
        {
            foreach (TeamType t in teamtypes_info)
                if (FuzzyLogic(t, str))
                    return t;
            return NullTeamType;
        }

        public static bool FuzzyLogic<T>(T t, string search) where T : TagType
        {
            if (string.IsNullOrEmpty(search)) return false;
            return t._tag.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0
                        || t.Name.IndexOf(search, StringComparison.CurrentCultureIgnoreCase) >= 0;
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

        public IniClass ini;

        public bool IsFakeAI;

        public bool IsMapFile = false;
        public string FilePath { get { return ini.GetPath(); } }
        public string TagPost;

        public TLists.TaskForces taskForces;
        public TLists.ScriptTypes scriptTypes;
        public TLists.TeamTypes teamTypes;
        public TLists.AITriggerTypes aITriggerTypes;

        public static ObservableCollection<TaskForce> taskforces_info;
        public static ObservableCollection<ScriptType> scripttypes_info;
        public static ObservableCollection<TeamType> teamtypes_info;
        public static ObservableCollection<AITriggerType> aitriggers_info;
        public static ObservableCollection<string> waypoints_info = new ObservableCollection<string>();
        public static ObservableCollection<string> transwaypoints_info = new ObservableCollection<string>();
        public static ObservableCollection<Unit> hint_units;
        public static string DoNotUseTransportOrigin = "None";

        public static readonly TaskForce NullTaskForce = new TaskForce(null);
        public static readonly ScriptType NullScriptType = new ScriptType(null);
        public static readonly TeamType NullTeamType = new TeamType(null, null, NullScriptType, NullTaskForce);

        public Dictionary<TaskForce, (TaskForce, TaskForce, TaskForce)> releaseTaskForces = new Dictionary<TaskForce, (TaskForce, TaskForce, TaskForce)>();
        public Dictionary<TeamType, (TeamType, TeamType, TeamType)> releaseTeamTypes = new Dictionary<TeamType, (TeamType, TeamType, TeamType)>();
    }
}
