using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIcore;
using AIcore.Types;
using AIcore.TLists;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace RA2AI_Editor
{
    public class IniAnalyse
    {
        readonly AI curai;
        AI tar;

        public ObservableCollection<TaskForce> cpTaskForces;
        public ObservableCollection<ScriptType> cpScriptTypes;
        public ObservableCollection<TeamType> cpTeamTypes;
        public ObservableCollection<AITriggerType> cpAITriggerTypes;

        // What the fuck
        public ObservableCollection<TaskForce> TaskForces1_Diff;
        public ObservableCollection<ScriptType> ScriptTypes1_Diff;
        public ObservableCollection<TeamType> TeamTypes1_Diff;
        public ObservableCollection<AITriggerType> AITriggerTypes1_Diff;
        public ObservableCollection<TaskForce> TaskForces2_Diff;
        public ObservableCollection<ScriptType> ScriptTypes2_Diff;
        public ObservableCollection<TeamType> TeamTypes2_Diff;
        public ObservableCollection<AITriggerType> AITriggerTypes2_Diff;
        public ObservableCollection<TaskForce> TaskForces1_Left;
        public ObservableCollection<ScriptType> ScriptTypes1_Left;
        public ObservableCollection<TeamType> TeamTypes1_Left;
        public ObservableCollection<AITriggerType> AITriggerTypes1_Left;
        public ObservableCollection<TaskForce> TaskForces2_Left;
        public ObservableCollection<ScriptType> ScriptTypes2_Left;
        public ObservableCollection<TeamType> TeamTypes2_Left;
        public ObservableCollection<AITriggerType> AITriggerTypes2_Left;
        public ObservableCollection<TaskForce> TaskForces_Same;
        public ObservableCollection<ScriptType> ScriptTypes_Same;
        public ObservableCollection<TeamType> TeamTypes_Same;
        public ObservableCollection<AITriggerType> AITriggerTypes_Same;

        public IniAnalyse(AI current_ai, string target)
        {
            curai = current_ai;
            tar = new AI(target, true);
            cpTaskForces = new ObservableCollection<TaskForce>();
            cpScriptTypes = new ObservableCollection<ScriptType>();
            cpTeamTypes = new ObservableCollection<TeamType>();
            cpAITriggerTypes = new ObservableCollection<AITriggerType>();

            TaskForces1_Diff = new ObservableCollection<TaskForce>();
            ScriptTypes1_Diff = new ObservableCollection<ScriptType>();
            TeamTypes1_Diff = new ObservableCollection<TeamType>();
            AITriggerTypes1_Diff = new ObservableCollection<AITriggerType>();
            TaskForces2_Diff = new ObservableCollection<TaskForce>();
            ScriptTypes2_Diff = new ObservableCollection<ScriptType>();
            TeamTypes2_Diff = new ObservableCollection<TeamType>();
            AITriggerTypes2_Diff = new ObservableCollection<AITriggerType>();
            TaskForces1_Left = new ObservableCollection<TaskForce>();
            ScriptTypes1_Left = new ObservableCollection<ScriptType>();
            TeamTypes1_Left = new ObservableCollection<TeamType>();
            AITriggerTypes1_Left = new ObservableCollection<AITriggerType>();
            TaskForces2_Left = new ObservableCollection<TaskForce>();
            ScriptTypes2_Left = new ObservableCollection<ScriptType>();
            TeamTypes2_Left = new ObservableCollection<TeamType>();
            AITriggerTypes2_Left = new ObservableCollection<AITriggerType>();
            TaskForces_Same = new ObservableCollection<TaskForce>();
            ScriptTypes_Same = new ObservableCollection<ScriptType>();
            TeamTypes_Same = new ObservableCollection<TeamType>();
            AITriggerTypes_Same = new ObservableCollection<AITriggerType>();
        }

        public void Generate()
        {
            foreach (TaskForce tf in curai.taskForces.taskForces)
            {
                tf.HighLight = false;
                TaskForce g = tar.taskForces.GetTaskForce(tf._tag);
                if (g != null && g != AI.NullTaskForce)
                {
                    if (tf.CompareWith(g))
                    {
                        tf.AnalysisResult = AnalysisResult.Same;
                        TaskForces_Same.Add(tf);
                    }
                    else
                    {
                        tf.AnalysisResult = AnalysisResult.Different;
                        tf.CompareType = g;
                        g.CompareType = tf;
                        TaskForces1_Diff.Add(tf);
                        TaskForces2_Diff.Add(g);
                    }
                    g.HighLight = false;
                    cpTaskForces.Add(tf);
                    tar.taskForces.Delete(g);
                }
                else
                {
                    tf.AnalysisResult = AnalysisResult.OnlyInSource;
                    TaskForces1_Left.Add(tf);
                    cpTaskForces.Add(tf);
                }
            }
            foreach (TaskForce g in tar.taskForces.taskForces)
            {
                g.HighLight = false;
                g.AnalysisResult = AnalysisResult.OnlyInTarget;
                TaskForces2_Left.Add(g);
                cpTaskForces.Add(g);
            }

            foreach (ScriptType st in curai.scriptTypes.scriptTypes)
            {
                st.HighLight = false;
                ScriptType g = tar.scriptTypes.GetScriptType(st._tag);
                if (g != null && g != AI.NullScriptType)
                {
                    if (st.CompareWith(g))
                    {
                        st.AnalysisResult = AnalysisResult.Same;
                        ScriptTypes_Same.Add(st);
                    }
                    else
                    {
                        st.AnalysisResult = AnalysisResult.Different;
                        st.CompareType = g;
                        g.CompareType = st;
                        ScriptTypes1_Diff.Add(st);
                        ScriptTypes2_Diff.Add(g);
                    }
                    g.HighLight = false;
                    cpScriptTypes.Add(st);
                    tar.scriptTypes.Delete(g);
                }
                else
                {
                    st.AnalysisResult = AnalysisResult.OnlyInSource;
                    ScriptTypes1_Left.Add(st);
                    cpScriptTypes.Add(st);
                }
            }
            foreach (ScriptType g in tar.scriptTypes.scriptTypes)
            {
                g.HighLight = false;
                g.AnalysisResult = AnalysisResult.OnlyInTarget;
                ScriptTypes2_Left.Add(g);
                cpScriptTypes.Add(g);
            }

            foreach (TeamType st in curai.teamTypes.teamTypes)
            {
                st.HighLight = false;
                TeamType g = tar.teamTypes.GetTeamType(st._tag);
                if (g != null && g != AI.NullTeamType)
                {
                    if (st.CompareWith(g))
                    {
                        st.AnalysisResult = AnalysisResult.Same;
                        TeamTypes_Same.Add(st);
                    }
                    else
                    {
                        st.AnalysisResult = AnalysisResult.Different;
                        st.CompareType = g;
                        g.CompareType = st;
                        TeamTypes1_Diff.Add(st);
                        TeamTypes2_Diff.Add(g);
                    }
                    g.HighLight = false;
                    cpTeamTypes.Add(st);
                    tar.teamTypes.Delete(g);
                }
                else
                {
                    st.AnalysisResult = AnalysisResult.OnlyInSource;
                    TeamTypes1_Left.Add(st);
                    cpTeamTypes.Add(st);
                }
            }
            foreach (TeamType g in tar.teamTypes.teamTypes)
            {
                g.HighLight = false;
                g.AnalysisResult = AnalysisResult.OnlyInTarget;
                TeamTypes2_Left.Add(g);
                cpTeamTypes.Add(g);
            }

            foreach (AITriggerType st in curai.aITriggerTypes.aITriggerTypes)
            {
                st.HighLight = false;
                AITriggerType g = tar.aITriggerTypes.GetAITriggerType(st._tag);
                if (g != null)
                {
                    if (st.CompareWith(g))
                    {
                        st.AnalysisResult = AnalysisResult.Same;
                        AITriggerTypes_Same.Add(st);
                    }
                    else
                    {
                        st.AnalysisResult = AnalysisResult.Different;
                        st.CompareType = g;
                        g.CompareType = st;
                        AITriggerTypes1_Diff.Add(st);
                        AITriggerTypes2_Diff.Add(g);
                    }
                    g.HighLight = false;
                    cpAITriggerTypes.Add(st);
                    tar.aITriggerTypes.Delete(g);
                }
                else
                {
                    st.AnalysisResult = AnalysisResult.OnlyInSource;
                    AITriggerTypes1_Left.Add(st);
                    cpAITriggerTypes.Add(st);
                }
            }
            foreach (AITriggerType g in tar.aITriggerTypes.aITriggerTypes)
            {
                g.HighLight = false;
                g.AnalysisResult = AnalysisResult.OnlyInTarget;
                AITriggerTypes2_Left.Add(g);
                cpAITriggerTypes.Add(g);
            }
        }

        public TaskForce FindType1(TaskForce taskForce)
        {
            foreach (TaskForce t in TaskForces1_Diff)
                if (t._tag == taskForce._tag)
                    return t;
            foreach (TaskForce t in TaskForces_Same)
                if (t._tag == taskForce._tag)
                    return t;
            return null;
        }

        public TaskForce FindType2(TaskForce taskForce)
        {
            foreach (TaskForce t in TaskForces2_Diff)
                if (t._tag == taskForce._tag)
                    return t;
            foreach (TaskForce t in TaskForces_Same)
                if (t._tag == taskForce._tag)
                    return t;
            return null;
        }

        public ScriptType FindType1(ScriptType scriptType)
        {
            foreach (ScriptType t in ScriptTypes1_Diff)
                if (t._tag == scriptType._tag)
                    return t;
            foreach (ScriptType t in ScriptTypes_Same)
                if (t._tag == scriptType._tag)
                    return t;
            return null;
        }

        public ScriptType FindType2(ScriptType scriptType)
        {
            foreach (ScriptType t in ScriptTypes2_Diff)
                if (t._tag == scriptType._tag)
                    return t;
            foreach (ScriptType t in ScriptTypes_Same)
                if (t._tag == scriptType._tag)
                    return t;
            return null;
        }

        public TeamType FindType1(TeamType teamType)
        {
            foreach (TeamType t in TeamTypes1_Diff)
                if (t._tag == teamType._tag)
                    return t;
            foreach (TeamType t in TeamTypes_Same)
                if (t._tag == teamType._tag)
                    return t;
            return null;
        }

        public TeamType FindType2(TeamType teamType)
        {
            foreach (TeamType t in TeamTypes2_Diff)
                if (t._tag == teamType._tag)
                    return t;
            foreach (TeamType t in TeamTypes_Same)
                if (t._tag == teamType._tag)
                    return t;
            return null;
        }

        public AITriggerType FindType1(AITriggerType aITriggerType)
        {
            foreach (AITriggerType t in AITriggerTypes1_Diff)
                if (t._tag == aITriggerType._tag)
                    return t;
            foreach (AITriggerType t in AITriggerTypes_Same)
                if (t._tag == aITriggerType._tag)
                    return t;
            return null;
        }

        public AITriggerType FindType2(AITriggerType aITriggerType)
        {
            foreach (AITriggerType t in AITriggerTypes2_Diff)
                if (t._tag == aITriggerType._tag)
                    return t;
            foreach (AITriggerType t in AITriggerTypes_Same)
                if (t._tag == aITriggerType._tag)
                    return t;
            return null;
        }

        public void Clear()
        {
            cpTaskForces.Clear();
            cpScriptTypes.Clear();
            cpTeamTypes.Clear();
            cpAITriggerTypes.Clear();
            TaskForces1_Diff.Clear();
            ScriptTypes1_Diff.Clear();
            TeamTypes1_Diff.Clear();
            AITriggerTypes1_Diff.Clear();
            TaskForces2_Diff.Clear();
            ScriptTypes2_Diff.Clear();
            TeamTypes2_Diff.Clear();
            AITriggerTypes2_Diff.Clear();
            TaskForces1_Left.Clear();
            ScriptTypes1_Left.Clear();
            TeamTypes1_Left.Clear();
            AITriggerTypes1_Left.Clear();
            TaskForces2_Left.Clear();
            ScriptTypes2_Left.Clear();
            TeamTypes2_Left.Clear();
            AITriggerTypes2_Left.Clear();
            TaskForces_Same.Clear();
            ScriptTypes_Same.Clear();
            TeamTypes_Same.Clear();
            AITriggerTypes_Same.Clear();
        }

        public string Path { get { return tar.FilePath; } }

        public bool ShowResults;
    }
}
