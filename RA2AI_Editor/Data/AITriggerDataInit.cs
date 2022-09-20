using AIcore;
using AIcore.TLists;
using AIcore.Types;
using System;
using System.Collections.ObjectModel;

namespace RA2AI_Editor.Data
{
    public class AITriggerDataInit : GeneralDataInit<AITriggerType>
    {
        public AITriggerDataInit(AI _ai) : base(_ai)
        {
            ItemList = ai.aITriggerTypes.aITriggerTypes;
        }

        public NotifyList<AITriggerType> Find(TeamType tt)
        {
            NotifyList<AITriggerType> ret = new NotifyList<AITriggerType>();
            foreach (AITriggerType at in ItemList)
            {
                if (at.TeamType1 == tt || at.TeamType2 == tt)
                    ret.Add(at);
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
                CopyList(iniAnalyse.cpAITriggerTypes, ItemList);
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
            GlobalCommandStack.Push(new DeleteTriggerCommand(tag, ai.aITriggerTypes));
        }

        public override void Delete(AITriggerType aITriggerType, bool nohint = false)
        {
            if (fakemode)
                return;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            if (!nohint)
            {
                if (del_hint && MainWindow.MessageBoxShow(Local.Dictionary("MB_ITEMDELCONFIRM") +
                aITriggerType.PTag + " - " + aITriggerType.PName + Local.Dictionary("MB_ITEMDELCONFIRM2"), System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel)
                != System.Windows.MessageBoxResult.Yes)
                    return;
                del_hint = false;
            }
            GlobalCommandStack.Push(new DeleteTriggerCommand(aITriggerType, ai.aITriggerTypes));
        }

        public void Recover(AITriggerType aITriggerType)
        {
            aITriggerType.Recover(ai.ini);
        }

        public override AITriggerType Duplicate(AITriggerType aITriggerType, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));

            PasteTriggerCommand cmd = new PasteTriggerCommand(aITriggerType, ai.aITriggerTypes, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public override AITriggerType Add(int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));

            AddTriggerCommand cmd = new AddTriggerCommand(ai, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public AITriggerType AddAITriggerWith(TeamType tt, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));

            AddTriggerCommand cmd = new AddTriggerCommand(ai, tt, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public void Filter(string str)
        {
            if (str == null || str.Length == 0)
            {
                foreach (AITriggerType st in ItemList)
                    st.PTVisibility = true;
            }
            else
            {
                StringComparison sc = MainWindow.configData.Search_InCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                foreach (AITriggerType st in ItemList)
                {
                    if ((MainWindow.configData.Search_ID && st.PTag.IndexOf(str, sc) >= 0) || 
                        (MainWindow.configData.Search_Name && st.PName.IndexOf(str, sc) >= 0))
                        st.PTVisibility = true;
                    else
                        st.PTVisibility = false;
                }
            }
        }

        public class AddTriggerCommand : AddTypeCommandClass<AITriggerTypes, AITriggerType>
        {
            public AddTriggerCommand(AI ai, int _index) : base(ai.aITriggerTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDAT");
                TypeObject = new AITriggerType(ai, tlist.GetNewTag(), AI.NullTeamType, AI.NullTeamType);
            }

            public AddTriggerCommand(AI ai, TeamType teamType1, int _index) : base(ai.aITriggerTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDAT");
                TypeObject = new AITriggerType(ai, tlist.GetNewTag(), teamType1, AI.NullTeamType);
            }
        }

        public class PasteTriggerCommand : AddTypeCommandClass<AITriggerTypes, AITriggerType>
        {
            public PasteTriggerCommand(AITriggerType cp, AITriggerTypes aITriggerTypes, int _index) : base(aITriggerTypes, _index)
            {
                Name = Local.Dictionary("CMD_CPYAT");
                TypeObject = cp.CloneType(tlist.GetNewTag());
                if (MainWindow.configData.Append_WhileCopy)
                    TypeObject.PName += " Copy";
            }
        }

        public class DeleteTriggerCommand : DeleteTypeCommandClass<AITriggerTypes, AITriggerType>
        {
            public DeleteTriggerCommand(AITriggerType del, AITriggerTypes aITriggerTypes) : base(del, aITriggerTypes)
            {
                Name = Local.Dictionary("CMD_DELAT");
            }

            public DeleteTriggerCommand(string deltag, AITriggerTypes aITriggerTypes) : base(deltag, aITriggerTypes)
            {
                Name = Local.Dictionary("CMD_DELAT");
            }
        }
    }
}
