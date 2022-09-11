using System;
using System.Collections.ObjectModel;
using AIcore.Types;

namespace AIcore.TLists
{
    public class TaskForces : TTypeLists<TaskForce>
    {
        public TaskForces(AI _ai) : base(_ai)
        {
            taskForces = new ObservableCollection<TaskForce>();
            current_tag = base_tag = 0x0A000000;

            foreach (string value in ini.GetValuesWithoutNotes("TaskForces"))
            {
                taskForces.Add(new TaskForce(ini, value) { IsOriginalType = !ai.IsFakeAI });
            }
            GetNewTag();
            --current_tag;
        }

        public string GetNewTag()
        {
            return GenerateTag(taskForces);
        }

        public void SaveIni(bool release = false)
        {
            ini.WriteValue("TaskForces", null, null);
            for (int i = 0; i < taskForces.Count; ++i)
            {
                taskForces[i].Output(ini, release);
                ini.WriteValue("TaskForces", i.ToString(), taskForces[i]._tag);
            }
        }

        public TaskForce GetTaskForce(string tag)
        {
            return F_Find(taskForces, tag);
        }

        public ObservableCollection<TaskForce> taskForces { get { return typelist; } private set { typelist = value; } }

    }
}
