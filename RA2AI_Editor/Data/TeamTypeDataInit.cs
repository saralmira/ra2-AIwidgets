using AIcore;
using AIcore.TLists;
using AIcore.Types;
using System;
using System.Collections.ObjectModel;

namespace RA2AI_Editor.Data
{
    public class TeamTypeDataInit : GeneralDataInit<TeamType>
    {
        public TeamTypeDataInit(AI _ai) : base(_ai)
        {
            ItemList = ai.teamTypes.teamTypes;
        }

        public NotifyList<TeamType> Find(TaskForce tf)
        {
            NotifyList<TeamType> ret = new NotifyList<TeamType>();
            foreach (TeamType tt in ItemList)
            {
                if (tt.Find(tf))
                {
                    ret.Add(tt);
                }
            }
            return ret;
        }

        public NotifyList<TeamType> Find(ScriptType st)
        {
            NotifyList<TeamType> ret = new NotifyList<TeamType>();
            foreach (TeamType tt in ItemList)
            {
                if (tt.Find(st))
                {
                    ret.Add(tt);
                }
            }
            return ret;
        }

        public override void SwitchMode(bool fake)
        {
            if (fakemode == fake)
                return;

            fakemode = fake;
            if (fakemode)
            {
                if (iniAnalyse == null)
                    return;
                TempList.Clear();
                CopyList(ItemList, TempList);
                ItemList.Clear();
                CopyList(iniAnalyse.cpTeamTypes, ItemList);
                ShowAnalysis(true);
            }
            else
            {
                ItemList.Clear();
                CopyList(TempList, ItemList);
                ShowAnalysis(false);
            }
        }

        public override void Delete(string tag, bool nohint = false)
        {
            if (fakemode)
                return;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            if (!nohint)
            {
                if (del_hint && MainWindow.MessageBoxShow(Local.Dictionary("MB_ITEMDELCONFIRM") + tag +
                Local.Dictionary("MB_ITEMDELCONFIRM2"), System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel)
                != System.Windows.MessageBoxResult.Yes)
                    return;
                del_hint = false;
            }
            GlobalCommandStack.Push(new DeleteTeamTypeCommand(tag, ai.teamTypes));
        }

        public override void Delete(TeamType teamtype, bool nohint = false)
        {
            if (fakemode)
                return;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            if (!nohint)
            {
                if (del_hint && MainWindow.MessageBoxShow(Local.Dictionary("MB_ITEMDELCONFIRM") +
                teamtype.PTag + " - " + teamtype.PName + Local.Dictionary("MB_ITEMDELCONFIRM2"), System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel)
                != System.Windows.MessageBoxResult.Yes)
                    return;
                del_hint = false;
            }
            GlobalCommandStack.Push(new DeleteTeamTypeCommand(teamtype, ai.teamTypes));
        }

        public void Recover(TeamType teamtype)
        {
            teamtype.Recover(ai.ini);
        }

        public override TeamType Duplicate(TeamType teamtype, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            PasteTeamTypeCommand cmd = new PasteTeamTypeCommand(teamtype, ai.teamTypes, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public override TeamType Add(int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            AddTeamTypeCommand cmd = new AddTeamTypeCommand(ai, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public TeamType AddTeamTypeWith(TaskForce tf, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));

            AddTeamTypeCommand cmd = new AddTeamTypeCommand(ai, tf, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public TeamType AddTeamTypeWith(ScriptType st, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));

            AddTeamTypeCommand cmd = new AddTeamTypeCommand(ai, st, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public TeamType AddTeamTypeWith(TaskForce tf, ScriptType st, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));

            AddTeamTypeCommand cmd = new AddTeamTypeCommand(ai, tf, st, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public void Filter(string str)
        {
            if (str == null || str.Length == 0)
            {
                foreach (TeamType st in ItemList)
                    st.PTVisibility = true;
            }
            else
            {
                StringComparison sc = MainWindow.configData.Search_InCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                foreach (TeamType st in ItemList)
                {
                    if ((MainWindow.configData.Search_ID && st.PTag.IndexOf(str, sc) >= 0) || 
                        (MainWindow.configData.Search_Name && st.PName.IndexOf(str, sc) >= 0))
                        st.PTVisibility = true;
                    else
                        st.PTVisibility = false;
                }
            }
        }

        public class AddTeamTypeCommand : AddTypeCommandClass<TeamTypes, TeamType>
        {
            public AddTeamTypeCommand(AI ai, int _index) : base(ai.teamTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDTT");
                TypeObject = new TeamType(ai, tlist.GetNewTag(), AI.NullScriptType, AI.NullTaskForce);
            }

            public AddTeamTypeCommand(AI ai, TaskForce taskForce, int _index) : base(ai.teamTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDTT");
                TypeObject = new TeamType(ai, tlist.GetNewTag(), AI.NullScriptType, taskForce);
            }

            public AddTeamTypeCommand(AI ai, ScriptType scriptType, int _index) : base(ai.teamTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDTT");
                TypeObject = new TeamType(ai, tlist.GetNewTag(), scriptType, AI.NullTaskForce);
            }

            public AddTeamTypeCommand(AI ai, TaskForce taskForce, ScriptType scriptType, int _index) : base(ai.teamTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDTT");
                TypeObject = new TeamType(ai, tlist.GetNewTag(), scriptType, taskForce);
            }
        }

        public class PasteTeamTypeCommand : AddTypeCommandClass<TeamTypes, TeamType>
        {
            public PasteTeamTypeCommand(TeamType cp, TeamTypes teamTypes, int _index) : base(teamTypes, _index)
            {
                Name = Local.Dictionary("CMD_CPYTT");
                TypeObject = cp.CloneType(tlist.GetNewTag());
                if (MainWindow.configData.Append_WhileCopy)
                    TypeObject.PName += " Copy";
            }
        }

        public class DeleteTeamTypeCommand : DeleteTypeCommandClass<TeamTypes, TeamType>
        {
            public DeleteTeamTypeCommand(TeamType cp, TeamTypes teamTypes) : base(cp, teamTypes)
            {
                Name = Local.Dictionary("CMD_DELTT");
            }

            public DeleteTeamTypeCommand(string deltag, TeamTypes teamTypes) : base(deltag, teamTypes)
            {
                Name = Local.Dictionary("CMD_DELTT");
            }
        }
    }

}
