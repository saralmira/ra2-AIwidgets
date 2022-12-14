using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using AIcore.Types;
using AIcore;
//using WPF.JoshSmith.ServiceProviders.UI;

namespace RA2AI_Editor.Styles
{
    /// <summary>
    /// TaskForceGrid.xaml 的交互逻辑
    /// </summary>
    public partial class TaskForceGrid : UserControl
    {
        public TaskForceGrid()
        {
            InitializeComponent();
            //lvddm = new ListViewDragDropManager<TaskForce.TaskForceData>(this.list);
            //lvddm.DragAdornerOpacity = 0.7;
            //lvddm.ProcessDrop += Lvddm_ProcessDrop;

            //statebtn.Initialize(Local.Dictionary("BTN_AUTOCREATE"), 3, NameCreate_Click);
        }

        //private void Lvddm_ProcessDrop(object sender, ProcessDropEventArgs<TaskForce.TaskForceData> e)
        //{
        //    if (tf != null)
        //        tf.Move(e.OldIndex, e.NewIndex);
        //}
        //
        //private ListViewDragDropManager<TaskForce.TaskForceData> lvddm;
        private TaskForce tf;

        public void Initialize(TaskForce _tf)
        {
            //if (_tf == null)
            //    return;

            tf = _tf;
            //tb_tag.Text = tf.PTag;
            //tb_name.Text = tf.PName;
            //tb_group.Text = tf.Group.ToString();

            grid.DataContext = tf;
            grid.IsEnabled = tf != null;

            list.ItemsSource = tf?._clist;
            //statebtn.State = statebtn.OState;
            NextNameCreateState = NameCreateState;
        }

        public void HideGrid(bool hide)
        {
            gd_id.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tf != null)
            {
                if (tf._clist.Count >= TaskForce.MaxCount)
                    MainWindow.MessageBoxShow(Local.Dictionary("MB_MAXTASKFORCE") + TaskForce.MaxCount.ToString() + Local.Dictionary("MB_MAXTASKFORCE2"));
                else
                {
                    if (sender is Button)
                        tf.Insert((int)((Button)sender).Tag);
                    else
                        tf.Insert(list.SelectedIndex);
                }
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (tf != null)
            {
                if (sender is Button)
                    tf.Delete((int)((Button)sender).Tag);
                else
                    tf.Delete(list.SelectedIndex);
            }
        }

        ListViewItem GetListViewItem(int index)
        {
            if (list.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return list.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }
        
        private void List_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //调整列宽
            if (list.View is GridView gv)
            {
                double w = list.ActualWidth;
                gv.Columns[1].Width = gv.Columns[0].Width = w * 0.18;
                gv.Columns[2].Width = w * 0.48;
                gv.Columns[3].Width = w * 0.16;
                //gv.Columns[1].Width = gv.Columns[0].Width = w * 0.1875;
                //gv.Columns[2].Width = w * 0.46875;
                //gv.Columns[3].Width = w * 0.15625;
                //foreach (GridViewColumn gvc in gv.Columns)
                //{
                //    gvc.Width = gvc.ActualWidth;
                //    gvc.Width = Double.NaN;
                //}
            }
        }

        //private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    lvddm.ListView = list;
        //}
        //
        //private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    lvddm.ListView = null;
        //}

        private void TextBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            AutoCompleteBox tb = (AutoCompleteBox)sender;
            try
            {
                tb.Text = (e.Delta > 0 ? Convert.ToInt32(tb.Text) + 1 : Math.Max(0, Convert.ToInt32(tb.Text) - 1)).ToString();
            }
            catch
            {
                tb.Text = "0";
            }
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tf != null)
            {
                //Grid g = (Grid)LPopup.Child;
                //System.Windows.Controls.ListBox lb = Local.FindListBoxInElement(g.Children, "PopList");
                PopList.ItemsSource = MainWindow.teamTypeDataInit.Find(tf);
                PopList.SelectedIndex = -1;
                LPopup.IsOpen = true;
            }
        }

        private void AutoCompleteBox_Popup(object sender, MouseButtonEventArgs e)
        {
            if (sender is AutoCompleteBox)
                ((AutoCompleteBox)sender).IsDropDownOpen = true;
        }

        private int NameCreateState = 0;
        private int NextNameCreateState = 0;
        private void NameCreate_Click(object sender, RoutedEventArgs e)
        {
            if (tf != null)
            {
                string name = "";
                if (NextNameCreateState != NameCreateState)
                    NameCreateState = NextNameCreateState;
                switch (NameCreateState)
                {
                    case 0:
                        for (int i = 0; i < tf._clist.Count; ++i)
                        {
                            if (i > 0)
                                name += ", ";
                            name += tf._clist[i].SCount + " " + GetFirstWord(tf._clist[i].Description);
                        }
                        NextNameCreateState = 1;
                        break;
                    case 1:
                        for (int i = 0; i < tf._clist.Count; ++i)
                        {
                            if (i > 0)
                                name += ", ";
                            name += tf._clist[i].SCount + " " + tf._clist[i].Description;
                        }
                        NextNameCreateState = 2;
                        break;
                    case 2:
                        for (int i = 0; i < tf._clist.Count; ++i)
                        {
                            if (i > 0)
                                name += ", ";
                            name += tf._clist[i].SCount + " " + tf._clist[i].Name;
                        }
                        NextNameCreateState = 0;
                        break;
                }
                tf.PName = name;
            }
        }

        private string GetFirstWord(string input)
        {
            string tmp = input.Replace("Allied", "").Replace("Soviet", "").Replace("Yuri", "").Trim();
            if (tmp.Length == 0)
                tmp = input;
            int index = tmp.IndexOf(' ');
            if (index >= 0)
                return tmp.Substring(0, index);
            return tmp;
        }

        private void AutoCompleteBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            switch(e.Key)
            {
                case Key.Up:
                case Key.Down:
                    if (!acb.IsDropDownOpen)
                    { 
                        acb.IsDropDownOpen = true;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (acb.IsDropDownOpen)
                        acb.IsDropDownOpen = false;
                    break;
                case Key.Tab:
                case Key.Enter:
                    if (acb.IsDropDownOpen)
                    {
                        if (acb.SelectedItem is Unit sel)
                            acb.Text = sel.Name;
                        else
                            acb.Text = Units.FindUnitWithFuzzyLogic(acb.SearchText).Name;
                        acb.IsDropDownOpen = false; 
                    }
                    break;
            }
        }

        private void Group_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    if (!acb.IsDropDownOpen)
                    {
                        acb.IsDropDownOpen = true;
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (acb.IsDropDownOpen)
                        acb.IsDropDownOpen = false;
                    break;
                case Key.Tab:
                case Key.Enter:
                    if (acb.IsDropDownOpen)
                    {
                        if (acb.SelectedItem is InfoValueClass sel)
                            acb.Text = sel.Value;
                        else
                            acb.SelectedItem = GroupInfo.FindGroupWithFuzzyLogic(acb.SearchText);
                        acb.IsDropDownOpen = false;
                    }
                    break;
            }
        }

        private void PopList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedItem != null && lb.SelectedItem is TeamType)
            {
                MainWindow.TeamTypeJumpEvent((TeamType)lb.SelectedItem);
            }
        }

        private void LPopup_Opened(object sender, EventArgs e)
        {
            if (PopList.Items.Count > 0)
                PopList.SelectedIndex = 0;
            PopList.MoveFocus(Utils.NextTraversalRequest);
        }

        private void LPopup_Closed(object sender, EventArgs e)
        {
            btn_pop.Focus();
        }
    }
}
