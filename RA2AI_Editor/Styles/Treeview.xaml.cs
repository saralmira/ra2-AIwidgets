using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AIcore;
using AIcore.Types;

namespace RA2AI_Editor.Styles
{
    partial class Treeview
    {
        private void JumpButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Button b = sender as Button;
            object data = b.DataContext;
            if (data != null)
            {
                if (data is OType t)
                    MainWindow.JumpTo(t);
            }
        }

        private void TreeViewItem_Selected(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            OType type = item.DataContext as OType;
            if (type != null)
            {
                if (type.AnalysisResult == AnalysisResult.Same)
                    type.HighLight = true;
                else if (type.CompareType != null)
                    (type.CompareType as OType).HighLight = true;
            }
        }

        private void TreeViewItem_Unselected(object sender, System.Windows.RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            OType type = item.DataContext as OType;
            if (type != null)
            {
                if (type.AnalysisResult == AnalysisResult.Same)
                    type.HighLight = false;
                else if (type.CompareType != null)
                    (type.CompareType as OType).HighLight = false;
            }
        }

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            if (item.IsSelected)
                item.IsSelected = false;
        }
    }
}
