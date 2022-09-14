using AIcore.Types;
using System;
using System.Collections.ObjectModel;

namespace AIcore.TLists
{
    public class ScriptTypes : TTypeLists<ScriptType>
    {
        public ScriptTypes(AI _ai) : base(_ai)
        {
            scriptTypes = new ObservableCollection<ScriptType>();
            current_tag = base_tag = 0x0B000000;

            foreach (string value in ini.GetValuesWithoutNotes("ScriptTypes"))
            {
                scriptTypes.Add(new ScriptType(ini, value) { IsOriginalType = !ai.IsFakeAI });
            }
            GetNewTag();
            --current_tag;
        }

        public string GetNewTag()
        {
            return GenerateTag(scriptTypes);
        }

        public void SaveIni(bool release = false)
        {
            ini.WriteValue("ScriptTypes", null, null);
            if (release) ResetTag();
            for (int i = 0; i < scriptTypes.Count; ++i)
            {
                scriptTypes[i].Output(ini, release);
                ini.WriteValue("ScriptTypes", i.ToString(), scriptTypes[i]._tag);
            }
        }

        public ScriptType GetScriptType(string tag)
        {
            return F_Find(scriptTypes, tag);
        }

        public ObservableCollection<ScriptType> scriptTypes { get { return typelist; } private set { typelist = value; } }
    }
}
