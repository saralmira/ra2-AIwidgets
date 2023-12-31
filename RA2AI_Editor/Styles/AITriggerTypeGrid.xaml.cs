using System;
using System.Collections.Generic;
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
using AIcore.Types;
using RA2AI_Editor.UserControls;
using static RA2AI_Editor.MainWindow;

namespace RA2AI_Editor.Styles
{
    /// <summary>
    /// AITriggerTypeGrid.xaml 的交互逻辑
    /// </summary>
    public partial class AITriggerTypeGrid : UserControl
    {
        public AITriggerTypeGrid()
        {
            InitializeComponent();
            TriggerCard.ConditionsAddEventHandler = AddTriggerCondition;
            TriggerCard.ConditionsDeleteEventHandler = DeleteTriggerCondition;
        }

        private AITriggerType at;

        public void Initialize(AITriggerType _at)
        {
            //if (_at == null)
            //    return;

            at = _at;
            //tb_tag.Text = tf.PTag;
            //tb_name.Text = tf.PName;
            //tb_group.Text = tf.Group.ToString();

            grid.DataContext = at;
            grid.IsEnabled = at != null;
            pn_triggers.Children.RemoveRange(1, pn_triggers.Children.Count - 1);
            grid_ext.DataContext = at;
            if (at != null && at.EnableExt)
            {
                for (int i = 0; i < at.Ext_Conditions.Count; ++i)
                {
                    AddTriggerCondition(at.Ext_Conditions[i], i + 1);
                }
            }
            
            SwitchView();
        }

        public void HideGrid(bool hide)
        {
            gd_id.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        public void GetToolTit()
        {
            ControlPlace(gd_basedefence, "IsBaseDefense");
            ControlPlace(gd_comparetype, "Comparator");
            ControlPlace(gd_easymode, "EnabledInEasy");
            ControlPlace(gd_hardmode, "EnabledInHard");
            ControlPlace(gd_mediummode, "EnabledInMedium");
            ControlPlace(gd_owner, "OwnerHouse");
            ControlPlace(gd_side, "Side");
            ControlPlace(gd_skirmish, "IsForSkirmish");
            ControlPlace(gd_team1, "Team1");
            ControlPlace(gd_team2, "Team2");
            ControlPlace(gd_techcount, "ComparisonCount");
            ControlPlace(gd_techlevel, "TechLevel");
            ControlPlace(gd_techtype, "ComparisonObject");
            ControlPlace(gd_triggercondition, "ConditionType");
            ControlPlace(gd_initweight, "InitialWeight");
            ControlPlace(gd_minweight, "MinimumWeight");
            ControlPlace(gd_maxweight, "MaximumWeight");
        }

        public void SwitchView()
        {
            if (at != null)
            {
                viewtab.SelectedIndex = at.EnableExt ? 1 : 0;
            }
        }

        private void ControlPlace<T>(T c, string name) where T : FrameworkElement
        {
            c.ToolTip = TriggerTypes.GetToolTip(name);
        }

        public void AddTriggerCondition(AITriggerTypeBase triggerbase, int index)
        {
            var tc = new TriggerCard
            {
                DataContext = triggerbase
            };
            pn_triggers.Children.Insert(index, tc);
        }

        public void DeleteTriggerCondition(AITriggerTypeBase triggerbase)
        {
            TriggerCard predel = null;
            foreach (var c in pn_triggers.Children)
            {
                if (c is TriggerCard tc && tc.DataContext is AITriggerTypeBase b && b == triggerbase)
                {
                    predel = tc; break;
                }
            }
            pn_triggers.Children.Remove(predel);
        }

        private void AutoCompleteBox_Popup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox box)
                box.IsDropDownOpen = true;
        }

        private void AutoCompleteBox_TeamTypesPopup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox box && at != null)
            {
                AutoCompleteBox acb = box;
                //if (acb.ItemsSource == null)
                //{
                //    Binding b = new Binding();
                //    b.Source = MainWindow.current_ai.teamtypes_info;
                //    b.Mode = BindingMode.OneWay;
                //    acb.SetBinding(AutoCompleteBox.ItemsSourceProperty, b);
                //}
                acb.IsDropDownOpen = true;
            }
        }

        private void MouseWheel_Index(object sender, MouseWheelEventArgs e)
        {
            if (sender is System.Windows.Controls.TextBox tb)
            {
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

        private void TeamType1_JumpClick(object sender, RoutedEventArgs e)
        {
            if (at != null)
                JumpTo(at.TeamType1);
        }

        private void TeamType1_CreateClick(object sender, RoutedEventArgs e)
        {
            if (at != null)
            {
                TeamType tt = teamTypeDataInit.Add();
                tt.PName = at.PName + " 1";
                at.TeamType1 = tt;
                JumpTo(tt);
            }
        }

        private void TeamType2_JumpClick(object sender, RoutedEventArgs e)
        {
            if (at != null)
                JumpTo(at.TeamType2);
        }

        private void TeamType2_CreateClick(object sender, RoutedEventArgs e)
        {
            if (at != null)
            {
                // AITriggerType aITrigger = at;
                TeamType tt = teamTypeDataInit.Add();
                tt.PName = at.PName + " 2";
                at.TeamType2 = tt;
                JumpTo(tt);
            }
        }

        private void MinWeight_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (at != null && sender is System.Windows.Controls.TextBox)
            {
                System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
                if (tb.IsFocused)
                {
                    try
                    {
                        tb.Text = ScrollToNewWeight(Convert.ToInt32(tb.Text), e.Delta, 0, (int)at.BaseWeight).ToString();
                    }
                    catch
                    {
                        tb.Text = "0";
                    }
                    e.Handled = true;
                }
            }
        }

        private void Weight_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (at != null && sender is System.Windows.Controls.TextBox)
            {
                System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
                if (tb.IsFocused)
                {
                    try
                    {
                        tb.Text = ScrollToNewWeight(Convert.ToInt32(tb.Text), e.Delta, (int)at.MinWeight, (int)at.MaxWeight).ToString();
                    }
                    catch
                    {
                        tb.Text = "0";
                    }
                    e.Handled = true;
                }
            }
        }

        private void MaxWeight_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (at != null && sender is System.Windows.Controls.TextBox)
            {
                System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
                if (tb.IsFocused)
                {
                    try
                    {
                        tb.Text = ScrollToNewWeight(Convert.ToInt32(tb.Text), e.Delta, (int)at.BaseWeight, 5000).ToString();
                    }
                    catch
                    {
                        tb.Text = "0";
                    }
                    e.Handled = true;
                }
            }
        }

        private int ScrollToNewWeight(int oldvalue, int delta, int min, int max)
        {
            if (delta > 0)
            {
                if (oldvalue >= 2000)
                    oldvalue += 500;
                else if (oldvalue >= 1000)
                    oldvalue += 200;
                else if (oldvalue >= 500)
                    oldvalue += 100;
                else if (oldvalue >= 150)
                    oldvalue += 50;
                else
                    oldvalue += 10;
            }
            else
            {
                if (oldvalue >= 2000)
                    oldvalue -= 500;
                else if (oldvalue >= 1000)
                    oldvalue -= 200;
                else if (oldvalue >= 500)
                    oldvalue -= 100;
                else if (oldvalue >= 150)
                    oldvalue -= 50;
                else
                    oldvalue -= 10;
            }
            return Math.Max(Math.Min(oldvalue, max), min);
        }

        private void MouseWheelTechLevel_Index(object sender, MouseWheelEventArgs e)
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

        private void AutoCompleteBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (at != null)
            {
                if (at.STeamType1 == null || at.STeamType1.Length == 0)
                    at.STeamType1 = AI.NullTeamType.PTag;
                if (at.STeamType2 == null || at.STeamType2.Length == 0)
                    at.STeamType2 = AI.NullTeamType.PTag;
            }
        }

        private void TeamType_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && acb.IsDropDownOpen)
            {
                TeamType sel = acb.SelectedItem as TeamType;
                if (sel != null)
                    acb.Text = sel.PTag;
                else
                    acb.Text = AI.FindTeamTypeWithFuzzyLogic(acb.SearchText).PTag;
                e.Handled = true;
            }
        }

        private void Unit_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if ((e.Key == Key.Tab || e.Key == Key.Enter) && acb.IsDropDownOpen)
            {
                Unit sel = acb.SelectedItem as Unit;
                if (sel != null)
                    acb.Text = sel.Name;
                else
                    acb.Text = Units.FindUnitFromAllWithFuzzyLogic(acb.SearchText).Name;
                e.Handled = true;
            }
        }

        private void AutoTech_Click(object sender, RoutedEventArgs e)
        {
            if (at != null)
                at.SetMaxTechLevel();
        }
    }
}
