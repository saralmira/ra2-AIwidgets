using AIcore.Types;
using System;
using System.Collections.ObjectModel;

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

        public void SaveIni()
        {
            ai.ini.WriteValue("AITriggerTypes", null, null);
            foreach (Types.AITriggerType a in aITriggerTypes)
                a.Output(ai.ini);
        }

        public AITriggerType GetAITriggerType(string tag)
        {
            return F_Find(aITriggerTypes, tag);
        }

        public ObservableCollection<AITriggerType> aITriggerTypes { get { return typelist; } private set { typelist = value; } }
    }
}
