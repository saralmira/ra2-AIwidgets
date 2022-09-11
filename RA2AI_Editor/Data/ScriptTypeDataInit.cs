using AIcore;
using AIcore.TLists;
using AIcore.Types;
using System;

namespace RA2AI_Editor.Data
{
    public class ScriptTypeDataInit : GeneralDataInit<ScriptType>
    {
        public ScriptTypeDataInit(AI _ai) : base(_ai)
        {
            ItemList = ai.scriptTypes.scriptTypes;
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
                CopyList(iniAnalyse.cpScriptTypes, ItemList);
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
            GlobalCommandStack.Push(new DeleteScriptTypeCommand(tag, ai.scriptTypes));
        }

        public override void Delete(ScriptType scripttype, bool nohint = false)
        {
            if (fakemode)
                return;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            if (!nohint)
            {
                if (del_hint && MainWindow.MessageBoxShow(Local.Dictionary("MB_ITEMDELCONFIRM") +
                scripttype.PTag + " - " + scripttype.PName + Local.Dictionary("MB_ITEMDELCONFIRM2"), System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel)
                != System.Windows.MessageBoxResult.Yes)
                    return;
                del_hint = false;
            }
            GlobalCommandStack.Push(new DeleteScriptTypeCommand(scripttype, ai.scriptTypes));
        }

        public void Recover(ScriptType scripttype)
        {
            scripttype.Recover(ai.ini);
        }

        public override ScriptType Add(int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            AddScriptTypeCommand cmd = new AddScriptTypeCommand(ai.scriptTypes, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public override ScriptType Duplicate(ScriptType scripttype, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            PasteScriptTypeCommand cmd = new PasteScriptTypeCommand(scripttype, ai.scriptTypes, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public void Filter(string str)
        {
            if (str == null || str.Length == 0)
            {
                foreach (ScriptType st in ItemList)
                    st.PTVisibility = true;
            }
            else
            {
                StringComparison sc = MainWindow.configData.Search_InCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                foreach (ScriptType st in ItemList)
                {
                    if ((MainWindow.configData.Search_ID && st.PTag.IndexOf(str, sc) >= 0) || 
                        (MainWindow.configData.Search_Name && st.PName.IndexOf(str, sc) >= 0))
                        st.PTVisibility = true;
                    else
                        st.PTVisibility = false;
                }
            }
        }

        public class AddScriptTypeCommand : AddTypeCommandClass<ScriptTypes, ScriptType>
        {
            public AddScriptTypeCommand(ScriptTypes _scriptTypes, int _index) : base(_scriptTypes, _index)
            {
                Name = Local.Dictionary("CMD_ADDST");
                TypeObject = new ScriptType(tlist.GetNewTag());
            }
        }

        public class PasteScriptTypeCommand : AddTypeCommandClass<ScriptTypes, ScriptType>
        {
            public PasteScriptTypeCommand(ScriptType cp, ScriptTypes _scriptTypes, int _index) : base(_scriptTypes, _index)
            {
                Name = Local.Dictionary("CMD_CPYST");
                TypeObject = cp.CloneType(tlist.GetNewTag());
                if (MainWindow.configData.Append_WhileCopy)
                    TypeObject.PName += " Copy";
            }
        }

        public class DeleteScriptTypeCommand : DeleteTypeCommandClass<ScriptTypes, ScriptType>
        {
            public DeleteScriptTypeCommand(ScriptType cp, ScriptTypes _scriptTypes) : base(cp, _scriptTypes)
            {
                Name = Local.Dictionary("CMD_DELST");
            }

            public DeleteScriptTypeCommand(string deltag, ScriptTypes _scriptTypes) : base(deltag, _scriptTypes)
            {
                Name = Local.Dictionary("CMD_DELST");
            }
        }
    }

}
