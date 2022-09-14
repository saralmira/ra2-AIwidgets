using AIcore.Types;
using System;
using System.Collections.ObjectModel;

namespace AIcore.TLists
{
    public class TeamTypes : TTypeLists<TeamType>
    {
        public TeamTypes(AI _ai) : base(_ai)
        {
            teamTypes = new ObservableCollection<TeamType>();
            current_tag = base_tag = 0x0C000000;

            foreach (string value in ini.GetValuesWithoutNotes("TeamTypes"))
            {
                teamTypes.Add(new TeamType(_ai, ini, value) { IsOriginalType = !ai.IsFakeAI });
            }
            GetNewTag();
            --current_tag;
        }

        public string GetNewTag()
        {
            return GenerateTag(teamTypes);
        }

        public void SaveIni(bool release = false)
        {
            ini.WriteValue("TeamTypes", null, null);
            if (release) ResetTag();
            for (int i = 0; i < teamTypes.Count; ++i)
            {
                teamTypes[i].Output(ini, release);
                ini.WriteValue("TeamTypes", i.ToString(), teamTypes[i]._tag);
            }
        }

        public TeamType GetTeamType(string tag)
        {
            return F_Find(teamTypes, tag);
        }

        public ObservableCollection<TeamType> teamTypes { get { return typelist; } private set { typelist = value; } }
    }
}
