using AIcore;
using AIcore.TLists;
using AIcore.Types;
using RA2AI_Editor.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace RA2AI_Editor.Data
{
    public class TaskForceDataInit : GeneralDataInit<TaskForce>
    {
        public TaskForceDataInit(AI _ai) : base(_ai)
        {
            ItemList = ai.taskForces.taskForces;
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
                CopyList(iniAnalyse.cpTaskForces, ItemList);
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
            GlobalCommandStack.Push(new DeleteTaskForceCommand(tag, ai.taskForces));
        }

        public override void Delete(TaskForce taskforce, bool nohint = false)
        {
            if (fakemode)
                return;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            if (!nohint)
            {
                if (del_hint && MainWindow.MessageBoxShow(Local.Dictionary("MB_ITEMDELCONFIRM") + taskforce.PTag + " - " + taskforce.PName +
                Local.Dictionary("MB_ITEMDELCONFIRM2"), System.Windows.MessageBoxButton.YesNoCancel,
                System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel)
                != System.Windows.MessageBoxResult.Yes)
                    return;
                del_hint = false;
            }
            GlobalCommandStack.Push(new DeleteTaskForceCommand(taskforce, ai.taskForces));
        }

        public void Recover(TaskForce taskForce)
        {
            taskForce.Recover(ai.ini);
        }

        public override TaskForce Duplicate(TaskForce taskForce, int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            PasteTaskForceCommand cmd = new PasteTaskForceCommand(taskForce, ai.taskForces, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        public override TaskForce Add(int index = -1)
        {
            if (fakemode)
                return null;
            //throw new InvalidOperationException(Local.Dictionary("EXC_INVALIDOPERATION"));
            AddTaskForceCommand cmd = new AddTaskForceCommand(ai.taskForces, index);
            GlobalCommandStack.Push(cmd);
            return cmd.TypeObject;
        }

        private bool IsForceDataExist(TaskForce tf, string name, StringComparison sc)
        {
            return !tf.EnableExt ? IsForceDataExist(tf._clist, name, sc) : (
                   IsForceDataExist(tf.Ext_EasyMode_Type._clist, name, sc) ||
                   IsForceDataExist(tf.Ext_MediumMode_Type._clist, name, sc) ||
                   IsForceDataExist(tf.Ext_HardMode_Type._clist, name, sc));
        }

        private bool IsForceDataExist(Collection<TaskForceData> clist, string name, StringComparison sc)
        {
            foreach (TaskForceData tfd in clist)
            {
                if (tfd.Name.IndexOf(name, sc) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void Filter(string str)
        {
            if (str == null || str.Length == 0)
            {
                foreach (TaskForce tf in ItemList)
                    tf.PTVisibility = true;
            }
            else
            {
                StringComparison sc = MainWindow.configData.Search_InCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;
                foreach (TaskForce tf in ItemList)
                {
                    if ((MainWindow.configData.Search_ID && tf.PTag.IndexOf(str, sc) >= 0) ||
                        (MainWindow.configData.Search_Name && tf.PName.IndexOf(str, sc) >= 0) ||
                        (MainWindow.configData.Search_Unit && IsForceDataExist(tf, str, sc)))
                        tf.PTVisibility = true;
                    else
                        tf.PTVisibility = false;
                }
            }
        }

        public class AddTaskForceCommand : AddTypeCommandClass<TaskForces, TaskForce>
        {
            public AddTaskForceCommand(TaskForces _taskForces, int _index) : base(_taskForces, _index)
            {
                Name = Local.Dictionary("CMD_ADDTF");
                TypeObject = new TaskForce(tlist.GetNewTag());
            }
        }

        public class PasteTaskForceCommand : AddTypeCommandClass<TaskForces, TaskForce>
        {
            public PasteTaskForceCommand(TaskForce cp, TaskForces _taskForces, int _index) : base(_taskForces, _index)
            {
                Name = Local.Dictionary("CMD_CPYTF");
                TypeObject = cp.CloneType(tlist.GetNewTag());
                if (MainWindow.configData.Append_WhileCopy)
                    TypeObject.PName += " Copy";
            }
        }

        public class DeleteTaskForceCommand : DeleteTypeCommandClass<TaskForces, TaskForce>
        {
            public DeleteTaskForceCommand(TaskForce del, TaskForces _taskForces) : base(del, _taskForces)
            {
                Name = Local.Dictionary("CMD_DELTF");
            }

            public DeleteTaskForceCommand(string deltag, TaskForces _taskForces) : base(deltag, _taskForces)
            {
                Name = Local.Dictionary("CMD_DELTF");
            }
        }
    }

}
