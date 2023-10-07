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
using AvalonDock;
using AvalonDock.Layout;

namespace RA2AI_Editor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void AITriggerTypeJumpEventDel(AITriggerType at);
        private static AITriggerTypeJumpEventDel AITriggerTypeJumpEvent;
        public delegate void TeamTypeJumpEventDel(TeamType tf);
        private static TeamTypeJumpEventDel TeamTypeJumpEvent;
        public delegate void TaskForceJumpEventDel(TaskForce tf);
        private static TaskForceJumpEventDel TaskForceJumpEvent;
        public delegate void ScriptTypeJumpEventDel(ScriptType st);
        private static ScriptTypeJumpEventDel ScriptTypeJumpEvent;
        public delegate void JumpEventDel(OType type);
        private static JumpEventDel JumpEvent;

        public delegate void SwitchTypeViewDel(OType type, bool alter);
        public static SwitchTypeViewDel SwitchTypeViewEvent;

        public delegate void GeneralEventDel();
        public static GeneralEventDel InitGridEvent;
        public static GeneralEventDel InitPageEvent;
        public delegate void GeneralBoolEventDel(bool value);
        public static GeneralBoolEventDel HideIDGridEvent;

        public delegate void TmpFileEventDel(string md5, string path);
        public static TmpFileEventDel TmpFileEvent;
        public static TmpFileEventDel TmpFileDeleteEvent;

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

        public static Window Window { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Window = this;

            InitData();

            InitHistoryListBox();

            MainWindow_Navigate(true);

            if (Program.FileToOpen != null && File.Exists(Program.FileToOpen))
            {
                current_file = Program.FileToOpen;
                OpenFile(current_file);
            }
        }

        private void InitData()
        {
            Config_Current = new IniClass(AppDomain.CurrentDomain.BaseDirectory + @"\config.ini");
            HideIDGridEvent = HideIDGrid;
            configData = new ConfigClass(Config_Current);
            searchinterval = Config_Current.ReadUIntValue("Config", "SearchInterval", 0);
            current_file = "";

            LoadXmlOfCurrent();
            ttgrid.GetToolTit();
            atgrid.GetToolTit();

            lanc_tf.Hide();
            lanc_st.Hide();
            lanc_tt.Hide();
            lanc_at.Hide();
            lanc_analyse.Hide();

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
            //for (int i = 0; i < tabiteminfo.Count; ++i)
            //    Config_Current.WriteValue("Preference", tabiteminfo[i].Tag, i);

            Config_Current.WriteValue("Config", "SearchInterval", searchinterval.ToString());

            configData.Save();

            tmpFileDataInit.Save();

            Config_Current.Save(); 
            Units.Save();
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

            lanc_tf.Show();
            lanc_st.Show();
            lanc_tt.Show();
            lanc_at.Show();
            lanc_analyse.Show();
            taskForceDataInit = new TaskForceDataInit(current_ai);
            TaskForceList.ItemsSource = taskForceDataInit.ItemList;

            scriptTypeDataInit = new ScriptTypeDataInit(current_ai);
            ScriptTypeList.ItemsSource = scriptTypeDataInit.ItemList;

            teamTypeDataInit = new TeamTypeDataInit(current_ai);
            ttgrid.Init();
            TeamTypeList.ItemsSource = teamTypeDataInit.ItemList;

            aITriggerDataInit = new AITriggerDataInit(current_ai);
            AITriggersList.ItemsSource = aITriggerDataInit.ItemList;

            InitGrid();
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

            lanc_tf.Hide();
            lanc_st.Hide();
            lanc_tt.Hide();
            lanc_at.Hide();
            lanc_analyse.Hide();
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
            InitPage();
        }

        private void InitPage()
        {
            tfgrid.Initialize(null);
            stgrid2.Initialize(null);
            ttgrid.Initialize(null);
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

        public static string GetGameXmlDirRelative()
        {
            return Game.GetGameDirRelative();
        }

        private static void LoadXmlOfCurrent()
        {
            string infodir = GetGameXmlDirRelative();
            new Units(infodir);
            MindControlDecision.Load(infodir);
            GroupInfo.Load(infodir);
            VeteranLevelInfo.Load(infodir);
            Sides.Load(infodir);
            Countries.Load(infodir);
            TriggerTypes.Load(infodir);
            TeamTypeInfo.Load(infodir);
            Scripts.Load(infodir);
            GameInfo.Load(infodir);
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
