using AIcore.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AIcore;
using RA2AI_Editor.UserControls;
using static RA2AI_Editor.MainWindow;
using System.Windows.Controls.Primitives;

namespace RA2AI_Editor.Styles
{
    /// <summary>
    /// TeamTypeGrid.xaml 的交互逻辑
    /// </summary>
    public partial class TeamTypeGrid : UserControl
    {
        public TeamTypeGrid()
        {
            InitializeComponent();
            runonceflag = true;
            abtn1.expander = exp1;
            abtn2.expander = exp2;
            abtn3.expander = exp3;
            abtn4.expander = exp4;
            abtn5.expander = exp5;
            abtn1.AddClickEvent(ArBtn1_Click);
            abtn2.AddClickEvent(ArBtn1_Click);
            abtn3.AddClickEvent(ArBtn1_Click);
            abtn4.AddClickEvent(ArBtn1_Click);
            abtn5.AddClickEvent(ArBtn1_Click);
        }

        private TeamType tt;

        public void Initialize(TeamType _tt)
        {
            //if (_tt == null)
            //    return;

            tt = _tt;
            //tb_tag.Text = tf.PTag;
            //tb_name.Text = tf.PName;
            //tb_group.Text = tf.Group.ToString();

            grid.DataContext = tt;
            grid.IsEnabled = tt != null;
            SwitchView();
        }

        public void HideGrid(bool hide)
        {
            gd_id.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        public void Init()
        {
            gd_mindcontroldecision.Visibility = Game.IsCurrentGameYR ? Visibility.Visible : Visibility.Collapsed;

            if (!runonceflag)
                return;
            runonceflag = false;

            exp1.SetBinding(Expander.IsExpandedProperty, new Binding() { Source = configData, Path = new PropertyPath("Expander1"), Mode = BindingMode.TwoWay });
            exp2.SetBinding(Expander.IsExpandedProperty, new Binding() { Source = configData, Path = new PropertyPath("Expander2"), Mode = BindingMode.TwoWay });
            exp3.SetBinding(Expander.IsExpandedProperty, new Binding() { Source = configData, Path = new PropertyPath("Expander3"), Mode = BindingMode.TwoWay });
            exp4.SetBinding(Expander.IsExpandedProperty, new Binding() { Source = configData, Path = new PropertyPath("Expander4"), Mode = BindingMode.TwoWay });
            exp5.SetBinding(Expander.IsExpandedProperty, new Binding() { Source = configData, Path = new PropertyPath("Expander5"), Mode = BindingMode.TwoWay });
            abtn1.SetBinding(ArrowButton.ColumnProperty, new Binding() { Source = configData, Path = new PropertyPath("ExpanderColumn1"), Mode = BindingMode.TwoWay });
            abtn2.SetBinding(ArrowButton.ColumnProperty, new Binding() { Source = configData, Path = new PropertyPath("ExpanderColumn2"), Mode = BindingMode.TwoWay });
            abtn3.SetBinding(ArrowButton.ColumnProperty, new Binding() { Source = configData, Path = new PropertyPath("ExpanderColumn3"), Mode = BindingMode.TwoWay });
            abtn4.SetBinding(ArrowButton.ColumnProperty, new Binding() { Source = configData, Path = new PropertyPath("ExpanderColumn4"), Mode = BindingMode.TwoWay });
            abtn5.SetBinding(ArrowButton.ColumnProperty, new Binding() { Source = configData, Path = new PropertyPath("ExpanderColumn5"), Mode = BindingMode.TwoWay });

            if (configData.ExpanderColumn1 == 1)
                ArBtn1_Click(abtn1, null);
            if (configData.ExpanderColumn2 == 1)
                ArBtn1_Click(abtn2, null);
            if (configData.ExpanderColumn3 == 1)
                ArBtn1_Click(abtn3, null);
            if (configData.ExpanderColumn4 == 1)
                ArBtn1_Click(abtn4, null);
            if (configData.ExpanderColumn5 == 1)
                ArBtn1_Click(abtn5, null);
        }

        public void SwitchView()
        {
            if (tt != null)
            {
                viewtab.SelectedIndex = tt.EnableExt ? 1 : 0;
            }
        }

        private bool runonceflag;

        public void GetToolTit()
        {
            ControlPlace(gd_taskforce, TeamTypeInfo.GetTeamTypeInfo("TaskForce"));
            ControlPlace(gd_scripttype, TeamTypeInfo.GetTeamTypeInfo("Script"));

            ControlPlace(gd_veteranlevel, TeamTypeInfo.GetTeamTypeInfo("VeteranLevel"));
            ControlPlace(gd_techlevel, TeamTypeInfo.GetTeamTypeInfo("TechLevel"));
            ControlPlace(gd_house, TeamTypeInfo.GetTeamTypeInfo("House"));
            ControlPlace(gd_max, TeamTypeInfo.GetTeamTypeInfo("Max"));
            ControlPlace(gd_prebuild, TeamTypeInfo.GetTeamTypeInfo("Prebuild"));
            ControlPlace(gd_reinforce, TeamTypeInfo.GetTeamTypeInfo("Reinforce"));
            ControlPlace(gd_annoyance, TeamTypeInfo.GetTeamTypeInfo("Annoyance"));
            ControlPlace(gd_guardslower, TeamTypeInfo.GetTeamTypeInfo("GuardSlower"));
            ControlPlace(gd_isbasedefense, TeamTypeInfo.GetTeamTypeInfo("IsBaseDefense"));
            ControlPlace(gd_whiner, TeamTypeInfo.GetTeamTypeInfo("Whiner"));
            ControlPlace(gd_mindcontroldecision, TeamTypeInfo.GetTeamTypeInfo("MindControlDecision"));
            ControlPlace(gd_ionimmune, TeamTypeInfo.GetTeamTypeInfo("IonImmune"));
            ControlPlace(gd_priority, TeamTypeInfo.GetTeamTypeInfo("Priority"));
            ControlPlace(gd_group, TeamTypeInfo.GetTeamTypeInfo("Group"));
            ControlPlace(gd_areteammembersrecruitble, TeamTypeInfo.GetTeamTypeInfo("AreTeamMembersRecruitable"));
            ControlPlace(gd_recruiter, TeamTypeInfo.GetTeamTypeInfo("Recruiter"));
            ControlPlace(gd_autocreate, TeamTypeInfo.GetTeamTypeInfo("Autocreate"));
            ControlPlace(gd_looserecruit, TeamTypeInfo.GetTeamTypeInfo("LooseRecruits"));
            ControlPlace(gd_avoidthreats, TeamTypeInfo.GetTeamTypeInfo("AvoidThreats"));
            ControlPlace(gd_onlytargethouseenemy, TeamTypeInfo.GetTeamTypeInfo("OnlyTargetHouseEnemy"));
            ControlPlace(gd_aggressive, TeamTypeInfo.GetTeamTypeInfo("Aggressive"));
            ControlPlace(gd_suicide, TeamTypeInfo.GetTeamTypeInfo("Suicide"));

            ControlPlace(gd_loadable, TeamTypeInfo.GetTeamTypeInfo("Loadable"));
            ControlPlace(gd_transportsreturnonunload, TeamTypeInfo.GetTeamTypeInfo("TransportsReturnOnUnload"));
            ControlPlace(gd_ontransonly, TeamTypeInfo.GetTeamTypeInfo("OnTransOnly"));
            ControlPlace(gd_waypoint, TeamTypeInfo.GetTeamTypeInfo("Waypoint"));
            ControlPlace(gd_transwaypoint, TeamTypeInfo.GetTeamTypeInfo("TransportWaypoint"));
            ControlPlace(gd_tag, TeamTypeInfo.GetTeamTypeInfo("Tag"));
            ControlPlace(gd_full, TeamTypeInfo.GetTeamTypeInfo("Full"));
            ControlPlace(gd_dropped, TeamTypeInfo.GetTeamTypeInfo("Droppod"));

        }

        private void ControlPlace<T>(T c, TeamTypeInfo.TeamType t) where T : FrameworkElement
        {
            c.ToolTip = t.Description;
            c.Visibility = t.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AutoCompleteBox_Popup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox)
                ((AutoCompleteBox)sender).IsDropDownOpen = true;
        }

        private void AutoCompleteBox_TaskForcesPopup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox && tt != null)
            {
                AutoCompleteBox acb = (AutoCompleteBox)sender;
                //if (acb.ItemsSource == null)
                //{
                //    Binding b = new Binding();
                //    b.Source = MainWindow.current_ai.taskforces_info;
                //    b.Mode = BindingMode.OneWay;
                //    acb.SetBinding(AutoCompleteBox.ItemsSourceProperty, b);
                //}
                acb.IsDropDownOpen = true;
            }
        }

        private void AutoCompleteBox_ScriptsPopup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox && tt != null)
            {
                AutoCompleteBox acb = (AutoCompleteBox)sender;
                //if (acb.ItemsSource == null)
                //{
                //    Binding b = new Binding();
                //    b.Source = MainWindow.current_ai.scripttypes_info;
                //    b.Mode = BindingMode.OneWay;
                //    acb.SetBinding(AutoCompleteBox.ItemsSourceProperty, b);
                //}
                acb.IsDropDownOpen = true;
            }
        }

        private void MouseWheel_Index(object sender, MouseWheelEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox)
            {
                System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
                if (tb.IsFocused)
                {
                    try
                    {
                        tb.Text = Math.Max(e.Delta > 0 ? Convert.ToInt32(tb.Text) + 1 : Convert.ToInt32(tb.Text) - 1, 0).ToString();
                    }
                    catch
                    {
                        tb.Text = "0";
                    }
                    e.Handled = true;
                }
            }
        }

        private void MouseWheelInt_Index(object sender, MouseWheelEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox)
            {
                System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
                if (tb.IsFocused)
                {
                    try
                    {
                        tb.Text = (e.Delta > 0 ? Convert.ToInt32(tb.Text) + 1 : Convert.ToInt32(tb.Text) - 1).ToString();
                    }
                    catch
                    {
                        tb.Text = "0";
                    }
                    e.Handled = true;
                }
            }
        }

        private void AutoCompleteBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is AutoCompleteBox)
            {
                AutoCompleteBox acb = (AutoCompleteBox)sender;
                acb.FilterMode = AutoCompleteFilterMode.Contains;
                acb.IsDropDownOpen = true;
            }
        }

        private void TaskForce_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.TaskForce);
        }

        private void TaskForce_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                TaskForce tf = taskForceDataInit.Add();
                tf.PName = tt.PName;
                tt.TaskForce = tf;
                acb_tf.Text = tf.PTag;
                JumpTo(tf);
            }
        }

        private void ScriptType_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Script);
        }

        private void ScriptType_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                ScriptType st = scriptTypeDataInit.Add();
                st.PName = tt.PName;
                tt.Script = st;
                acb_s.Text = st.PTag;
                JumpTo(st);
            }
        }

        private void TeamType_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null && tt != AI.NullTeamType)
            {
                Button b = (Button)sender;
                if (b.Content is Popup)
                {
                    Popup p = (Popup)b.Content;
                    Grid g = (Grid)p.Child;
                    System.Windows.Controls.ListBox lb = Local.FindListBoxInElement(g.Children, "PopList");
                    if (lb != null)
                    {
                        lb.ItemsSource = MainWindow.aITriggerDataInit.Find(tt);
                        lb.SelectedIndex = -1;
                        p.IsOpen = true;
                    }
                }
                //Button b = (Button)sender;
                //Point pp = new Point(0, 60);
                //Window window = Window.GetWindow(b);
                //Point point = b.TransformToAncestor(window).Transform(pp);
                //
                //MainWindow.TeamTypeFromTriggersEvent(point, tt);
            }
        }

        private void ArBtn1_Click(object sender, RoutedEventArgs e)
        {
            ArrowButton ab = null;
            if (sender is ArrowButton)
                ab = (ArrowButton)sender;
            else if (sender is Button)
                ab = (ArrowButton)((Button)sender).Tag;

            if (ab == null)
                return;

            if (ab.State)
            {
                stackpanel.Children.Remove(ab.expander);
                stackpanel_formap.Children.Add(ab.expander);
            }
            else
            {
                stackpanel_formap.Children.Remove(ab.expander);
                stackpanel.Children.Add(ab.expander);
            }
            ab.Transform();
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListBox lb = (System.Windows.Controls.ListBox)sender;
            if (lb.SelectedItem != null && lb.SelectedItem is AITriggerType)
            {
                var pele = Utils.GetParent(lb, typeof(Popup));
                if (pele != null && pele is Popup p)
                {
                    p.IsOpen = false;
                }
                MainWindow.JumpTo((AITriggerType)lb.SelectedItem);
            }
        }

        private void Popup_LostFocus(object sender, RoutedEventArgs e)
        {
            Popup p = (Popup)sender;
            p.IsOpen = false;
        }

        private void TaskForceBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && acb.IsDropDownOpen)
            {
                TaskForce sel = acb.SelectedItem as TaskForce;
                if (sel != null)
                    acb.Text = sel.PTag;
                else
                    acb.Text = AI.FindTaskForceWithFuzzyLogic(acb.SearchText).PTag;
                e.Handled = true;
            }
        }

        private void ScriptTypeBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && acb.IsDropDownOpen)
            {
                ScriptType sel = acb.SelectedItem as ScriptType;
                if (sel != null)
                    acb.Text = sel.PTag;
                else
                    acb.Text = AI.FindScriptTypeWithFuzzyLogic(acb.SearchText).PTag;
                e.Handled = true;
            }
        }


        private void Ext_EasyMode_TaskForce_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                TaskForce tf = taskForceDataInit.Add();
                tf.PName = tt.PName + " - E";
                tt.Ext_EasyMode_Type.TaskForce = tf;
                ext_acb_tf.Text = tf.PTag;
                JumpTo(tf);
            }
        }

        private void Ext_EasyMode_TaskForce_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Ext_EasyMode_Type.TaskForce);
        }

        private void Ext_MediumMode_TaskForce_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                TaskForce tf = taskForceDataInit.Add();
                tf.PName = tt.PName + " - M";
                tt.Ext_MediumMode_Type.TaskForce = tf;
                ext_m_acb_tf.Text = tf.PTag;
                JumpTo(tf);
            }
        }

        private void Ext_MediumMode_TaskForce_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Ext_MediumMode_Type.TaskForce);
        }

        private void Ext_HardMode_TaskForce_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                TaskForce tf = taskForceDataInit.Add();
                tf.PName = tt.PName + " - H";
                tt.Ext_HardMode_Type.TaskForce = tf;
                ext_h_acb_tf.Text = tf.PTag;
                JumpTo(tf);
            }
        }

        private void Ext_HardMode_TaskForce_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Ext_HardMode_Type.TaskForce);
        }

        private void Ext_EasyMode_ScriptType_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                ScriptType st = scriptTypeDataInit.Add();
                st.PName = tt.PName + " - E";
                tt.Ext_EasyMode_Type.Script = st;
                ext_acb_s.Text = st.PTag;
                JumpTo(st);
            }
        }

        private void Ext_EasyMode_ScriptType_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Ext_EasyMode_Type.Script);
        }

        private void Ext_MediumMode_ScriptType_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                ScriptType st = scriptTypeDataInit.Add();
                st.PName = tt.PName + " - M";
                tt.Ext_MediumMode_Type.Script = st;
                ext_m_acb_s.Text = st.PTag;
                JumpTo(st);
            }
        }

        private void Ext_MediumMode_ScriptType_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Ext_MediumMode_Type.Script);
        }
        
        private void Ext_HardMode_ScriptType_CreateClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
            {
                ScriptType st = scriptTypeDataInit.Add();
                st.PName = tt.PName + " - H";
                tt.Ext_HardMode_Type.Script = st;
                ext_h_acb_s.Text = st.PTag;
                JumpTo(st);
            }
        }

        private void Ext_HardMode_ScriptType_JumpClick(object sender, RoutedEventArgs e)
        {
            if (tt != null)
                JumpTo(tt.Ext_HardMode_Type.Script);
        }
    }
}
