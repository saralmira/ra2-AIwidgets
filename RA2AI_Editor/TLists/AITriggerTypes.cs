using AIcore.Types;
using System;
using System.Collections.ObjectModel;
using static AIcore.TeamTypeInfo;

namespace AIcore.TLists
{
    public class AITriggerTypes : TTypeLists<AITriggerType>
    {
        public AITriggerTypes(AI _ai) : base(_ai)
        {
            aITriggerTypes = new ObservableCollection<AITriggerType>();
            current_tag = base_tag = 0x0D000000;

            foreach (string key in ai.ini.GetKeys("AITriggerTypes"))
            {
                aITriggerTypes.Add(new AITriggerType(ai, key) { IsOriginalType = !ai.IsFakeAI });
            }
            GetNewTag();
            --current_tag;
        }

        public string GetNewTag()
        {
            return GenerateTag(aITriggerTypes);
        }

        public void SaveIni(bool release = false)
        {
            ai.ini.WriteValue("AITriggerTypes", null, null);
            if (release) ResetTag();
            for (int i = 0; i < aITriggerTypes.Count; ++i)
            {
                aITriggerTypes[i].Output(ini, release);
            }
        }

        public AITriggerType GetAITriggerType(string tag)
        {
            return F_Find(aITriggerTypes, tag);
        }

        public ObservableCollection<AITriggerType> aITriggerTypes { get { return typelist; } private set { typelist = value; } }
    }
}
