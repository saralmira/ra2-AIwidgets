using AIcore;
using AIcore.Types;
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
using static AIcore.Types.AITriggerTypeBase;

namespace RA2AI_Editor.UserControls
{
    /// <summary>
    /// TriggerCard.xaml 的交互逻辑
    /// </summary>
    public partial class TriggerCard : UserControl
    {
        public TriggerCard()
        {
            InitializeComponent();
        }

        private void AutoCompleteBox_Popup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox box)
                box.IsDropDownOpen = true;
        }

        private void MouseWheel_Index(object sender, MouseWheelEventArgs e)
        {
            if (sender is TextBox tb)
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

        public void GetToolTit()
        {
            ControlPlace(gd_comparetype, "Comparator");
            ControlPlace(gd_techcount, "ComparisonCount");
            ControlPlace(gd_techtype, "ComparisonObject");
            ControlPlace(gd_triggercondition, "ConditionType");
        }

        private void ControlPlace<T>(T c, string name) where T : FrameworkElement
        {
            c.ToolTip = TriggerTypes.GetToolTip(name);
        }

        public delegate void ConditionsAdd(AITriggerTypeBase tb, int index);
        public delegate void ConditionsDelete(AITriggerTypeBase tb);
        public static ConditionsAdd ConditionsAddEventHandler;
        public static ConditionsDelete ConditionsDeleteEventHandler;

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AITriggerTypeBase tb)
            {
                if (!tb.DerivedBase)
                {
                    var trigger = tb as AITriggerType;
                    var ret = trigger.Ext_ConditionsAdd();
                    ConditionsAddEventHandler?.Invoke(ret.Item1, ret.Item2);
                }
                else
                {
                    var ret = tb.Ext_ConditionsAdd();
                    ConditionsAddEventHandler?.Invoke(ret.Item1, ret.Item2);
                }
            }
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is AITriggerTypeBase tb && tb.DerivedBase)
            {
                tb.Ext_ConditionsDelete(tb);
                ConditionsDeleteEventHandler?.Invoke(tb);
            }
        }
    }
}
