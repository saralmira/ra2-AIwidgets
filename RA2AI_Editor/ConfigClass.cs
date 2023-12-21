using AIcore;
using AIcore.Types;
using Library;
using RA2AI_Editor.PopupForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace RA2AI_Editor
{
    public partial class MainWindow : Window
    {
        public class ConfigClass : INotifyPropertyChanged
        {
            public ConfigClass(IniClass config)
            {
                cfg = config;
                CurrentFile = null;
                SearchDelay = new Duration(new TimeSpan(searchinterval * 10000));
                
                if (Directory.Exists(Environment.CurrentDirectory + @"\Data\Custom"))
                {
                    foreach (string game in Directory.GetDirectories(Environment.CurrentDirectory + @"\Data\Custom"))
                    {
                        Game.GameTypeClass cgame = Game.GetExistedCustomGameType(Path.GetFileName(game));
                        if (cgame != null)
                            GameTypesInfo.Add(cgame);
                    }
                }

                CurrentGame(true);
                IniAnalyse = null;
                IniAnalyseShowRes = true;
                HideScripts = cfg.ReadBoolValue("Config", "HideScripts", true);
                HideIDGrid = cfg.ReadBoolValue("Config", "HideIDGrid", true);

                Expander1 = cfg.ReadBoolValue("Config", "Expander1", true);
                Expander2 = cfg.ReadBoolValue("Config", "Expander2", true);
                Expander3 = cfg.ReadBoolValue("Config", "Expander3", true);
                Expander4 = cfg.ReadBoolValue("Config", "Expander4", true);
                Expander5 = cfg.ReadBoolValue("Config", "Expander5", true);
                ExpanderColumn1 = cfg.ReadIntValue("Config", "ExpanderColumn1", 0);
                ExpanderColumn2 = cfg.ReadIntValue("Config", "ExpanderColumn2", 0);
                ExpanderColumn3 = cfg.ReadIntValue("Config", "ExpanderColumn3", 0);
                ExpanderColumn4 = cfg.ReadIntValue("Config", "ExpanderColumn4", 0);
                ExpanderColumn5 = cfg.ReadIntValue("Config", "ExpanderColumn5", 1);

                ARColor_None = GetBrushFrom("ARColor_None", 0xfe, 0xf9, 0xeb);
                ARColor_Same = GetBrushFrom("ARColor_Same", 255, 255, 255);
                ARColor_Diff = GetBrushFrom("ARColor_Diff", 255, 160, 112);
                ARColor_Sour = GetBrushFrom("ARColor_Sour", 160, 255, 112);
                ARColor_Targ = GetBrushFrom("ARColor_Targ", 112, 160, 255);
                ARColorOver_None = GetBrushFrom("ARColorOver_None", 0xfc, 0xee, 0xb9);
                ARColorOver_Same = GetBrushFrom("ARColorOver_Same", 245, 245, 245);
                ARColorOver_Diff = GetBrushFrom("ARColorOver_Diff", 255, 148, 100);
                ARColorOver_Sour = GetBrushFrom("ARColorOver_Sour", 148, 255, 100);
                ARColorOver_Targ = GetBrushFrom("ARColorOver_Targ", 100, 148, 255);
                ARColorSelected_None = GetBrushFrom("ARColorSelected_None", 0xfa, 0xe3, 0x88);
                ARColorSelected_Same = GetBrushFrom("ARColorSelected_Same", 225, 225, 225);
                ARColorSelected_Diff = GetBrushFrom("ARColorSelected_Diff", 255, 64, 0);
                ARColorSelected_Sour = GetBrushFrom("ARColorSelected_Sour", 64, 255, 0);
                ARColorSelected_Targ = GetBrushFrom("ARColorSelected_Targ", 0, 64, 255);

                Search_ID = cfg.ReadBoolValue("Config", "Search_ID", true);
                Search_Name = cfg.ReadBoolValue("Config", "Search_Name", true);
                Search_InCase = cfg.ReadBoolValue("Config", "Search_InCase", false);
                Search_Unit = cfg.ReadBoolValue("Config", "Search_Unit", true);
                Append_WhileCopy = cfg.ReadBoolValue("Config", "Append_WhileCopy", false);
                GenerateTriggersForAllSides = cfg.ReadBoolValue("Config", "GenerateTriggersForAllSides", false);

                PropertyChanged += PropertyChangedEvent;
            }

            public void Save()
            {
                CurrentGame(false);
                cfg.WriteValue("Config", "Expander1", Expander1);
                cfg.WriteValue("Config", "Expander2", Expander2);
                cfg.WriteValue("Config", "Expander3", Expander3);
                cfg.WriteValue("Config", "Expander4", Expander4);
                cfg.WriteValue("Config", "Expander5", Expander5);
                cfg.WriteValue("Config", "ExpanderColumn1", ExpanderColumn1);
                cfg.WriteValue("Config", "ExpanderColumn2", ExpanderColumn2);
                cfg.WriteValue("Config", "ExpanderColumn3", ExpanderColumn3);
                cfg.WriteValue("Config", "ExpanderColumn4", ExpanderColumn4);
                cfg.WriteValue("Config", "ExpanderColumn5", ExpanderColumn5);
                cfg.WriteValue("Config", "HideScripts", HideScripts);
                cfg.WriteValue("Config", "HideIDGrid", HideIDGrid);
                SaveBrushTo("ARColor_None", ARColor_None);
                SaveBrushTo("ARColor_Same", ARColor_Same);
                SaveBrushTo("ARColor_Diff", ARColor_Diff);
                SaveBrushTo("ARColor_Sour", ARColor_Sour);
                SaveBrushTo("ARColor_Targ", ARColor_Targ);
                SaveBrushTo("ARColorOver_None", ARColorOver_None);
                SaveBrushTo("ARColorOver_Same", ARColorOver_Same);
                SaveBrushTo("ARColorOver_Diff", ARColorOver_Diff);
                SaveBrushTo("ARColorOver_Sour", ARColorOver_Sour);
                SaveBrushTo("ARColorOver_Targ", ARColorOver_Targ);
                SaveBrushTo("ARColorSelected_None", ARColorSelected_None);
                SaveBrushTo("ARColorSelected_Same", ARColorSelected_Same);
                SaveBrushTo("ARColorSelected_Diff", ARColorSelected_Diff);
                SaveBrushTo("ARColorSelected_Sour", ARColorSelected_Sour);
                SaveBrushTo("ARColorSelected_Targ", ARColorSelected_Targ);
                cfg.WriteValue("Config", "Search_ID", Search_ID);
                cfg.WriteValue("Config", "Search_Name", Search_Name);
                cfg.WriteValue("Config", "Search_InCase", Search_InCase);
                cfg.WriteValue("Config", "Search_Unit", Search_Unit);
                cfg.WriteValue("Config", "Append_WhileCopy", Append_WhileCopy);
                cfg.WriteValue("Config", "GenerateTriggersForAllSides", GenerateTriggersForAllSides);
            }

            private readonly IniClass cfg;

            private string _CurrentFile;
            public string CurrentFile
            {
                get { return _CurrentFile; }
                set
                {
                    _CurrentFile = value;
                    PropertyChange("CurrentFile");
                    PropertyChange("CurrentFileName");
                    PropertyChange("OpenFileBoxText");
                }
            }
            public string CurrentFileName
            {
                get { return listBoxDataInit.GetName(CurrentFile); }
            }
            public string TitleText
            {
                get { if (CurrentFile == null) return "AI Editor"; return "AI Editor - " + CurrentFile + " " + CurrentFileName; }
            }
            public string OpenFileBoxText
            {
                get { if (CurrentFile == null) return Local.Dictionary("FILE_OPEN"); return CurrentFileName + Environment.NewLine + CurrentFile; }
            }

            private Duration _SearchDelay;
            public Duration SearchDelay { get { return _SearchDelay; } set { _SearchDelay = value; try { searchinterval = Convert.ToUInt32(value.TimeSpan.Milliseconds); } catch { searchinterval = 0; } PropertyChange("SearchDelayValue"); PropertyChange("SearchDelayDisplay"); } }
            public UInt32 SearchDelayValue { get { return Convert.ToUInt32(SearchDelay.TimeSpan.Milliseconds); } set { SearchDelay = new Duration(new TimeSpan(value * 10000)); PropertyChange("SearchDelay"); } }
            public string SearchDelayDisplay { get { return SearchDelay.TimeSpan.Milliseconds.ToString() + Local.Dictionary("STR_MS"); } }

            public Game.GameTypeClass SelectedGameType
            {
                get { return Game.CurrentGame; }
                set
                {
                    ChangeGamePrivate(value);
                    PropertyChange(nameof(SelectedGameType));
                    PropertyChange(nameof(IsCustomGame));
                }
            }
            public ObservableCollection<Game.GameTypeClass> GameTypesInfo { get { return Game.GameLists; } }
            public bool IsCustomGame { get { return Game.IsCustomGameType(); } }
            public Game.GameTypeClass GetGameType(string gamename)
            {
                foreach (Game.GameTypeClass g in GameTypesInfo)
                    if (g.Description == gamename)
                        return g;
                return null;
            }
            public void CurrentGame(bool read)
            {
                if (read)
                {
                    Game.GameTypeClass gametype;
                    if (cfg.ReadBoolValue("Config", nameof(IsCustomGame), false) &&
                        (gametype = GetGameType(cfg.ReadValueWithoutNotes("Config", "CustomGameName"))) != null)
                        SelectedGameType = gametype;
                    else
                    {
                        Game.GameType gt;
                        gt = cfg.ReadEnumValue("Config", "GameType", Game.GameType.Unknown);
                        if (gt == Game.GameType.Unknown)
                            gametype = Game.DefaultRAGame;
                        else
                            gametype = gt == Game.GameType.RA ? Game.DefaultRAGame : Game.DefaultYRGame;
                        SelectedGameType = gametype;
                    }
                }
                else
                {
                    if (IsCustomGame)
                    {
                        cfg.WriteValue("Config", nameof(IsCustomGame), true);
                        cfg.WriteValue("Config", "CustomGameName", Game.CurrentGame.Description);
                    }
                    else
                    {
                        cfg.WriteValue("Config", nameof(IsCustomGame), false);
                        cfg.WriteValue("Config", "GameType", (int)Game.CurrentGame.GameType);
                    }
                }
            }
            public void CreateGame(Game.GameTypeClass newgame)
            {
                GameTypesInfo.Add(newgame);
                SelectedGameType = newgame;
            }
            public void DeleteCurrentGame()
            {
                Game.GameTypeClass cgame = Game.CurrentGame;
                SelectedGameType = cgame.GameType == Game.GameType.RA ? Game.DefaultRAGame : Game.DefaultYRGame;
                GameTypesInfo.Remove(cgame);
            }

            public bool Expander1 { get; set; }
            public bool Expander2 { get; set; }
            public bool Expander3 { get; set; }
            public bool Expander4 { get; set; }
            public bool Expander5 { get; set; }
            public int ExpanderColumn1 { get; set; }
            public int ExpanderColumn2 { get; set; }
            public int ExpanderColumn3 { get; set; }
            public int ExpanderColumn4 { get; set; }
            public int ExpanderColumn5 { get; set; }

            public Brush RandomColor
            {
                get
                {
                    Random ran = new Random(DateTime.Now.Millisecond);
                    byte r = (byte)ran.Next(36, 216);
                    byte g = (byte)ran.Next(36, 216);
                    byte b;
                    if (r > 72 && g > 72)
                        b = (byte)ran.Next(36, 64);
                    else if (r < 64 && g < 64)
                        b = (byte)ran.Next(96, 255);
                    else
                        b = (byte)ran.Next(36, 216);
                    return new SolidColorBrush(Color.FromRgb(r, g, b));
                }
            }

            public Brush ARColor_None { get { return Convers.AnalysisResultToBrush_Bg.BrushNone; } set { Convers.AnalysisResultToBrush_Bg.BrushNone = value; } }
            public Brush ARColor_Same { get { return Convers.AnalysisResultToBrush_Bg.BrushSame; } set { Convers.AnalysisResultToBrush_Bg.BrushSame = value; } }
            public Brush ARColor_Diff { get { return Convers.AnalysisResultToBrush_Bg.BrushDifferent; } set { Convers.AnalysisResultToBrush_Bg.BrushDifferent = value; } }
            public Brush ARColor_Sour { get { return Convers.AnalysisResultToBrush_Bg.BrushOnlyInSource; } set { Convers.AnalysisResultToBrush_Bg.BrushOnlyInSource = value; } }
            public Brush ARColor_Targ { get { return Convers.AnalysisResultToBrush_Bg.BrushOnlyInTarget; } set { Convers.AnalysisResultToBrush_Bg.BrushOnlyInTarget = value; } }
            public Brush ARColorOver_None { get { return Convers.AnalysisResultToBrush_MOver.BrushNone; } set { Convers.AnalysisResultToBrush_MOver.BrushNone = value; } }
            public Brush ARColorOver_Same { get { return Convers.AnalysisResultToBrush_MOver.BrushSame; } set { Convers.AnalysisResultToBrush_MOver.BrushSame = value; } }
            public Brush ARColorOver_Diff { get { return Convers.AnalysisResultToBrush_MOver.BrushDifferent; } set { Convers.AnalysisResultToBrush_MOver.BrushDifferent = value; } }
            public Brush ARColorOver_Sour { get { return Convers.AnalysisResultToBrush_MOver.BrushOnlyInSource; } set { Convers.AnalysisResultToBrush_MOver.BrushOnlyInSource = value; } }
            public Brush ARColorOver_Targ { get { return Convers.AnalysisResultToBrush_MOver.BrushOnlyInTarget; } set { Convers.AnalysisResultToBrush_MOver.BrushOnlyInTarget = value; } }
            public Brush ARColorSelected_None { get { return Convers.AnalysisResultToBrush_MSelected.BrushNone; } set { Convers.AnalysisResultToBrush_MSelected.BrushNone = value; } }
            public Brush ARColorSelected_Same { get { return Convers.AnalysisResultToBrush_MSelected.BrushSame; } set { Convers.AnalysisResultToBrush_MSelected.BrushSame = value; } }
            public Brush ARColorSelected_Diff { get { return Convers.AnalysisResultToBrush_MSelected.BrushDifferent; } set { Convers.AnalysisResultToBrush_MSelected.BrushDifferent = value; } }
            public Brush ARColorSelected_Sour { get { return Convers.AnalysisResultToBrush_MSelected.BrushOnlyInSource; } set { Convers.AnalysisResultToBrush_MSelected.BrushOnlyInSource = value; } }
            public Brush ARColorSelected_Targ { get { return Convers.AnalysisResultToBrush_MSelected.BrushOnlyInTarget; } set { Convers.AnalysisResultToBrush_MSelected.BrushOnlyInTarget = value; } }

            private IniAnalyse _IniAnalyse;
            public IniAnalyse IniAnalyse { get { return _IniAnalyse; } set { _IniAnalyse = value; PropertyChange("IniAnalyse"); PropertyChange("AnalyseIniPath"); } }
            private bool _IniAnalyseShowRes;
            public bool IniAnalyseShowRes
            {
                get { return _IniAnalyseShowRes; }
                set
                {
                    _IniAnalyseShowRes = value;
                    if (IniAnalyse != null)
                    {
                        IniAnalyse.ShowResults = value;
                        taskForceDataInit.SwitchMode(value);
                        scriptTypeDataInit.SwitchMode(value);
                        teamTypeDataInit.SwitchMode(value);
                        aITriggerDataInit.SwitchMode(value);
                        PropertyChange("IniAnalyse");
                    }
                    PropertyChange("IniAnalyseShowRes");
                }
            }
            public string AnalyseIniPath { get { if (IniAnalyse == null) return Local.Dictionary("STR_CONTRASTFILE"); return Local.Dictionary("STR_CONTRASTFILE") + ": " + IniAnalyse?.Path; } }

            public bool HideScripts
            {
                get { return Scripts.HideScripts; }
                set { Scripts.HideScripts = value; }
            }

            private bool _HideIDGrid;
            public bool HideIDGrid
            {
                get { return _HideIDGrid; }
                set
                {
                    _HideIDGrid = value;
                    HideIDGridEvent?.Invoke(value);
                    PropertyChange("HideIDGrid");
                }
            }

            private bool _Search_ID;
            private bool _Search_Name;
            private bool _Search_InCase;
            private bool _Search_Unit;
            public bool Search_ID { get { return _Search_ID; } set { _Search_ID = value; PropertyChange("Search_ID"); } }
            public bool Search_Name { get { return _Search_Name; } set { _Search_Name = value; PropertyChange("Search_Name"); } }
            public bool Search_InCase { get { return _Search_InCase; } set { _Search_InCase = value; PropertyChange("Search_InCase"); } }
            public bool Search_Unit { get { return _Search_Unit; } set { _Search_Unit = value; PropertyChange("Search_Unit"); } }

            public bool Append_WhileCopy { get; set; }

            public bool GenerateTriggersForAllSides { get; set; }

            public CommandStack CommandStack { get { return Local.GlobalCommandStack; } }
            public ObservableCollection<CommandClass> ExecutedCommand { get { return Local.GlobalCommandStack.ExecutedCommand; } }
            public ObservableCollection<CommandClass> UndoCommand { get { return Local.GlobalCommandStack.UndoCommand; } }

            protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {

            }
            protected void PropertyChange(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
            public event PropertyChangedEventHandler PropertyChanged;

            private Brush GetBrushFrom(string name, byte defr, byte defg, byte defb)
            {
                string[] v = cfg.ReadValueWithoutNotes("Config", name).Split(',');
                if (v.Length > 2)
                    return new SolidColorBrush(Color.FromRgb(Convert.ToByte(v[0]), Convert.ToByte(v[1]), Convert.ToByte(v[2])));
                else
                    return new SolidColorBrush(Color.FromRgb(defr, defg, defb));
            }

            private void SaveBrushTo(string name, Brush brush)
            {
                if (brush is SolidColorBrush)
                {
                    SolidColorBrush tmp = brush as SolidColorBrush;
                    cfg.WriteValue("Config", name, tmp.Color.R.ToString() + "," + tmp.Color.G.ToString() + "," + tmp.Color.B.ToString());
                }
            }
        }

        private IniClass Config_Current;
    }
}
