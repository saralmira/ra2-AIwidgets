using AIcore;
using AIcore.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFCustomMessageBox;
using Library;
using System.Collections.ObjectModel;
using RA2AI_Editor.PopupForms;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RA2AI_Editor
{
    public partial class MainWindow : Window
    {
        private void HistoryItem_Click(string path)
        {
            if (path != null)
            {
                if (File.Exists(path))
                {
                    if (FileCloseConfirm() == MessageBoxResult.None)
                        return;
                    current_file = path;
                    OpenFile(current_file);
                }
                else
                {
                    MessageBox.Show(Local.Dictionary("MB_FILENOTEXIST") + path + Local.Dictionary("MB_FILENOTEXIST2"),
                        Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                    listBoxDataInit.Delete(path);
                }
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (FileCloseConfirm() == MessageBoxResult.None)
                return;

            string path = SelectFileToOpen();
            if (path != null)
            {
                if (File.Exists(path))
                {
                    current_file = path;
                    OpenFile(current_file);
                }
                else
                {
                    MessageBox.Show(Local.Dictionary("MB_FILENOTEXIST") + path + Local.Dictionary("MB_FILENOTEXIST2"),
                        Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                    listBoxDataInit.Delete(path);
                }
            }
        }

        private void HistoryItemList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                listBoxDataInit.sizeChangedEventHandler(Convert.ToInt32(HistoryItemList.ActualWidth) - 4);
            }
            catch { }
        }

        private void HistoryItemDelete_Click(object sender, RoutedEventArgs e)
        {
            if (HistoryItemList.SelectedItem != null)
            {
                listBoxDataInit.Delete(HistoryItemList.SelectedItem as Model.HistoryItemInfo);
            }
        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            if (!CloseFile())
                return;
            string filepath = Environment.CurrentDirectory + @"\newfile";
            PathClass.CreateFile(filepath, true);
            OpenFile(filepath);
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            if (current_ai != null)
                SaveToFile(current_ai.FilePath);
        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (current_ai != null)
            {
                string path;
                if ((path = SelectFileToSave()) != null)
                    SaveToFile(path);
            }
        }

        private void FileRelease_Click(object sender, RoutedEventArgs e)
        {
            if (current_ai != null)
            {
                string path;
                if ((path = SelectFileToSave(Local.Dictionary("STR_RELEASETOFILE"))) != null)
                    ReleaseToFile(path);
            }
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            CloseFile();
        }

        private void FileRecover_Click(object sender, RoutedEventArgs e)
        {
            TmpFileForm tf = new TmpFileForm(tmpFileDataInit.FilePaths);
            tf.Owner = this;
            tf.ShowDialog();
        }
        
        private void BottomAdd_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem == tab_taskforce)
            {
                TaskForce tf = taskForceDataInit.Add();
                Jump_To_TaskForce(tf);
            }
            else if (MainTabControl.SelectedItem == tab_scripttype)
            {
                ScriptType st = scriptTypeDataInit.Add();
                Jump_To_ScriptType(st);
            }
            else if (MainTabControl.SelectedItem == tab_teamtype)
            {
                TeamType tt = teamTypeDataInit.Add();
                Jump_To_TeamType(tt);
            }
            else if (MainTabControl.SelectedItem == tab_aitriggers)
            {
                AITriggerType at = aITriggerDataInit.Add();
                Jump_To_AITriggerType(at);
            }
        }

        private void BottomFocus_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabControl.SelectedItem == tab_taskforce)
            {
                if (TaskForceList.SelectedItem != null)
                    Jump_To_TaskForce((TaskForce)TaskForceList.SelectedItem);
            }
            else if (MainTabControl.SelectedItem == tab_scripttype)
            {
                if (ScriptTypeList.SelectedItem != null)
                    Jump_To_ScriptType((ScriptType)ScriptTypeList.SelectedItem);
            }
            else if (MainTabControl.SelectedItem == tab_teamtype)
            {
                if (TeamTypeList.SelectedItem != null)
                    Jump_To_TeamType((TeamType)TeamTypeList.SelectedItem);
            }
            else if (MainTabControl.SelectedItem == tab_aitriggers)
            {
                if (AITriggersList.SelectedItem != null)
                    Jump_To_AITriggerType((AITriggerType)AITriggersList.SelectedItem);
            }
        }

        private void BottomUndo_Click(object sender, RoutedEventArgs e)
        {
            Local.GlobalCommandStack.Undo();
        }

        private void BottomRedo_Click(object sender, RoutedEventArgs e)
        {
            Local.GlobalCommandStack.Redo();
        }

        private void ItemAdd_Click(object sender, RoutedEventArgs e)
        {
            ListView sel = GetListView((MenuItem)sender);
            if (sel != null)
            {
                if (sel == TaskForceList)
                    Jump_To_TaskForce(taskForceDataInit.Add(sel.SelectedIndex));
                else if (sel == ScriptTypeList)
                    Jump_To_ScriptType(scriptTypeDataInit.Add(sel.SelectedIndex));
                else if (sel == TeamTypeList)
                    Jump_To_TeamType(teamTypeDataInit.Add(sel.SelectedIndex));
                else if (sel == AITriggersList)
                    Jump_To_AITriggerType(aITriggerDataInit.Add(sel.SelectedIndex));
            }
        }

        private void ItemDelete_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
            {
                if (sel is TaskForce)
                    Delete((TaskForce)sel);
                else if (sel is ScriptType)
                    Delete((ScriptType)sel);
                else if (sel is TeamType)
                    Delete((TeamType)sel);
                else if (sel is AITriggerType)
                    Delete((AITriggerType)sel);
            }
        }

        private void ItemSafeDelete_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
            {
                string str = "";
                string str2 = "";
                int max = 30;
                if (sel is TaskForce tf)
                {
                    ObservableCollection<TeamType> t1 = teamTypeDataInit.Find(tf);
                    ObservableCollection<AITriggerType> t2 = new ObservableCollection<AITriggerType>();
                    foreach (TeamType t in t1)
                    {
                        if (max-- > 0)
                            str += "\t" + t.PTag + " - " + t.PName + "\r\n";
                        else if (!str.EndsWith("\t...\r\n"))
                            str += "\t...\r\n";
                        foreach (AITriggerType a in aITriggerDataInit.Find(t))
                        {
                            if (max-- > 0)
                                str2 += "\t" + a.PTag + " - " + a.PName + "\r\n";
                            else if (!str2.EndsWith("\t...\r\n"))
                                str2 += "\t...\r\n";
                            t2.Add(a);
                        }
                    }
                    if (str.Length > 0)
                        str = Local.Dictionary("STR_SAFEDEL_2") + "\r\n" + str;
                    if (str2.Length > 0)
                        str2 = Local.Dictionary("STR_SAFEDEL_3") + "\r\n" + str2;
                    if (MessageBoxShow(Local.Dictionary("STR_SAFEDEL_1") + "\r\n\t" + tf.PTag + " - " + tf.PName + "\r\n" + str + str2, System.Windows.MessageBoxButton.OKCancel,
               System.Windows.MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel) == System.Windows.MessageBoxResult.OK)
                    {
                        foreach (TeamType t in t1)
                            Delete(t, true);
                        foreach (AITriggerType t in t2)
                            Delete(t, true);
                        Delete(tf, true);
                    }
                }
                else if (sel is ScriptType st)
                {
                    ObservableCollection<TeamType> t1 = teamTypeDataInit.Find(st);
                    ObservableCollection<AITriggerType> t2 = new ObservableCollection<AITriggerType>();
                    foreach (TeamType t in t1)
                    {
                        if (max-- > 0)
                            str += "\t" + t.PTag + " - " + t.PName + "\r\n";
                        else if (!str.EndsWith("\t...\r\n"))
                            str += "\t...\r\n";
                        foreach (AITriggerType a in aITriggerDataInit.Find(t))
                        {
                            if (max-- > 0)
                                str2 += "\t" + a.PTag + " - " + a.PName + "\r\n";
                            else if (!str2.EndsWith("\t...\r\n"))
                                str2 += "\t...\r\n";
                            t2.Add(a);
                        }
                    }
                    if (str.Length > 0)
                        str = Local.Dictionary("STR_SAFEDEL_2") + "\r\n" + str;
                    if (str2.Length > 0)
                        str2 = Local.Dictionary("STR_SAFEDEL_3") + "\r\n" + str2;
                    if (MessageBoxShow(Local.Dictionary("STR_SAFEDEL_1") + "\r\n\t" + st.PTag + " - " + st.PName + "\r\n" + str + str2, System.Windows.MessageBoxButton.OKCancel,
               MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel) == System.Windows.MessageBoxResult.OK)
                    {
                        foreach (TeamType t in t1)
                            Delete(t, true);
                        foreach (AITriggerType t in t2)
                            Delete(t, true);
                        Delete(st, true);
                    }
                }
                else if (sel is TeamType tt)
                {
                    ObservableCollection<AITriggerType> t2 = aITriggerDataInit.Find(tt);
                    foreach (AITriggerType t in t2)
                    {
                        if (max-- > 0)
                            str += "\t" + t.PTag + " - " + t.PName + "\r\n";
                        else if (!str.EndsWith("\t...\r\n"))
                            str += "\t...\r\n";
                    }
                    if (str.Length > 0)
                        str = Local.Dictionary("STR_SAFEDEL_3") + "\r\n" + str;
                    if (MessageBoxShow(Local.Dictionary("STR_SAFEDEL_1") + "\r\n\t" + tt.PTag + " - " + tt.PName + "\r\n" + str, System.Windows.MessageBoxButton.OKCancel,
                MessageBoxImage.Information, System.Windows.MessageBoxResult.Cancel) == System.Windows.MessageBoxResult.OK)
                    {
                        foreach (AITriggerType t in t2)
                            Delete(t, true);
                        Delete(tt, true);
                    }
                }
            }
        }

        private void Delete(TaskForce tf, bool nohint = false)
        {
            if (TaskForceList.SelectedItem == tf)
                tfgrid.Initialize(null);
            taskForceDataInit.Delete(tf, nohint);
        }

        private void Delete(ScriptType st, bool nohint = false)
        {
            if (ScriptTypeList.SelectedItem == st)
                stgrid2.Initialize(null);
            scriptTypeDataInit.Delete(st, nohint);
        }

        private void Delete(TeamType tt, bool nohint = false)
        {
            if (TeamTypeList.SelectedItem == tt)
                ttgrid.Initialize(null);
            teamTypeDataInit.Delete(tt, nohint);
        }

        private void Delete(AITriggerType at, bool nohint = false)
        {
            if (AITriggersList.SelectedItem == at)
                atgrid.Initialize(null);
            aITriggerDataInit.Delete(at, nohint);
        }

        private void ItemRecover_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
            {
                if (sel is TaskForce)
                    taskForceDataInit.Recover(((TaskForce)sel));
                else if (sel is ScriptType)
                    scriptTypeDataInit.Recover(((ScriptType)sel));
                else if (sel is TeamType)
                    teamTypeDataInit.Recover(((TeamType)sel));
                else if (sel is AITriggerType)
                    aITriggerDataInit.Recover(((AITriggerType)sel));
            }
        }

        private void ItemCopyToList_Click(object sender, RoutedEventArgs e)
        {
            ListView lv = GetLVFromSelectedItem((MenuItem)sender);
            object sel = lv.SelectedItem;
            if (sel != null)
            {
                if (sel is TaskForce)
                    Jump_To_TaskForce(taskForceDataInit.Duplicate(((TaskForce)sel), lv.SelectedIndex));
                else if (sel is ScriptType)
                    Jump_To_ScriptType(scriptTypeDataInit.Duplicate(((ScriptType)sel), lv.SelectedIndex));
                else if (sel is TeamType)
                    Jump_To_TeamType(teamTypeDataInit.Duplicate(((TeamType)sel), lv.SelectedIndex));
                else if (sel is AITriggerType)
                    Jump_To_AITriggerType(aITriggerDataInit.Duplicate(((AITriggerType)sel), lv.SelectedIndex));
            }
        }

        private void ItemCopyCur_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
                SavedObject = sel;
        }

        private void ItemCopyRel_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
            {
                SavedObjectClass so = new SavedObjectClass();
                if (sel is TeamType)
                {
                    so.TeamType1 = sel as TeamType;
                    so.TaskForce1 = so.TeamType1.TaskForce == AI.NullTaskForce ? null : so.TeamType1.TaskForce;
                    so.ScriptType1 = so.TeamType1.Script == AI.NullScriptType ? null : so.TeamType1.Script;
                    SavedObject = so;
                }
                else if (sel is AITriggerType)
                {
                    so.AITriggerType = sel as AITriggerType;
                    so.TeamType1 = so.AITriggerType.TeamType1 == AI.NullTeamType ? null : so.AITriggerType.TeamType1;
                    so.TeamType2 = so.AITriggerType.TeamType2 == AI.NullTeamType ? null : so.AITriggerType.TeamType2;
                    if (so.TeamType1 != null)
                    {
                        so.TaskForce1 = so.TeamType1.TaskForce == AI.NullTaskForce ? null : so.TeamType1.TaskForce;
                        so.ScriptType1 = so.TeamType1.Script == AI.NullScriptType ? null : so.TeamType1.Script;
                    }
                    if (so.TeamType2 != null)
                    {
                        so.TaskForce2 = so.TeamType2.TaskForce == AI.NullTaskForce ? null : so.TeamType2.TaskForce;
                        so.ScriptType2 = so.TeamType2.Script == AI.NullScriptType ? null : so.TeamType2.Script;
                    }
                    SavedObject = so;
                }
            }
        }

        private void ItemPaste_Click(object sender, RoutedEventArgs e)
        {
            if (SavedObject == null || (configData.IniAnalyse != null && configData.IniAnalyse.ShowResults))
                return;
            ListView lv = GetLVFromSelectedItem((MenuItem)sender);
            if (lv == TaskForceList && SavedObject is TaskForce)
                Jump_To_TaskForce(taskForceDataInit.Duplicate((TaskForce)SavedObject, lv.SelectedIndex));
            else if (lv == ScriptTypeList && SavedObject is ScriptType)
                Jump_To_ScriptType(scriptTypeDataInit.Duplicate(((ScriptType)SavedObject), lv.SelectedIndex));
            else if (lv == TeamTypeList)
            {
                if (SavedObject is TeamType)
                    Jump_To_TeamType(teamTypeDataInit.Duplicate(((TeamType)SavedObject), lv.SelectedIndex));
                else if (SavedObject is SavedObjectClass)
                {
                    SavedObjectClass so = SavedObject as SavedObjectClass;
                    if (so.TaskForce1 != null)
                        so.TaskForce1 = taskForceDataInit.Duplicate(so.TaskForce1);
                    if (so.ScriptType1 != null)
                        so.ScriptType1 = scriptTypeDataInit.Duplicate(so.ScriptType1);
                    TeamType teamType = teamTypeDataInit.Duplicate(so.TeamType1, lv.SelectedIndex);
                    teamType.Script = so.ScriptType1;
                    teamType.TaskForce = so.TaskForce1;
                    Jump_To_TeamType(teamType);
                }
            }
            else if (lv == AITriggersList)
            {
                if (SavedObject is AITriggerType)
                    Jump_To_AITriggerType(aITriggerDataInit.Duplicate(((AITriggerType)SavedObject), lv.SelectedIndex));
                else if (SavedObject is SavedObjectClass)
                {
                    SavedObjectClass so = SavedObject as SavedObjectClass;
                    if (so.TaskForce1 != null)
                        so.TaskForce1=taskForceDataInit.Duplicate(so.TaskForce1);
                    if (so.TaskForce2 != null)
                        so.TaskForce2 = taskForceDataInit.Duplicate(so.TaskForce2);
                    if (so.ScriptType1 != null)
                        so.ScriptType1 = scriptTypeDataInit.Duplicate(so.ScriptType1);
                    if(so.ScriptType2 != null)
                        so.ScriptType2 = scriptTypeDataInit.Duplicate(so.ScriptType2);
                    if (so.TeamType1 != null)
                    { 
                        so.TeamType1 = teamTypeDataInit.Duplicate(so.TeamType1);
                        so.TeamType1.Script = so.ScriptType1;
                        so.TeamType1.TaskForce = so.TaskForce1;
                    }
                    if (so.TeamType2 != null)
                    { 
                        so.TeamType2 = teamTypeDataInit.Duplicate(so.TeamType2);
                        so.TeamType2.Script = so.ScriptType2;
                        so.TeamType2.TaskForce = so.TaskForce2;
                    }
                    AITriggerType aITriggerType = aITriggerDataInit.Duplicate(so.AITriggerType, lv.SelectedIndex);
                    aITriggerType.TeamType1 = so.TeamType1;
                    aITriggerType.TeamType2 = so.TeamType2;
                    Jump_To_AITriggerType(aITriggerType);
                }
            }
        }

        private void RulesImport_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectFileToOpen(Local.Dictionary("FILE_OPENRULES"));
            if (path != null)
            {
                if (File.Exists(path))
                {
                    IniClass rules = new IniClass(path);
                    UnitChooseForm ucf = new UnitChooseForm(rules, searchinterval);
                    ucf.Owner = this;
                    ucf.ShowDialog();

                    switch (ucf.MBResult)
                    {
                        case MessageBoxResult.Yes:
                            if (!ucf.DontImportUnits)
                            {
                                units.Update(ucf.AllList, ucf.BuildingList, ucf.Units, UnitChooseForm.KeepExistedUnits);
                                units.Save(false);
                            }
                            if (!ucf.KeepExistedSides)
                                Sides.Update(ucf.SideList);
                            if (!ucf.KeepExistedHouses)
                                Countries.Update(ucf.HouseList);
                            break;
                        //case MessageBoxResult.No:
                        //    units = new Units(alllist, buildingslist, unitslist, UnitChooseForm.KeepExistedUnits);
                        //    units.Save(true);
                        //    break;
                        default:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(Local.Dictionary("MB_FILENOTEXIST") + path + Local.Dictionary("MB_FILENOTEXIST2"),
                        Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void CsfImport_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectCSFFileToOpen();
            if (path != null)
            {
                if (File.Exists(path))
                {
                    CsfClass csf = new CsfClass(path);
                    if (csf.IsValidCSF && csf.ElementCount > 0)
                    {
                        foreach (Unit u in Units.AllList)
                        {
                            string tmp = csf.GetString(u.UIName);
                            if (tmp != null)
                                u.Description = tmp;
                        }
                        Units.StaticSave();
                        Countries.LoadCsfName(csf);

                        MessageBox.Show(Local.Dictionary("MB_CSFIMPORTCOMPLETE"), Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show(Local.Dictionary("MB_INVALIDCSFFILE"), Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(Local.Dictionary("MB_FILENOTEXIST") + path + Local.Dictionary("MB_FILENOTEXIST2"),
                        Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Jump_To_TaskForce(TaskForce tf)
        {
            if (tf == null || tf == AI.NullTaskForce)
                return;
            tf = taskForceDataInit.GetTypeOfCurrent(tf);
            if (tf != null)
            {
                MainTabControl.SelectedItem = tab_taskforce;
                TaskForceList.SelectedItem = tf;
                TaskForceList.ScrollIntoView(tf);
            }
        }

        private void Jump_To_ScriptType(ScriptType st)
        {
            if (st == null || st == AI.NullScriptType)
                return;
            st = scriptTypeDataInit.GetTypeOfCurrent(st);
            if (st != null)
            {
                MainTabControl.SelectedItem = tab_scripttype;
                ScriptTypeList.SelectedItem = st;
                ScriptTypeList.ScrollIntoView(st);
            }
        }

        private void Jump_To_TeamType(TeamType tt)
        {
            if (tt == null || tt == AI.NullTeamType)
                return;
            tt = teamTypeDataInit.GetTypeOfCurrent(tt);
            if (tt != null && tt != AI.NullTeamType)
            {
                MainTabControl.SelectedItem = tab_teamtype;
                TeamTypeList.SelectedItem = tt;
                TeamTypeList.ScrollIntoView(tt);
            }
        }

        private void Jump_To_AITriggerType(AITriggerType at)
        {
            if (at == null)
                return;
            at = aITriggerDataInit.GetTypeOfCurrent(at);
            if (at != null)
            {
                MainTabControl.SelectedItem = tab_aitriggers;
                AITriggersList.SelectedItem = at;
                AITriggersList.ScrollIntoView(at);
            }
        }

        private void Jump_To(OType type)
        {
            if (type != null)
            {
                if (type is TaskForce)
                    Jump_To_TaskForce(type as TaskForce);
                else if (type is ScriptType)
                    Jump_To_ScriptType(type as ScriptType);
                else if (type is TeamType)
                    Jump_To_TeamType(type as TeamType);
                else if (type is AITriggerType)
                    Jump_To_AITriggerType(type as AITriggerType);
            }
        }

        private void TaskForceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem != null && lb.SelectedItem is TaskForce)
            {
                TaskForce tf = (TaskForce)lb.SelectedItem;
                if (tf.ShowCompareResult && tf.SwitchType && tf.CompareType != null)
                    tfgrid.Initialize(tf.CompareType as TaskForce);
                else
                    tfgrid.Initialize(tf);
            }
        }

        private void ScriptTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem != null && lb.SelectedItem is ScriptType)
            {
                ScriptType st = (ScriptType)lb.SelectedItem;
                if (st.ShowCompareResult && st.SwitchType && st.CompareType != null)
                    stgrid2.Initialize(st.CompareType as ScriptType);
                else
                    stgrid2.Initialize(st);
            }
        }

        private void TeamTypeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            if (lv.SelectedItem != null && lv.SelectedItem is TeamType)
            {
                TeamType t = (TeamType)lv.SelectedItem;
                if (t.ShowCompareResult && t.SwitchType && t.CompareType != null)
                    ttgrid.Initialize(t.CompareType as TeamType);
                else
                    ttgrid.Initialize(t);
            }
        }

        private void AITriggersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = (ListView)sender;
            if (lv.SelectedItem != null && lv.SelectedItem is AITriggerType)
            {
                AITriggerType t = (AITriggerType)lv.SelectedItem;
                if (t.ShowCompareResult && t.SwitchType && t.CompareType != null)
                    atgrid.Initialize(t.CompareType as AITriggerType);
                else
                    atgrid.Initialize(t);
            }
        }

        private void SwitchTypeView(OType type, bool alter)
        {
            if (type is TaskForce)
                tfgrid.Initialize(alter ? (type.CompareType as TaskForce) : (type as TaskForce));
            else if (type is ScriptType)
                stgrid2.Initialize(alter ? (type.CompareType as ScriptType) : (type as ScriptType));
            else if (type is TeamType)
                ttgrid.Initialize(alter ? (type.CompareType as TeamType) : (type as TeamType));
            else if (type is AITriggerType)
                atgrid.Initialize(alter ? (type.CompareType as AITriggerType) : (type as AITriggerType));
        }

        private void TaskForceList_Search(object sender, RoutedEventArgs e)
        {
            taskForceDataInit.Filter(((SearchTextBox.SearchTextBox)sender).Text.Trim());
        }

        private void ScriptTypeList_Search(object sender, RoutedEventArgs e)
        {
            scriptTypeDataInit.Filter(((SearchTextBox.SearchTextBox)sender).Text.Trim());
        }

        private void TeamTypeList_Search(object sender, RoutedEventArgs e)
        {
            teamTypeDataInit.Filter(((SearchTextBox.SearchTextBox)sender).Text.Trim());
        }

        private void AITriggerList_Search(object sender, RoutedEventArgs e)
        {
            aITriggerDataInit.Filter(((SearchTextBox.SearchTextBox)sender).Text.Trim());
        }

        public static MessageBoxResult MessageBoxShow(string content, MessageBoxButton btns = MessageBoxButton.OK,
            MessageBoxImage icon = MessageBoxImage.Information, MessageBoxResult def = MessageBoxResult.OK)
        {
            return MessageBox.Show(content, Local.Dictionary("MB_HINT"), btns, icon, def);
        }

        public static MessageBoxResult MessageBoxShow(string content,string title, string btn1,string btn2)
        {
            return CustomMessageBox.ShowOKCancel(Application.Current.MainWindow, content, title, btn1, btn2, MessageBoxImage.Information);
        }

        private object GetSelectedItem(MenuItem sender)
        {
            ContextMenu cm = (ContextMenu)sender.Parent;
            if (cm.PlacementTarget is ListView)
            {
                return ((ListView)cm.PlacementTarget).SelectedItem;
            }
            return null;
        }

        private ListView GetLVFromSelectedItem(MenuItem sender)
        {
            ContextMenu cm = (ContextMenu)sender.Parent;
            if (cm.PlacementTarget is ListView)
            {
                return (ListView)cm.PlacementTarget;
            }
            return null;
        }

        private ListView GetListView(MenuItem sender)
        {
            ContextMenu cm = (ContextMenu)sender.Parent;
            if (cm.PlacementTarget is ListView)
            {
                return ((ListView)cm.PlacementTarget);
            }
            return null;
        }

        private ListView GetListView(ContextMenu sender)
        {
            if (sender.PlacementTarget is ListView)
            {
                return ((ListView)sender.PlacementTarget);
            }
            return null;
        }

        private void CreateTeamType_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
            {
                if (sel is TaskForce)
                {
                    TeamType newtt = teamTypeDataInit.AddTeamTypeWith((TaskForce)sel);
                    Jump_To_TeamType(newtt);
                }
                else if (sel is ScriptType)
                {
                    TeamType newtt = teamTypeDataInit.AddTeamTypeWith((ScriptType)sel);
                    Jump_To_TeamType(newtt);
                }
            }
        }

        private void CreateAITrigger_Click(object sender, RoutedEventArgs e)
        {
            object sel = GetSelectedItem((MenuItem)sender);
            if (sel != null)
            {
                if (sel is TaskForce)
                {
                    TeamType newtt = teamTypeDataInit.AddTeamTypeWith((TaskForce)sel);
                    AITriggerType newai = aITriggerDataInit.AddAITriggerWith(newtt);
                    Jump_To_TeamType(newtt);
                    Jump_To_AITriggerType(newai);
                }
                else if (sel is ScriptType)
                {
                    TeamType newtt = teamTypeDataInit.AddTeamTypeWith((ScriptType)sel);
                    AITriggerType newai = aITriggerDataInit.AddAITriggerWith(newtt);
                    Jump_To_TeamType(newtt);
                    Jump_To_AITriggerType(newai);
                }
                else if (sel is TeamType)
                {
                    AITriggerType newai = aITriggerDataInit.AddAITriggerWith((TeamType)sel);
                    newai.PName = ((TeamType)sel).PName;
                    Jump_To_AITriggerType(newai);
                }
            }
        }

        private void MainContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = (ContextMenu)sender;
            ListView sel = GetListView(cm);
            TagType selected = null;
            if (sel.SelectedItem != null)
                selected = (TagType)sel.SelectedItem;

            if (sel != null)
            {
                bool collapse = configData.IniAnalyse != null && configData.IniAnalyse.ShowResults;
                bool blsel = selected != null;

                SetMenuItem(cm, "mi_add", !collapse, !collapse);
                SetMenuItem(cm, "mi_delete", !collapse && blsel, !collapse);
                SetMenuItem(cm, "mi_recover", !collapse && blsel && !selected.NewType, !collapse);
                SetMenuItem(cm, "mi_copy", !collapse && blsel, !collapse);
                SetMenuItem(cm, "mi_cpyitem", blsel);

                if (sel == TaskForceList)
                {
                    SetMenuItem(cm, "mi_pstitem", !collapse && SavedObject != null && SavedObject is TaskForce, !collapse);
                    SetMenuItem(cm, "mi_safedelete", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_c_tt", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_c_at", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_cpyitemrel", false, false);
                    SetMenuItem(cm, "mi_sep1", !collapse, !collapse);
                    SetMenuItem(cm, "mi_sep2", !collapse, !collapse);
                }
                else if (sel == ScriptTypeList)
                {
                    SetMenuItem(cm, "mi_pstitem", !collapse && SavedObject != null && SavedObject is ScriptType, !collapse);
                    SetMenuItem(cm, "mi_safedelete", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_c_tt", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_c_at", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_cpyitemrel", false, false);
                    SetMenuItem(cm, "mi_sep1", !collapse, !collapse);
                    SetMenuItem(cm, "mi_sep2", !collapse, !collapse);
                }
                else if (sel == TeamTypeList)
                {
                    SetMenuItem(cm, "mi_pstitem", !collapse && SavedObject != null && (SavedObject is TeamType || 
                        (SavedObject is SavedObjectClass && ((SavedObjectClass)SavedObject).AITriggerType == null)), !collapse);
                    SetMenuItem(cm, "mi_safedelete", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_c_tt", false, false);
                    SetMenuItem(cm, "mi_c_at", !collapse && blsel, !collapse);
                    SetMenuItem(cm, "mi_cpyitemrel", blsel);
                    SetMenuItem(cm, "mi_sep1", !collapse, !collapse);
                    SetMenuItem(cm, "mi_sep2", !collapse, !collapse);
                }
                else if (sel == AITriggersList)
                {
                    SetMenuItem(cm, "mi_pstitem", !collapse && SavedObject != null && (SavedObject is AITriggerType) ||
                        (SavedObject is SavedObjectClass && ((SavedObjectClass)SavedObject).AITriggerType != null), !collapse);
                    SetMenuItem(cm, "mi_safedelete", false, false);
                    SetMenuItem(cm, "mi_c_tt", false, false);
                    SetMenuItem(cm, "mi_c_at", false, false);
                    SetMenuItem(cm, "mi_sep1", !collapse, !collapse);
                    SetMenuItem(cm, "mi_sep2", false, false);
                    SetMenuItem(cm, "mi_cpyitemrel", blsel);
                }
            }
        }

        private Control GetMenuItem(ContextMenu cm, string name)
        {
            foreach (object o in cm.Items)
                if (o is MenuItem && ((MenuItem)o).Name == name)
                    return (MenuItem)o;
                else if (o is Separator && ((Separator)o).Name == name)
                    return (Separator)o;
            return null;
        }

        private void SetMenuItem(ContextMenu cm, string name, bool isenabled, bool isvisible = true)
        {
            Control mi = GetMenuItem(cm, name);
            if (mi != null)
            { 
                mi.IsEnabled = isenabled;
                mi.Visibility = isvisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void Gen_CompareReport_Click(object sender, RoutedEventArgs e)
        {
            if (current_ai != null)
            {
                string fpath = SelectFileToOpen(Local.Dictionary("STR_COMPAREFILECHOOSE"));
                if (fpath != null && File.Exists(fpath))
                {
                    IniAnalyse ia = new IniAnalyse(current_ai, fpath);
                    ia.ShowResults = configData.IniAnalyseShowRes;
                    ia.Generate();
                    configData.IniAnalyse = ia;
                    taskForceDataInit.SetAnalysis(configData.IniAnalyse, configData.IniAnalyseShowRes);
                    scriptTypeDataInit.SetAnalysis(configData.IniAnalyse, configData.IniAnalyseShowRes);
                    teamTypeDataInit.SetAnalysis(configData.IniAnalyse, configData.IniAnalyseShowRes);
                    aITriggerDataInit.SetAnalysis(configData.IniAnalyse, configData.IniAnalyseShowRes);
                    ati_diff_tf.ItemsSource = ia.TaskForces1_Diff;
                    ati_diff_st.ItemsSource = ia.ScriptTypes1_Diff;
                    ati_diff_tt.ItemsSource = ia.TeamTypes1_Diff;
                    ati_diff_at.ItemsSource = ia.AITriggerTypes1_Diff;
                    ati_left_tf.ItemsSource = ia.TaskForces1_Left;
                    ati_left_st.ItemsSource = ia.ScriptTypes1_Left;
                    ati_left_tt.ItemsSource = ia.TeamTypes1_Left;
                    ati_left_at.ItemsSource = ia.AITriggerTypes1_Left;
                    ati_same_tf.ItemsSource = ia.TaskForces_Same;
                    ati_same_st.ItemsSource = ia.ScriptTypes_Same;
                    ati_same_tt.ItemsSource = ia.TeamTypes_Same;
                    ati_same_at.ItemsSource = ia.AITriggerTypes_Same;

                    ati_diff_tf2.ItemsSource = ia.TaskForces2_Diff;
                    ati_diff_st2.ItemsSource = ia.ScriptTypes2_Diff;
                    ati_diff_tt2.ItemsSource = ia.TeamTypes2_Diff;
                    ati_diff_at2.ItemsSource = ia.AITriggerTypes2_Diff;
                    ati_left_tf2.ItemsSource = ia.TaskForces2_Left;
                    ati_left_st2.ItemsSource = ia.ScriptTypes2_Left;
                    ati_left_tt2.ItemsSource = ia.TeamTypes2_Left;
                    ati_left_at2.ItemsSource = ia.AITriggerTypes2_Left;
                    ati_same_tf2.ItemsSource = ia.TaskForces_Same;
                    ati_same_st2.ItemsSource = ia.ScriptTypes_Same;
                    ati_same_tt2.ItemsSource = ia.TeamTypes_Same;
                    ati_same_at2.ItemsSource = ia.AITriggerTypes_Same;
                }
            }
        }

        private void Clear_CompareReport_Click(object sender, RoutedEventArgs e)
        {
            Clear_CompareReport();
        }

        private void AddCustomGame_Click(object sender, RoutedEventArgs e)
        {
            string name = tb_customgame.Text;
            if (name == null || name.Length == 0)
            {
                MessageBoxShow(Local.Dictionary("MB_BLANKCUSTOMGAME"));
                return;
            }
            string newdir = Environment.CurrentDirectory + @"\Custom\" + name;
            if (Directory.Exists(newdir))
            {
                MessageBoxShow(Local.Dictionary("MB_INVALIDCUSTOMGAME"));
                return;
            }
            
            PathClass.CreateDir(newdir);
            foreach(string file in Directory.GetFiles(Environment.CurrentDirectory + @"\"+GetGameXmlDirRelative()))
                Utils.FileCopy(file, newdir + @"\" + Path.GetFileName(file), true);
            configData.CreateGame(Game.CreateCustomGameType(name));
        }

        private void DeleteCustomGame_Click(object sender, RoutedEventArgs e)
        {
            if (!Game.IsCustomGameType())
                return;
            if (MessageBoxShow(Local.Dictionary("MB_DELETECUSTOMGAME"), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                string path = Environment.CurrentDirectory + @"\" + GetGameXmlDirRelative();
                foreach (string file in Directory.GetFiles(path))
                    Utils.FileDelete(file);
            }
            configData?.DeleteCurrentGame();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.PageDown:
                    Utils.SwitchToNextItem(MainTabControl, true);
                    break;
                case Key.PageUp:
                    Utils.SwitchToNextItem(MainTabControl, false);
                    break;
            }
        }



        private void SearchPopup_Opened(object sender, EventArgs e)
        {
            Popup p = sender as Popup;
            if (p == tf_pop)
                scb_1.Focus();

        }

        private void SearchPopup_Closed(object sender, EventArgs e)
        {
            Popup p = sender as Popup;
            p.PlacementTarget.Focus();
        }

        private object SavedObject = null;
        private class SavedObjectClass
        {
            public TaskForce TaskForce1;
            public TaskForce TaskForce2;
            public ScriptType ScriptType1;
            public ScriptType ScriptType2;
            public TeamType TeamType1;
            public TeamType TeamType2;
            public AITriggerType AITriggerType;
        }
    }
}
