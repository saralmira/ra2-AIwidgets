using AIcore;
using AIcore.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using WPF.JoshSmith.ServiceProviders.UI;

namespace RA2AI_Editor.Styles
{
    /// <summary>
    /// ScriptTypeGrid.xaml 的交互逻辑
    /// </summary>
    public partial class ScriptTypeGrid : UserControl
    {
        public ScriptTypeGrid()
        {
            InitializeComponent();
            //lvddm = new ListViewDragDropManager<ScriptType.ScriptTypeData>(this.list);
            //lvddm.DragAdornerOpacity = 0.7  ;
            //lvddm.ProcessDrop += Lvddm_ProcessDrop;
        }

        //private void Lvddm_ProcessDrop(object sender, ProcessDropEventArgs<ScriptType.ScriptTypeData> e)
        //{
        //    if (st != null)
        //        st.Move(e.OldIndex, e.NewIndex);
        //}
        //
        //private ListViewDragDropManager<ScriptType.ScriptTypeData> lvddm;
        private ScriptType st;

        public void Initialize(ScriptType _st)
        {
            //if (_st == null)
            //    return;

            st = _st;
            //tb_tag.Text = tf.PTag;
            //tb_name.Text = tf.PName;
            //tb_group.Text = tf.Group.ToString();

            grid.DataContext = st;
            grid.IsEnabled = st != null;

            list.ItemsSource = st == null ? null : st._clist;
        }

        public void HideGrid(bool hide)
        {
            gd_id.Visibility = hide ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (st != null)
            {
                if (sender is Button)
                    st.Insert((int)((Button)sender).Tag);
                else
                    st.Insert(list.SelectedIndex);
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (st != null)
            {
                if (sender is Button)
                    st.Delete((int)((Button)sender).Tag);
                else
                    st.Delete(list.SelectedIndex);
                
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
            GridView gv = list.View as GridView;
            if (gv != null)
            {

                double w = list.ActualWidth;
                gv.Columns[0].Width = w * 0.18;
                gv.Columns[2].Width = gv.Columns[1].Width = w * 0.32;
                gv.Columns[3].Width = w * 0.18;
            }
        }

        //private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    lvddm.ListView = list;
        //}
        //
        //private void AutoCompleteBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    lvddm.ListView = null;
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (st != null)
            {
                var ttlist = MainWindow.teamTypeDataInit.Find(st);
                PopList.ItemsSource = ttlist;
                PopList.SelectedIndex = -1;
                PopList2.ItemsSource = Local.FindAITriggers(ttlist);
                PopList2.SelectedIndex = -1;
                LPopup.IsOpen = true;
                //Button b = (Button)sender;
                //if (b.Content is Popup)
                //{
                //    Popup p = (Popup)b.Content;
                //    Grid g = (Grid)p.Child;
                //    System.Windows.Controls.ListBox lb = Local.FindListBoxInElement(g.Children, "PopList");
                //    if (lb != null)
                //    {
                //        lb.ItemsSource = MainWindow.teamTypeDataInit.Find(st);
                //        lb.SelectedIndex = -1;
                //        p.IsOpen = true;
                //    }
                //}
            }
        }

        //private void AutoCompleteBox_PreviewDragEnter(object sender, DragEventArgs e)
        //{
        //    e.Handled = true;
        //}

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            System.Windows.Controls.ListView lb = (System.Windows.Controls.ListView)sender;
            if (lb.SelectedItem != null && lb.SelectedItem is ScriptType.ScriptTypeData)
            {
                tb_info.Text = (lb.SelectedItem as ScriptType.ScriptTypeData).Description;
            }
        }

        private void Popup_LostFocus(object sender, RoutedEventArgs e)
        {
            Popup p = (Popup)sender;
            p.IsOpen = false;
        }

        private void AutoCompleteBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if (acb.Tag != null)
                list.SelectedIndex = (int)acb.Tag;
        }

    }


}
