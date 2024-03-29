﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Library;
using AIcore;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AIcore.Types
{
    public class TeamType : TeamTypeBase
    {
        public TeamType(AI _ai, string tag, ScriptType scriptType, TaskForce taskForce) : base(_ai, tag)
        {
            Name = tag == null ? "" : "_TeamType_";

            Script = scriptType;
            TaskForce = taskForce;

            Ext_EasyMode_Type = new TeamTypeBase(_ai, null);
            Ext_MediumMode_Type = new TeamTypeBase(_ai, null);
            Ext_HardMode_Type = new TeamTypeBase(_ai, null);

            //VeteranLevelInfo = AIcore.VeteranLevelInfo.veteranLevels;
            //CountryListInfo = Countries.CountryListWithNone;
            //MindControlDecisionInfo = AIcore.MindControlDecision.MindControlDecisionInfo;
            //GroupInfo = AIcore.GroupInfo.Group;
            //WaypointsInfo = AI.waypoints_info;
            //TransportWaypointsInfo = AI.transwaypoints_info;
            //TaskForceInfo = AI.taskforces_info;
            //ScriptTypeInfo = AI.scripttypes_info;
        }

        public TeamType(AI _ai, IniClass ini, string tag) : base(_ai, tag)
        {
            rec_tag = tag;
            NewType = false;

            Ext_EasyMode_Type = new TeamTypeBase(_ai, null);
            Ext_MediumMode_Type = new TeamTypeBase(_ai, null);
            Ext_HardMode_Type = new TeamTypeBase(_ai, null);

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
            MindControlDecision = AIcore.MindControlDecision.GetMindControlDecision(ini.ReadIntValue(tag, "MindControlDecision", 0));
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

            base.Init(ini, tag);

            EnableExt = ini.ReadBoolValue(tag, nameof(EnableExt), false);
            Ext_EasyMode = ini.ReadBoolValue(tag, nameof(Ext_EasyMode), true);
            Ext_MediumMode = ini.ReadBoolValue(tag, nameof(Ext_MediumMode), true);
            Ext_HardMode = ini.ReadBoolValue(tag, nameof(Ext_HardMode), true);
            Ext_EasyMode_Type.STaskForce = ini.ReadValueWithoutNotes(tag, "Ext_EasyMode_Type.TaskForce", STaskForce);
            Ext_EasyMode_Type.SScript = ini.ReadValueWithoutNotes(tag, "Ext_EasyMode_Type.Script", SScript);
            Ext_MediumMode_Type.STaskForce = ini.ReadValueWithoutNotes(tag, "Ext_MediumMode_Type.TaskForce", STaskForce);
            Ext_MediumMode_Type.SScript = ini.ReadValueWithoutNotes(tag, "Ext_MediumMode_Type.Script", SScript);
            Ext_HardMode_Type.STaskForce = ini.ReadValueWithoutNotes(tag, "Ext_HardMode_Type.TaskForce", STaskForce);
            Ext_HardMode_Type.SScript = ini.ReadValueWithoutNotes(tag, "Ext_HardMode_Type.Script", SScript);

        }

        public override int GetTechLevelMax()
        {
            if (EnableExt)
            {
                return Math.Max(Math.Max(Ext_EasyMode_Type.GetTechLevelMax(),
                    Ext_MediumMode_Type.GetTechLevelMax()),
                    Ext_HardMode_Type.GetTechLevelMax());
            }
            else
            {
                return TaskForce != null && TaskForce != AI.NullTaskForce ? TaskForce.GetTechLevelMax() : 0;
            }
        }

        public override void Output(IniClass ini, bool release)
        {
            ini.WriteValue(_tag, null, null);
            ini.WriteValue(_tag, "Name", Name);
            ini.WriteValue(_tag, "VeteranLevel", VeteranLevel.ToString());
            if (IsGameYR)
            {
                ini.WriteValue(_tag, "MindControlDecision", MindControlDecision.Value);
            }

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
            {
                ini.WriteValue(_tag, "UseTransportOrigin", "yes");
            }
            else
            {
                ini.WriteValue(_tag, "UseTransportOrigin", "no");
            }

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
            {
                ini.WriteValue(_tag, "Tag", Tag);
            }

            // ext
            if (!release && EnableExt)
            {
                ini.WriteValue(_tag, nameof(EnableExt), EnableExt);
                ini.WriteValue(_tag, nameof(Ext_EasyMode), Ext_EasyMode);
                ini.WriteValue(_tag, nameof(Ext_MediumMode), Ext_MediumMode);
                ini.WriteValue(_tag, nameof(Ext_HardMode), Ext_HardMode);
                if (Ext_EasyMode)
                {
                    ini.WriteValue(_tag, "Ext_EasyMode_Type.TaskForce", Ext_EasyMode_Type.STaskForce);
                    ini.WriteValue(_tag, "Ext_EasyMode_Type.Script", Ext_EasyMode_Type.SScript);
                }
                if (Ext_MediumMode)
                {
                    ini.WriteValue(_tag, "Ext_MediumMode_Type.TaskForce", Ext_MediumMode_Type.STaskForce);
                    ini.WriteValue(_tag, "Ext_MediumMode_Type.Script", Ext_MediumMode_Type.SScript);
                }
                if (Ext_HardMode)
                {
                    ini.WriteValue(_tag, "Ext_HardMode_Type.TaskForce", Ext_HardMode_Type.STaskForce);
                    ini.WriteValue(_tag, "Ext_HardMode_Type.Script", Ext_HardMode_Type.SScript);
                }
            }

            base.Output(ini, release);
        }

        private string YesOrNo(bool flag)
        {
            return flag ? "yes" : "no";
        }

        public bool CompareWith(TeamType a)
        {
            return a.PTag == PTag && a.SScript == SScript && a.STaskForce == STaskForce &&
                a.SHouse == SHouse && a.VeteranLevel == VeteranLevel && (!IsGameYR || MindControlDecision == null || a.MindControlDecision == null ||
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
            var ret = new TeamType(ai, tag, this.Script, this.TaskForce)
            {
                PName = this.PName,
                VeteranLevel = this.VeteranLevel,
                SMindControlDecision = this.MindControlDecision.Value,
                Priority = this.Priority,
                Max = this.Max,
                TechLevel = this.TechLevel,
                Group = this.Group,
                SWaypoint = this.SWaypoint,
                STransportWaypoint = this.STransportWaypoint,

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
                Tag = this.Tag,

                // ext
                EnableExt = this.EnableExt,
                Ext_EasyMode = this.Ext_EasyMode,
                Ext_MediumMode = this.Ext_MediumMode,
                Ext_HardMode = this.Ext_HardMode
            };

            // ext
            if (EnableExt)
            {
                ret.Ext_EasyMode_Type.STaskForce = this.Ext_EasyMode_Type.STaskForce;
                ret.Ext_EasyMode_Type.SScript = this.Ext_EasyMode_Type.SScript;
                ret.Ext_MediumMode_Type.STaskForce = this.Ext_MediumMode_Type.STaskForce;
                ret.Ext_MediumMode_Type.SScript = this.Ext_MediumMode_Type.SScript;
                ret.Ext_HardMode_Type.STaskForce = this.Ext_HardMode_Type.STaskForce;
                ret.Ext_HardMode_Type.SScript = this.Ext_HardMode_Type.SScript;
            }
            
            return ret;
        }

        public bool Find(TaskForce tf)
        {
            return this.TaskForce == tf ||
                    (this.EnableExt && ((this.Ext_EasyMode && this.Ext_EasyMode_Type.TaskForce == tf) ||
                    (this.Ext_MediumMode && this.Ext_MediumMode_Type.TaskForce == tf) ||
                    (this.Ext_HardMode && this.Ext_HardMode_Type.TaskForce == tf)));
        }

        public bool Find(ScriptType st)
        {
            return this.Script == st ||
                    (this.EnableExt && ((this.Ext_EasyMode && this.Ext_EasyMode_Type.Script == st) ||
                    (this.Ext_MediumMode && this.Ext_MediumMode_Type.Script == st) ||
                    (this.Ext_HardMode && this.Ext_HardMode_Type.Script == st)));
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
            get => _OnlyTargetHouseEnemy;
            set { _OnlyTargetHouseEnemy = value; PropertyChange(this, "OnlyTargetHouseEnemy"); }
        }

        private string _Tag = null;
        public string Tag
        {
            get => _Tag;
            set { _Tag = value; PropertyChange(this, "Tag"); }
        }

        private Country _House = Countries.All;
        public Country House
        {
            get => _House;
            set
            {
                _House = value;
                PropertyChange(this, "House");
            }
        }
        public string SHouse
        {
            get => House.NameOrNone;
            set => House = Countries.GetCountryOrNone(value);
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

        public bool IsGameYR => Game.CurrentGame.GameType == Game.GameType.YR;

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
        public bool EnableExtInRelease { get { return EnableExt || TaskForce.EnableExt; } }

        private bool _Ext_EasyMode = true;
        public bool Ext_EasyMode { get => _Ext_EasyMode; set { _Ext_EasyMode = value; PropertyChange(nameof(Ext_EasyMode)); } }
        public TeamTypeBase Ext_EasyMode_Type { get; set; }
        public TeamType Get_Easy_Type { get => Get_Ext_Type(Ext_EasyMode_Type, Ext_EasyMode, 1); }

        private bool _Ext_MediumMode = true;
        public bool Ext_MediumMode { get => _Ext_MediumMode; set { _Ext_MediumMode = value; PropertyChange(nameof(Ext_MediumMode)); } }
        public TeamTypeBase Ext_MediumMode_Type { get; set; }
        public TeamType Get_Medium_Type { get => Get_Ext_Type(Ext_MediumMode_Type, Ext_MediumMode, 2); }

        private bool _Ext_HardMode = true;
        public bool Ext_HardMode { get => _Ext_HardMode; set { _Ext_HardMode = value; PropertyChange(nameof(Ext_HardMode)); } }
        public TeamTypeBase Ext_HardMode_Type { get; set; }
        public TeamType Get_Hard_Type { get => Get_Ext_Type(Ext_HardMode_Type, Ext_HardMode, 3); }

        public (TaskForce, TaskForce, TaskForce) GetExtTaskForce(TaskForce tf)
        {
            if (!tf.EnableExt)
                return (tf, tf, tf);

            return (tf.CloneFromBase(tf.Ext_EasyMode_Type, ai.taskForces.GetNewTag(), " - E"),
                tf.CloneFromBase(tf.Ext_MediumMode_Type, ai.taskForces.GetNewTag(), " - M"),
                tf.CloneFromBase(tf.Ext_HardMode_Type, ai.taskForces.GetNewTag(), " - H"));
        }

        private void FromCache(TaskForce t, out (TaskForce, TaskForce, TaskForce) out_type)
        {
            if (t == AI.NullTaskForce) out_type = (t, t, t);
            else if (!ai.releaseTaskForces.TryGetValue(t, out out_type))
            {
                // remove totally
                t.RemoveSectionFromIni(ai.ini);
                ai.taskForces.Delete(t);
                ai.releaseTaskForces[t] = out_type = GetExtTaskForce(t);
                ai.taskForces.TryAdd(out_type.Item1);
                ai.taskForces.TryAdd(out_type.Item2);
                ai.taskForces.TryAdd(out_type.Item3);
            }
        }

        public TeamType Get_Ext_Type(TeamTypeBase ttb, bool extmode, int mode)
        {
            var tf = ttb.TaskForce;
            if (!EnableExt && !tf.EnableExt)
                return this;

            if (!extmode)
                return this;

            FromCache(tf, out (TaskForce, TaskForce, TaskForce) out_tf);
            string modefix;
            switch (mode)
            {
                case 1: // easy
                    modefix = " - E";
                    tf = out_tf.Item1;
                    break;
                case 2: // medium
                    modefix = " - M";
                    tf = out_tf.Item2;
                    break;
                case 3: // hard
                    modefix = " - H";
                    tf = out_tf.Item3;
                    break;
                default:
                    return this;
            }

            var ext = this.CloneType(ai.teamTypes.GetNewTag());
            ext.PName = NameWithExt(this.Name, modefix);
            ext.Script = ttb.Script;
            ext.TaskForce = tf;
            return ext;
        }

        public AutoCompleteFilterPredicate<object> TagFilter => (searchText, obj) => AI.FuzzyLogic(obj as TagType, searchText);

        // Info
        public ObservableCollection<InfoValueClass> VeteranLevelInfo => AIcore.VeteranLevelInfo.veteranLevels;
        public ObservableCollection<Country> CountryListInfo => Countries.CountryListWithNone;
        public ObservableCollection<InfoValueClass> MindControlDecisionInfo => AIcore.MindControlDecision.MindControlDecisionInfo;
        public ObservableCollection<InfoValueClass> GroupInfo => AIcore.GroupInfo.Group;
        public ObservableCollection<string> WaypointsInfo => AI.waypoints_info;
        public ObservableCollection<string> TransportWaypointsInfo => AI.transwaypoints_info;
        public ObservableCollection<TaskForce> TaskForceInfo => AI.taskforces_info;
        public ObservableCollection<ScriptType> ScriptTypeInfo => AI.scripttypes_info;

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

    public class TeamTypeBase : OType
    {
        public TeamTypeBase(AI _ai, string tag) : base(tag)
        {
            this.ai = _ai;
        }

        protected ScriptType _Script = AI.NullScriptType;
        public ScriptType Script
        {
            get => _Script;
            set
            {
                _Script = value ?? AI.NullScriptType;
                _SScript = _Script.PTag;
                PropertyChange(nameof(SScript));
                PropertyChange(nameof(SScriptTypeName));
            }
        }
        protected string _SScript = AI.NullScriptType.PTag;
        public string SScript
        {
            get => _SScript;
            set
            {
                _SScript = value;
                _Script = ai != null ? ai.GetScriptType(value) : AI.NullScriptType;
                PropertyChange(nameof(SScript));
                PropertyChange(nameof(SScriptTypeName));
            }
        }
        public string SScriptTypeName => Script != null && Script != AI.NullScriptType ? Script.PName : AI.NullScriptType.PTag;

        protected TaskForce _TaskForce = AI.NullTaskForce;
        public TaskForce TaskForce
        {
            get => _TaskForce;
            set
            {
                _TaskForce = value ?? AI.NullTaskForce;
                _STaskForce = _TaskForce.PTag;
                PropertyChange(nameof(STaskForce));
                PropertyChange(nameof(STaskForceName));
            }
        }
        protected string _STaskForce = AI.NullTaskForce.PTag;
        public string STaskForce
        {
            get => _STaskForce;
            set
            {
                _STaskForce = value;
                _TaskForce = ai == null ? AI.NullTaskForce : ai.GetTaskForce(_STaskForce);
                PropertyChange(nameof(STaskForce));
                PropertyChange(nameof(STaskForceName));
            }
        }
        public string STaskForceName => TaskForce != null && TaskForce != AI.NullTaskForce ? TaskForce.PName : AI.NullTaskForce.PTag;

        protected readonly AI ai;

        public override void Output(IniClass ini, bool release)
        {
            ini.WriteValue(_tag, "Script", Script == null ? SScript : Script._tag);
            ini.WriteValue(_tag, "TaskForce", TaskForce == null ? STaskForce : TaskForce._tag);
        }

        protected override void Init(IniClass ini, string tag)
        {
            SScript = ini.ReadValueWithoutNotes(tag, "Script");
            STaskForce = ini.ReadValueWithoutNotes(tag, "TaskForce");
        }

        public virtual int GetTechLevelMax()
        {
            return TaskForce == null ? 0 : TaskForce.GetTechLevelMax();
        }
    }

}
