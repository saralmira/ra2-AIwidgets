using AIcore;
using AIcore.Types;
using Library;
using RA2AI_Editor.Data;
using RA2AI_Editor.PopupForms;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using WPF.JoshSmith.ServiceProviders.UI;

namespace RA2AI_Editor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void AITriggerTypeJumpEventDel(AITriggerType at);
        public static AITriggerTypeJumpEventDel AITriggerTypeJumpEvent;
        public delegate void TeamTypeJumpEventDel(TeamType tf);
        public static TeamTypeJumpEventDel TeamTypeJumpEvent;
        public delegate void TaskForceJumpEventDel(TaskForce tf);
        public static TaskForceJumpEventDel TaskForceJumpEvent;
        public delegate void ScriptTypeJumpEventDel(ScriptType st);
        public static ScriptTypeJumpEventDel ScriptTypeJumpEvent;
        public delegate void JumpEventDel(OType type);
        public static JumpEventDel JumpEvent;

        public delegate void SwitchTypeViewDel(OType type, bool alter);
        public static SwitchTypeViewDel SwitchTypeViewEvent;

        public delegate void GeneralEventDel();
        public static GeneralEventDel InitGridEvent;
        public static GeneralEventDel InitPageEvent;
        public delegate void GeneralBoolEventDel(bool value);
        public static GeneralBoolEventDel HideIDGridEvent;

        private ListViewDragDropManager<TabItemInfo> lvddm_tabitem;
        private ObservableCollection<TabItemInfo> tabiteminfo;
        public delegate void TmpFileEventDel(string md5, string path);
        public static TmpFileEventDel TmpFileEvent;
        public static TmpFileEventDel TmpFileDeleteEvent;

        private static Units units;
        private static string current_lang;
        public static UInt32 searchinterval;

        public static ListBoxDataInit listBoxDataInit { get; private set; }
        private string current_file;
        public static ConfigClass configData;

        public static AI current_ai;
        private TmpFileDataInit tmpFileDataInit; 
        public static TaskForceDataInit taskForceDataInit { get; private set; }
        public static ScriptTypeDataInit scriptTypeDataInit { get; private set; }
        public static TeamTypeDataInit teamTypeDataInit { get; private set; }
        public static AITriggerDataInit aITriggerDataInit { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            InitData();

            InitHistoryListBox();
            
            InitTabItem();
            
            if (Program.FileToOpen != null && File.Exists(Program.FileToOpen))
            {
                current_file = Program.FileToOpen;
                OpenFile(current_file);
            }
        }

        private void InitTabItem()
        {
            MainTabControl.Items.Clear();
            MainTabControl.Items.Add(tab_file);

            tabiteminfo = new ObservableCollection<TabItemInfo>
            {
                new TabItemInfo()
                {
                    Index = Config_Current.ReadIntValue("Preference", "TAB_INDEX1", 0),
                    Name = Local.Dictionary("MENU_TASKFORCE"),
                    TabItem = tab_taskforce,
                    Tag = "TAB_INDEX1"
                },
                new TabItemInfo()
                {
                    Index = Config_Current.ReadIntValue("Preference", "TAB_INDEX2", 1),
                    Name = Local.Dictionary("MENU_SCRIPTTYPE"),
                    TabItem = tab_scripttype,
                    Tag = "TAB_INDEX2"
                },
                new TabItemInfo()
                {
                    Index = Config_Current.ReadIntValue("Preference", "TAB_INDEX3", 2),
                    Name = Local.Dictionary("MENU_TEAMTYPE"),
                    TabItem = tab_teamtype,
                    Tag = "TAB_INDEX3"
                },
                new TabItemInfo()
                {
                    Index = Config_Current.ReadIntValue("Preference", "TAB_INDEX4", 3),
                    Name = Local.Dictionary("MENU_AITRIGGERTYPE"),
                    TabItem = tab_aitriggers,
                    Tag = "TAB_INDEX4"
                }
            };
            Utils.Sort(tabiteminfo, t => t.Index);
            for (int i = 0; i < tabiteminfo.Count; ++i)
                MainTabControl.Items.Add(tabiteminfo[i].TabItem);
            MainTabControl.Items.Add(tab_analyse);
            MainTabControl.Items.Add(tab_settings);

            lv_tabitem.ItemsSource = tabiteminfo;
            lvddm_tabitem = new ListViewDragDropManager<TabItemInfo>(lv_tabitem)
            {
                DragAdornerOpacity = 0.7
            };
            lvddm_tabitem.ProcessDrop += Lvddm_tabitem_ProcessDrop;
        }

        private void InitData()
        {
            current_lang = App.LanguageCurrent;
            Config_Current = new IniClass(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
            HideIDGridEvent = HideIDGrid;
            configData = new ConfigClass(Config_Current);
            searchinterval = Config_Current.ReadUIntValue("Config", "SearchInterval", 0);
            current_file = "";

            LoadXmlOfCurrent();
            ttgrid.GetToolTit();
            atgrid.GetToolTit();
            
            tab_taskforce.Visibility = Visibility.Collapsed;
            tab_scripttype.Visibility = Visibility.Collapsed;
            tab_teamtype.Visibility = Visibility.Collapsed;
            tab_aitriggers.Visibility = Visibility.Collapsed;
            tab_analyse.Visibility = Visibility.Collapsed;

            TaskForceJumpEvent = Jump_To_TaskForce;
            ScriptTypeJumpEvent = Jump_To_ScriptType;
            TeamTypeJumpEvent = Jump_To_TeamType;
            AITriggerTypeJumpEvent = Jump_To_AITriggerType;
            JumpEvent = Jump_To;
            //TaskForceFromTeamTypeEvent = SearchFromTeamTypes;
            //ScriptTypeFromTeamTypeEvent = SearchFromTeamTypes;
            //TeamTypeFromTriggersEvent = SearchFromAITriggers;
            TmpFileEvent = TmpFileEventHandler;
            TmpFileDeleteEvent = TmpFileDeleteEventHandler;
            InitGridEvent = InitGrid;
            InitPageEvent = InitPage;
            SwitchTypeViewEvent = SwitchTypeView;
            //ListBoxForm = new ListBoxForm();
            tmpFileDataInit = new TmpFileDataInit(Config_Current);

            maingrid.DataContext = configData;

        }

        private void InitHistoryListBox()
        {
            listBoxDataInit = new ListBoxDataInit(Config_Current, Convert.ToInt32(HistoryItemList.ActualWidth) - 4);
            listBoxDataInit.HistoryFileOpen += HistoryItem_Click;

            HistoryItemList.SelectionMode = System.Windows.Controls.SelectionMode.Single;
            HistoryItemList.MouseDoubleClick += HistoryItemList_MouseDoubleClick;
            HistoryItemList.ItemsSource = listBoxDataInit.ItemList;

        }

        private void HistoryItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxDataInit.IsEmpty)
                return;
            if (e.ChangedButton == MouseButton.Left)
            {
                if (HistoryItemList.SelectedItem != null)
                    HistoryItem_Click(((Model.HistoryItemInfo)HistoryItemList.SelectedItem).Name);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (FileCloseConfirm() == MessageBoxResult.None)
            {
                e.Cancel = true;
                return;
            }

            //if (ListBoxForm != null)
            //    ListBoxForm.Close();
            if (listBoxDataInit != null)
                listBoxDataInit.Save();

            SaveCurrentConfig();
        }

        private void SaveCurrentConfig()
        {
            for (int i = 0; i < tabiteminfo.Count; ++i)
                Config_Current.WriteValue("Preference", tabiteminfo[i].Tag, i);

            Config_Current.WriteValue("Config", "SearchInterval", searchinterval.ToString());

            configData.Save();

            tmpFileDataInit.Save();

            Config_Current.Save(); 
            units.Save();
            Sides.Save();
            Countries.Save();
        }

        /// <summary>
        /// 打开AI文件
        /// </summary>
        /// <param name="path"></param>
        private void OpenFile(string path)
        {
            Clear_CompareReport();
            Local.GlobalCommandStack.Clear();

            listBoxDataInit.Add(path);
            current_ai = new AI(path);
            configData.CurrentFile = path;
            Title = configData.TitleText;

            tab_taskforce.Visibility = Visibility.Visible;
            tab_scripttype.Visibility = Visibility.Visible;
            tab_teamtype.Visibility = Visibility.Visible;
            tab_aitriggers.Visibility = Visibility.Visible;
            tab_analyse.Visibility = Visibility.Visible;
            taskForceDataInit = new TaskForceDataInit(current_ai);
            TaskForceList.ItemsSource = taskForceDataInit.ItemList;

            scriptTypeDataInit = new ScriptTypeDataInit(current_ai);
            ScriptTypeList.ItemsSource = scriptTypeDataInit.ItemList;

            teamTypeDataInit = new TeamTypeDataInit(current_ai);
            ttgrid.Init();
            TeamTypeList.ItemsSource = teamTypeDataInit.ItemList;

            aITriggerDataInit = new AITriggerDataInit(current_ai);
            //if (AITriggersList.ItemsSource == null && AITriggersList.Items != null)
            //    AITriggersList.Items.Clear();
            AITriggersList.ItemsSource = aITriggerDataInit.ItemList;

            InitGrid();
        }

        private string SelectFileToOpen(string title = null)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = false
            };
            if (title == null)
                title = Local.Dictionary("FILE_OPEN");
            openFileDialog.Title = title;
            openFileDialog.Filter = "RA2 Files(*.ini,*.mpr,*.yrm,*.map)| *.ini;*.mpr;*.yrm;*.map|ALL Files.| *";
            openFileDialog.CheckFileExists = openFileDialog.CheckPathExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private string SelectCSFFileToOpen()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = false
            };
            openFileDialog.Title = Local.Dictionary("FILE_OPENCSF");
            openFileDialog.Filter = "Csf Files(*.csf)| *.csf|ALL Files.| *";
            openFileDialog.CheckFileExists = openFileDialog.CheckPathExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private string SelectFileToSave(string title = null)
        {
            if (title == null)
                title = Local.Dictionary("FILE_SAVE");
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog
            {
                Title = title,
                Filter = "RA2 Files(*.ini,*.mpr,*.yrm,*.map)| *.ini;*.mpr;*.yrm;*.map|ALL Files.| *.*",
                DefaultExt = System.IO.Path.GetExtension(current_ai.FilePath),
                AddExtension = false
            };
            bool? bl = sfd.ShowDialog();
            if (bl == true)
            {
                return sfd.FileName;
            }
            return null;
        }

        private void SaveToFile(string path)
        {
            tmpFileDataInit.Add(current_ai.FilePath);
            current_ai.SaveAI(path);
            configData.CurrentFile = path;
            Title = configData.TitleText;
        }

        private void ReleaseToFile(string path)
        {
            current_ai.ReleaseAI(path);
        }

        private bool CloseFile()
        {
            if (current_ai == null)
                return true;

            if (FileCloseConfirm() == MessageBoxResult.None)
                return false;

            Clear_CompareReport();
            Local.GlobalCommandStack.Clear();
            configData.CurrentFile = null;
            current_ai = null;
            Title = configData.TitleText;

            tab_taskforce.Visibility = Visibility.Collapsed;
            tab_scripttype.Visibility = Visibility.Collapsed;
            tab_teamtype.Visibility = Visibility.Collapsed;
            tab_aitriggers.Visibility = Visibility.Collapsed;
            tab_analyse.Visibility = Visibility.Collapsed;
            TaskForceList.ItemsSource = null;
            ScriptTypeList.ItemsSource = null;
            TeamTypeList.ItemsSource = null;
            AITriggersList.ItemsSource = null;
            taskForceDataInit = null;
            scriptTypeDataInit = null;
            teamTypeDataInit = null;
            aITriggerDataInit = null;
            GC.Collect();

            InitGrid();
            return true;
        }

        private void InitGrid()
        {
            TaskForceList.SelectedIndex = -1;
            ScriptTypeList.SelectedIndex = -1;
            TeamTypeList.SelectedIndex = -1;
            AITriggersList.SelectedIndex = -1;
            tfgrid.Initialize(null);
            stgrid2.Initialize(null);
            ttgrid.Initialize(null);
            atgrid.Initialize(null);
        }

        private void InitPage()
        {
            if (TaskForceList.SelectedItem == null)
                tfgrid.Initialize(null);
            if (ScriptTypeList.SelectedItem == null)
                stgrid2.Initialize(null);
            if (TeamTypeList.SelectedItem == null)
                ttgrid.Initialize(null);
            if (AITriggersList.SelectedItem == null)
                atgrid.Initialize(null);
        }

        /// <summary>
        /// 关闭文件确认（None为不关闭，OK为未保存关闭，Cancel为已保存关闭）
        /// </summary>
        /// <returns>None为不关闭，OK为未保存关闭，Cancel为已保存关闭</returns>
        private MessageBoxResult FileCloseConfirm()
        {
            if (current_ai != null)
            {
                MessageBoxResult r = MessageBoxShow(Local.Dictionary("MB_CONFIRM"), Local.Dictionary("MB_HINT"), Local.Dictionary("MB_IHAVESAVED"),
                    Local.Dictionary("MB_IMNOTSURE"));
                if (r == MessageBoxResult.Cancel)
                {
                    string path;
                    if ((path = SelectFileToSave()) != null)
                    {
                        //if (File.Exists(path))
                        //{
                        //    MessageBoxResult mbr = MessageBoxShow(Local.Dictionary("MB_REPLACE"), Local.Dictionary("MB_HINT"),
                        //        Local.Dictionary("MB_REPLACEYES"), Local.Dictionary("MB_REPLACENO"));
                        //    if (mbr == MessageBoxResult.Cancel)
                        //        continue;
                        //    else if (mbr != MessageBoxResult.OK)
                        //        break;
                        //}
                        SaveToFile(path);
                        return MessageBoxResult.Cancel;
                    }
                }
                else if (r == MessageBoxResult.OK)
                    return MessageBoxResult.OK;

                return MessageBoxResult.None;
            }
            return MessageBoxResult.OK;
        }

        private void TmpFileEventHandler(string md5, string path)
        {
            tmpFileDataInit.Recover(md5, path);
        }

        private void TmpFileDeleteEventHandler(string md5, string path)
        {
            tmpFileDataInit.Delete(md5);
        }

        private void Lvddm_tabitem_ProcessDrop(object sender, ProcessDropEventArgs<TabItemInfo> e)
        {
            if (e.NewIndex != e.OldIndex)
            {
                tabiteminfo.Move(e.OldIndex, e.NewIndex);
            }
        }

        private void ARRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = sender as Rectangle;
            ColorDialog cd = new ColorDialog
            {
                FullOpen = true
            };
            SolidColorBrush tmp = rec.Fill as SolidColorBrush;
            cd.Color = System.Drawing.Color.FromArgb(tmp.Color.A, tmp.Color.R, tmp.Color.G, tmp.Color.B);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                rec.Fill = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                current_ai.RefreshAnalysisResult();
            }
            cd.Dispose();
        }

        private void Clear_CompareReport()
        {
            ati_diff_tf.ItemsSource = null;
            ati_diff_st.ItemsSource = null;
            ati_diff_tt.ItemsSource = null;
            ati_diff_at.ItemsSource = null;
            ati_left_tf.ItemsSource = null;
            ati_left_st.ItemsSource = null;
            ati_left_tt.ItemsSource = null;
            ati_left_at.ItemsSource = null;
            ati_same_tf.ItemsSource = null;
            ati_same_st.ItemsSource = null;
            ati_same_tt.ItemsSource = null;
            ati_same_at.ItemsSource = null;
            ati_diff_tf2.ItemsSource = null;
            ati_diff_st2.ItemsSource = null;
            ati_diff_tt2.ItemsSource = null;
            ati_diff_at2.ItemsSource = null;
            ati_left_tf2.ItemsSource = null;
            ati_left_st2.ItemsSource = null;
            ati_left_tt2.ItemsSource = null;
            ati_left_at2.ItemsSource = null;
            ati_same_tf2.ItemsSource = null;
            ati_same_st2.ItemsSource = null;
            ati_same_tt2.ItemsSource = null;
            ati_same_at2.ItemsSource = null;

            if (configData.IniAnalyse != null)
            {
                configData.IniAnalyse.Clear();
                configData.IniAnalyse = null;
            }
            if (taskForceDataInit != null) taskForceDataInit.ClearAnalysisData();
            if (scriptTypeDataInit != null) scriptTypeDataInit.ClearAnalysisData();
            if (teamTypeDataInit != null) teamTypeDataInit.ClearAnalysisData();
            if (aITriggerDataInit != null) aITriggerDataInit.ClearAnalysisData();
        }

        public static void ChangeGame(Game.GameType newgametype)
        {
            switch (newgametype)
            {
                case Game.GameType.Unknown:
                    ChangeGame(Game.DefaultUnknownGame);
                    break;
                case Game.GameType.RA:
                    if (Game.CurrentGame.GameType != Game.GameType.RA)
                        ChangeGame(Game.DefaultRAGame);
                    break;
                case Game.GameType.YR:
                    if (Game.CurrentGame.GameType != Game.GameType.YR)
                        ChangeGame(Game.DefaultYRGame);
                    break;
            }
        }

        private static void ChangeGamePrivate(Game.GameTypeClass newgametype)
        {
            if (newgametype.GameType == Game.GameType.Unknown)
                newgametype = GameTypeChoose.Choose();
            if (newgametype == Game.CurrentGame)
                return;
            if (newgametype == null || newgametype == Game.DefaultUnknownGame)
                newgametype = Game.DefaultRAGame;

            Game.CurrentGame = newgametype;
            LoadXmlOfCurrent();
            InitGridEvent?.Invoke();
        }

        public static void ChangeGame(Game.GameTypeClass newgametype)
        {
            if (configData != null)
                configData.SelectedGameType = newgametype;
        }

        private static string GetGameXmlDirRelative()
        {
            if (Game.IsCustomGameType())
                return @"Custom\" + Game.CurrentGame.Description;
            else
                return current_lang + Game.CurrentGameDir;
        }

        private static void LoadXmlOfCurrent()
        {
            string infodir = GetGameXmlDirRelative();
            units = new Units(infodir);
            MindControlDecision.Load(infodir);
            GroupInfo.Load(infodir);
            VeteranLevelInfo.Load(infodir);
            Sides.Load(infodir);
            Countries.Load(infodir);
            TriggerTypes.Load(infodir);
            TeamTypeInfo.Load(infodir);
            Scripts.Load(infodir);
        }

        private void HideIDGrid(bool hide)
        {
            tfgrid.HideGrid(hide);
            stgrid2.HideGrid(hide);
            ttgrid.HideGrid(hide);
            atgrid.HideGrid(hide);
        }

        private int ReplaceUnit(string oldunit, string newunit)
        {
            if (!Units.ContainsUnit(oldunit) || !Units.ContainsUnit(newunit))
                return 0;

            int count = 0;
            foreach (var t in current_ai.taskForces.taskForces)
            {
                count += t.ReplaceUnitWith(oldunit, newunit);
            }
            return count;
        }
    }
}
