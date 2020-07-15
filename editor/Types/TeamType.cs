using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Library;
using AIcore;

namespace AIcore.Types
{
    public class TeamType : OType, INotifyPropertyChanged
    {
        public TeamType(AI _ai, string tag, ScriptType scriptType, TaskForce taskForce) : base(tag)
        {
            Name = tag == null ? "" : "_TeamType_";
            ai = _ai;

            Script = scriptType;
            TaskForce = taskForce;

            //VeteranLevelInfo = AIcore.VeteranLevelInfo.veteranLevels;
            //CountryListInfo = Countries.CountryListWithNone;
            //MindControlDecisionInfo = AIcore.MindControlDecision.MindControlDecisionInfo;
            //GroupInfo = AIcore.GroupInfo.Group;
            //WaypointsInfo = AI.waypoints_info;
            //TransportWaypointsInfo = AI.transwaypoints_info;
            //TaskForceInfo = AI.taskforces_info;
            //ScriptTypeInfo = AI.scripttypes_info;
        }

        public TeamType(AI _ai, IniClass ini, string tag) : base(tag)
        {
            rec_tag = tag;
            ai = _ai;
            NewType = false;
            Init(ini, tag);

            //VeteranLevelInfo = AIcore.VeteranLevelInfo.veteranLevels;
            //CountryListInfo = Countries.CountryListWithNone;
            //MindControlDecisionInfo = AIcore.MindControlDecision.MindControlDecisionInfo;
            //GroupInfo = AIcore.GroupInfo.Group;
            //WaypointsInfo = AI.waypoints_info;
            //TransportWaypointsInfo = AI.transwaypoints_info;
            //TaskForceInfo = AI.taskforces_info;
            //ScriptTypeInfo = AI.scripttypes_info;
        }

        protected override void Init(IniClass ini, string tag)
        {
            PName = ini.ReadValueWithoutNotes(tag, "Name");
            VeteranLevel = ini.ReadUIntValue(tag, "VeteranLevel", 1);
            MindControlDecision = AIcore.MindControlDecision.GetMindControlDecision(ini.ReadValueWithoutNotes(tag, "MindControlDecision"));
            Priority = ini.ReadUIntValue(tag, "Priority", 10);
            Max = ini.ReadUIntValue(tag, "Max", 2);
            TechLevel = ini.ReadIntValue(tag, "TechLevel", 0);
            Group = ini.ReadIntValue(tag, "Group", -1);
            Waypoint = new WaypointClass() { SWaypointFormat = ini.ReadValueWithoutNotes(tag, "Waypoint") };
            TransportWaypoint = new WaypointClass() { SWaypointFormatTransport = ini.ReadValueWithoutNotes(tag, "TransportWaypoint") };

            Loadable = ini.ReadBoolValue(tag, "Loadable", false);
            Full = ini.ReadBoolValue(tag, "Full", false);
            Annoyance = ini.ReadBoolValue(tag, "Annoyance", false);
            GuardSlower = ini.ReadBoolValue(tag, "GuardSlower", false);
            Recruiter = ini.ReadBoolValue(tag, "Recruiter", false);
            Autocreate = ini.ReadBoolValue(tag, "Autocreate", true);
            Prebuild = ini.ReadBoolValue(tag, "Prebuild", false);
            Reinforce = ini.ReadBoolValue(tag, "Reinforce", false);
            Droppod = ini.ReadBoolValue(tag, "Droppod", false);
            //UseTransportOrigin = ini.ReadBoolValue(tag, "UseTransportOrigin", DV.UseTransportOrigin);
            Whiner = ini.ReadBoolValue(tag, "Whiner", false);
            LooseRecruit = ini.ReadBoolValue(tag, "LooseRecruit", false);
            Aggressive = ini.ReadBoolValue(tag, "Aggressive", false);
            Suicide = ini.ReadBoolValue(tag, "Suicide", false);
            OnTransOnly = ini.ReadBoolValue(tag, "OnTransOnly", false);
            AvoidThreats = ini.ReadBoolValue(tag, "AvoidThreats", false);
            IonImmune = ini.ReadBoolValue(tag, "IonImmune", false);
            TransportsReturnOnUnload = ini.ReadBoolValue(tag, "TransportsReturnOnUnload", false);
            AreTeamMembersRecruitable = ini.ReadBoolValue(tag, "AreTeamMembersRecruitable", false);
            IsBaseDefense = ini.ReadBoolValue(tag, "IsBaseDefense", false);
            OnlyTargetHouseEnemy = ini.ReadBoolValue(tag, "OnlyTargetHouseEnemy", false);

            SHouse = ini.ReadValueWithoutNotes(tag, "House");

            Tag = ini.ReadValueWithoutNotes(tag, "Tag");

            SScript = ini.ReadValueWithoutNotes(tag, "Script");
            STaskForce = ini.ReadValueWithoutNotes(tag, "TaskForce");
        }

        public int GetTechLevelMax()
        {
            if (TaskForce != null && TaskForce != AI.NullTaskForce)
                return TaskForce.GetTechLevelMax();
            return 0;
        }

        public override void Output(IniClass ini)
        {
            ini.WriteValue(_tag, null, null);
            ini.WriteValue(_tag, "Name", Name);
            ini.WriteValue(_tag, "VeteranLevel", VeteranLevel.ToString());
            if (Game.CurrentGame.GameType == Game.GameType.YR)
                ini.WriteValue(_tag, "MindControlDecision", MindControlDecision.Value);
            ini.WriteValue(_tag, "Priority", Priority.ToString());
            ini.WriteValue(_tag, "Max", Max.ToString());
            ini.WriteValue(_tag, "TechLevel", TechLevel.ToString());
            ini.WriteValue(_tag, "Group", Group.ToString());
            ini.WriteValue(_tag, "Waypoint", Waypoint.SWaypointFormat);
            ini.WriteValue(_tag, "TransportWaypoint", TransportWaypoint.SWaypointFormat);

            ini.WriteValue(_tag, "Loadable", YesOrNo(Loadable));
            ini.WriteValue(_tag, "Full", YesOrNo(Full));
            ini.WriteValue(_tag, "Annoyance", YesOrNo(Annoyance));
            ini.WriteValue(_tag, "GuardSlower", YesOrNo(GuardSlower));
            ini.WriteValue(_tag, "Recruiter", YesOrNo(Recruiter));
            ini.WriteValue(_tag, "Autocreate", YesOrNo(Autocreate));
            ini.WriteValue(_tag, "Prebuild", YesOrNo(Prebuild));
            ini.WriteValue(_tag, "Reinforce", YesOrNo(Reinforce));
            ini.WriteValue(_tag, "Droppod", YesOrNo(Droppod));
            if (TransportWaypoint.SWaypointFormat != null)
                ini.WriteValue(_tag, "UseTransportOrigin", "yes");
            else
                ini.WriteValue(_tag, "UseTransportOrigin", "no");
            ini.WriteValue(_tag, "Whiner", YesOrNo(Whiner));
            ini.WriteValue(_tag, "LooseRecruit", YesOrNo(LooseRecruit));
            ini.WriteValue(_tag, "Aggressive", YesOrNo(Aggressive));
            ini.WriteValue(_tag, "Suicide", YesOrNo(Suicide));
            ini.WriteValue(_tag, "OnTransOnly", YesOrNo(OnTransOnly));
            ini.WriteValue(_tag, "AvoidThreats", YesOrNo(AvoidThreats));
            ini.WriteValue(_tag, "IonImmune", YesOrNo(IonImmune));
            ini.WriteValue(_tag, "TransportsReturnOnUnload", YesOrNo(TransportsReturnOnUnload));
            ini.WriteValue(_tag, "AreTeamMembersRecruitable", YesOrNo(AreTeamMembersRecruitable));
            ini.WriteValue(_tag, "IsBaseDefense", YesOrNo(IsBaseDefense));
            ini.WriteValue(_tag, "OnlyTargetHouseEnemy", YesOrNo(OnlyTargetHouseEnemy));

            ini.WriteValue(_tag, "House", SHouse);

            if (Tag != null && Tag.Length > 0)
                ini.WriteValue(_tag, "Tag", Tag);

            ini.WriteValue(_tag, "Script", Script == null ? SScript : Script._tag);
            ini.WriteValue(_tag, "TaskForce", TaskForce == null ? STaskForce : TaskForce._tag);
        }

        private string YesOrNo(bool flag)
        {
            return flag ? "yes" : "no";
        }

        public bool CompareWith(TeamType a)
        {
            return a.PTag == PTag && a.SScript == SScript && a.STaskForce == STaskForce &&
                a.SHouse == SHouse && a.VeteranLevel == VeteranLevel && (MindControlDecision == null || a.MindControlDecision == null || 
                a.MindControlDecision.Value == MindControlDecision.Value) && a.Priority == Priority &&
                a.Max == Max && a.TechLevel == TechLevel && a.Group == Group && a.Waypoint.CompareWith(Waypoint) &&
                a.TransportWaypoint.CompareWith(TransportWaypoint) && a.Loadable == Loadable &&
                a.Full == Full && a.Annoyance == Annoyance && a.GuardSlower == GuardSlower &&
                a.Recruiter == Recruiter && a.Autocreate == Autocreate && a.Prebuild == Prebuild &&
                a.Reinforce == Reinforce && a.Droppod == Droppod && a.Whiner == Whiner &&
                a.LooseRecruit == LooseRecruit && a.Aggressive == Aggressive &&
                a.Suicide == Suicide && a.OnTransOnly == OnTransOnly &&
                a.AvoidThreats == AvoidThreats && a.IonImmune == IonImmune &&
                a.TransportsReturnOnUnload == TransportsReturnOnUnload &&
                a.AreTeamMembersRecruitable == AreTeamMembersRecruitable && a.IsBaseDefense == IsBaseDefense &&
                a.OnlyTargetHouseEnemy == OnlyTargetHouseEnemy && a.Tag == Tag;
        }

        public TeamType CloneType(string tag)
        {
            return new TeamType(ai, tag, this.Script, this.TaskForce)
            {
                PName = this.PName,
                VeteranLevel = this.VeteranLevel,
                SMindControlDecision = this.MindControlDecision.Value,
                Priority = this.Priority,
                Max = this.Max,
                TechLevel = this.TechLevel,
                Group = this.Group,
                Waypoint = this.Waypoint,
                TransportWaypoint = this.TransportWaypoint,

                Loadable = this.Loadable,
                Full = this.Full,
                Annoyance = this.Annoyance,
                GuardSlower = this.GuardSlower,
                Recruiter = this.Recruiter,
                Autocreate = this.Autocreate,
                Prebuild = this.Prebuild,
                Reinforce = this.Reinforce,
                Droppod = this.Droppod,
                //UseTransportOrigin = ini.ReadBoolValue(tag, "UseTransportOrigin", DV.UseTransportOrigin);
                Whiner = this.Whiner,
                LooseRecruit = this.LooseRecruit,
                Aggressive = this.Aggressive,
                Suicide = this.Suicide,
                OnTransOnly = this.OnTransOnly,
                AvoidThreats = this.AvoidThreats,
                IonImmune = this.IonImmune,
                TransportsReturnOnUnload = this.TransportsReturnOnUnload,
                AreTeamMembersRecruitable = this.AreTeamMembersRecruitable,
                IsBaseDefense = this.IsBaseDefense,
                OnlyTargetHouseEnemy = this.OnlyTargetHouseEnemy,
                SHouse = this.SHouse,
                Tag = this.Tag
            };
        }

        private uint _VeteranLevel = 1;
        public uint VeteranLevel
        {
            get { return _VeteranLevel; }
            set { _VeteranLevel = value; PropertyChange(this, "VeteranLevel"); }
        }

        private InfoValueClass _MindControlDecision = AIcore.MindControlDecision.DefaultMindControlDecision;
        public InfoValueClass MindControlDecision
        {
            get { return _MindControlDecision; }
            set { _MindControlDecision = value; PropertyChange(this, "MindControlDecision"); }
        }
        public string SMindControlDecision
        {
            set
            {
                MindControlDecision = AIcore.MindControlDecision.GetMindControlDecision(value);
            }
        }

        private uint _Priority = 10;
        public uint Priority
        {
            get { return _Priority; }
            set { _Priority = value; PropertyChange(this, "Priority"); }
        }
        private uint _Max = 2;
        public uint Max
        {
            get { return _Max; }
            set { _Max = value; PropertyChange(this, "Max"); }
        }
        private int _TechLevel = 0;
        public int TechLevel
        {
            get { return _TechLevel; }
            set { _TechLevel = value; PropertyChange(this, "TechLevel"); }
        }
        private int _Group = -1;
        public int Group
        {
            get { return _Group; }
            set { _Group = value; PropertyChange(this, "Group"); }
        }
        public WaypointClass Waypoint = WaypointClass.InvalidWaypoint;
        public string SWaypoint
        {
            get
            {
                return Waypoint.SWaypoint;
            }
            set
            {
                Waypoint.SWaypoint = value;
                PropertyChange(this, "SWaypoint");
            }
        }
        public WaypointClass TransportWaypoint = WaypointClass.InvalidWaypoint;
        public string STransportWaypoint
        {
            get
            {
                return TransportWaypoint.SWaypoint;
            }
            set
            {
                TransportWaypoint.SWaypoint = value;
                PropertyChange(this, "STransportWaypoint");
            }
        }

        private bool _Loadable = false;
        public bool Loadable
        {
            get { return _Loadable; }
            set { _Loadable = value; PropertyChange(this, "Loadable"); }
        }
        private bool _Full = false;
        public bool Full
        {
            get { return _Full; }
            set { _Full = value; PropertyChange(this, "Full"); }
        }
        private bool _Annoyance = false;
        public bool Annoyance
        {
            get { return _Annoyance; }
            set { _Annoyance = value; PropertyChange(this, "Annoyance"); }
        }
        private bool _GuardSlower = false;
        public bool GuardSlower
        {
            get { return _GuardSlower; }
            set { _GuardSlower = value; PropertyChange(this, "GuardSlower"); }
        }
        private bool _Recruiter = false;
        public bool Recruiter
        {
            get { return _Recruiter; }
            set { _Recruiter = value; PropertyChange(this, "Recruiter"); }
        }
        private bool _Autocreate = true;
        public bool Autocreate
        {
            get { return _Autocreate; }
            set { _Autocreate = value; PropertyChange(this, "Autocreate"); }
        }
        private bool _Prebuild = false;
        public bool Prebuild
        {
            get { return _Prebuild; }
            set { _Prebuild = value; PropertyChange(this, "Prebuild"); }
        }
        private bool _Reinforce = false;
        public bool Reinforce
        {
            get { return _Reinforce; }
            set { _Reinforce = value; PropertyChange(this, "Reinforce"); }
        }
        private bool _Droppod = false;
        public bool Droppod
        {
            get { return _Droppod; }
            set { _Droppod = value; PropertyChange(this, "Droppod"); }
        }
        //public bool UseTransportOrigin;
        private bool _Whiner = false;
        public bool Whiner
        {
            get { return _Whiner; }
            set { _Whiner = value; PropertyChange(this, "Whiner"); }
        }
        private bool _LooseRecruit = false;
        public bool LooseRecruit
        {
            get { return _LooseRecruit; }
            set { _LooseRecruit = value; PropertyChange(this, "LooseRecruit"); }
        }
        private bool _Aggressive = false;
        public bool Aggressive
        {
            get { return _Aggressive; }
            set { _Aggressive = value; PropertyChange(this, "Aggressive"); }
        }
        private bool _Suicide = false;
        public bool Suicide
        {
            get { return _Suicide; }
            set { _Suicide = value; PropertyChange(this, "Suicide"); }
        }
        private bool _OnTransOnly = false;
        public bool OnTransOnly
        {
            get { return _OnTransOnly; }
            set { _OnTransOnly = value; PropertyChange(this, "OnTransOnly"); }
        }
        private bool _AvoidThreats = false;
        public bool AvoidThreats
        {
            get { return _AvoidThreats; }
            set { _AvoidThreats = value; PropertyChange(this, "AvoidThreats"); }
        }
        private bool _IonImmune = false;
        public bool IonImmune
        {
            get { return _IonImmune; }
            set { _IonImmune = value; PropertyChange(this, "IonImmune"); }
        }
        private bool _TransportsReturnOnUnload = false;
        public bool TransportsReturnOnUnload
        {
            get { return _TransportsReturnOnUnload; }
            set { _TransportsReturnOnUnload = value; PropertyChange(this, "TransportsReturnOnUnload"); }
        }
        private bool _AreTeamMembersRecruitable = false;
        public bool AreTeamMembersRecruitable
        {
            get { return _AreTeamMembersRecruitable; }
            set { _AreTeamMembersRecruitable = value; PropertyChange(this, "AreTeamMembersRecruitable"); }
        }
        private bool _isbasedefense = false;
        public bool IsBaseDefense
        {
            get { return _isbasedefense; }
            set { _isbasedefense = value; PropertyChange(this, "IsBaseDefense"); }
        }
        public bool _OnlyTargetHouseEnemy = false;
        public bool OnlyTargetHouseEnemy
        {
            get { return _OnlyTargetHouseEnemy; }
            set { _OnlyTargetHouseEnemy = value; PropertyChange(this, "OnlyTargetHouseEnemy"); }
        }

        private string _Tag = null;
        public string Tag
        {
            get { return _Tag; }
            set { _Tag = value; PropertyChange(this, "Tag"); }
        }

        private ScriptType _Script = AI.NullScriptType;
        public ScriptType Script
        {
            get { return _Script; }
            set
            {
                _Script = value ?? AI.NullScriptType;
                _SScript = _Script.PTag;
                PropertyChange(this, "SScript");
                PropertyChange(this, "SScriptTypeName");
            }
        }
        private string _SScript = AI.NullScriptType.PTag;
        public string SScript
        {
            get { return _SScript; }
            set
            {
                _SScript = value;
                if (ai != null)
                    _Script = ai.GetScriptType(value);
                else
                    _Script = AI.NullScriptType;
                PropertyChange(this, "SScript");
                PropertyChange(this, "SScriptTypeName");
            }
        }
        public string SScriptTypeName
        {
            get
            {
                if (Script != null && Script != AI.NullScriptType)
                    return Script.PName;
                return AI.NullScriptType.PTag;
            }
        }

        private TaskForce _TaskForce = AI.NullTaskForce;
        public TaskForce TaskForce
        {
            get { return _TaskForce; }
            set
            {
                _TaskForce = value ?? AI.NullTaskForce;
                _STaskForce = _TaskForce.PTag;
                PropertyChange(this, "STaskForce");
                PropertyChange(this, "STaskForceName");
            }
        }
        private string _STaskForce = AI.NullTaskForce.PTag;
        public string STaskForce
        {
            get { return _STaskForce; }
            set
            {
                _STaskForce = value;
                _TaskForce = ai == null ? AI.NullTaskForce : ai.GetTaskForce(_STaskForce);
                PropertyChange(this, "STaskForce");
                PropertyChange(this, "STaskForceName");
            }
        }
        public string STaskForceName
        {
            get
            {
                if (TaskForce != null && TaskForce != AI.NullTaskForce)
                    return TaskForce.PName;
                return AI.NullTaskForce.PTag;
            }
        }

        private Country _House = Countries.All;
        public Country House
        {
            get { return _House; }
            set
            {
                _House = value;
                PropertyChange(this, "House");
            }
        }
        public string SHouse
        {
            get { return House.NameOrNone; }
            set
            {
                House = Countries.GetCountryOrNone(value);
            }
        }
        //public string SHouseDes
        //{
        //    get { return House.DescriptionOrNone; }
        //    set
        //    {
        //        House = Countries.GetCountryOrNoneFromDes(value);
        //        PropertyChange(this, "SHouseDes");
        //    }
        //}

        private readonly AI ai;

        public AutoCompleteFilterPredicate<object> TagFilter
        {
            get
            {
                return (searchText, obj) => AI.FuzzyLogic(obj as TagType, searchText);
            }
        }

        // Info
        public ObservableCollection<InfoValueClass> VeteranLevelInfo
        {
            get { return AIcore.VeteranLevelInfo.veteranLevels; }
            //set { _VeteranLevelInfo = value; PropertyChange(this, "VeteranLevelInfo"); }
        }
        public ObservableCollection<Country> CountryListInfo
        {
            get { return Countries.CountryListWithNone; }
            //set { _CountryListInfo = value; PropertyChange(this, "CountryListInfo"); }
        }
        public ObservableCollection<InfoValueClass> MindControlDecisionInfo
        {
            get { return AIcore.MindControlDecision.MindControlDecisionInfo; }
            //set { _MindControlDecisionInfo = value; PropertyChange(this, "MindControlDecisionInfo"); }
        }
        public ObservableCollection<InfoValueClass> GroupInfo
        {
            get { return AIcore.GroupInfo.Group; }
            //set { _GroupInfo = value; PropertyChange(this, "GroupInfo"); }
        }
        public ObservableCollection<string> WaypointsInfo
        {
            get { return AI.waypoints_info; }
            //set { _WaypointsInfo = value; PropertyChange(this, "WaypointsInfo"); }
        }
        public ObservableCollection<string> TransportWaypointsInfo
        {
            get { return AI.transwaypoints_info; }
            //set { _TransportWaypointsInfo = value; PropertyChange(this, "TransportWaypointsInfo"); }
        }
        public ObservableCollection<TaskForce> TaskForceInfo
        {
            get { return AI.taskforces_info; }
            //set { _TaskForceInfo = value; PropertyChange(this, "TaskForceInfo"); }
        }
        public ObservableCollection<ScriptType> ScriptTypeInfo
        {
            get { return AI.scripttypes_info; }
            //set { _ScriptTypeInfo = value; PropertyChange(this, "ScriptTypeInfo"); }
        }

        //public ObservableCollection<TaskForce> TaskForceInfo { get; set; }

        public static bool IsKeyOfTeamType(string key)
        {
            return key == "Name" || key == "Script" || key == "TaskForce" || key == "VeteranLevel" || key == "MindControlDecision" 
                || key == "Loadable" || key == "Full" || key == "Annoyance" || key == "GuardSlower" || key == "House" ||
                key == "Recruiter" || key == "Autocreate" || key == "Prebuild" || key == "Reinforce" ||
                key == "Droppod" || key == "UseTransportOrigin" || key == "Whiner" || key == "LooseRecruit" ||
                key == "Aggressive" || key == "Suicide" || key == "Priority" || key == "Max" ||
                key == "TechLevel" || key == "Group" || key == "OnTransOnly" || key == "AvoidThreats" ||
                key == "IonImmune" || key == "TransportsReturnOnUnload" || key == "AreTeamMembersRecruitable" ||
                key == "IsBaseDefense" || key == "OnlyTargetHouseEnemy" || key == "Tag" || key == "Waypoint" || 
                key == "TransportWaypoint";
        }
    }
}
