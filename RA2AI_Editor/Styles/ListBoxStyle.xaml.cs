using AIcore;
using AIcore.Types;
using RA2AI_Editor.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace RA2AI_Editor.Styles
{
    public partial class ListBoxStyle
    {
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.listBoxDataInit.HistoryFileOpen((string)((Button)sender).Tag);
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.taskForceDataInit.Delete((string)((Button)sender).Tag);
        }

        private void HistoryItem_KeyDown(object sender, KeyEventArgs e)
        {
            ListBoxItem item = sender as ListBoxItem;
            switch (e.Key)
            {
                case Key.Enter:
                    if (item.IsFocused && !item.IsSelected)
                        item.IsSelected = true;
                    else if (item.IsSelected)
                        MainWindow.listBoxDataInit.HistoryFileOpen((item.DataContext as HistoryItemInfo).Name);
                    break;
            }
        }

        private void JumpEvent(object o)
        {
            if (o is TeamType t)
                MainWindow.TeamTypeJumpEvent(t);
            else if (o is AITriggerType a)
                MainWindow.AITriggerTypeJumpEvent(a);
        }

        private void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            Popup p = sender as Popup;
            switch (e.Key)
            {
                case Key.Escape:
                    p.IsOpen = false;
                    break;
            }
        }

        private void PopupList_KeyDown(object sender, KeyEventArgs e)
        {
            ListBox l = sender as ListBox;
            switch (e.Key)
            {
                case Key.Up:
                case Key.Left:
                    Utils.SwitchToNextItem(l, false);
                    e.Handled = true;
                    break;
                case Key.Down:
                case Key.Right:
                case Key.Tab:
                    Utils.SwitchToNextItem(l, true);
                    e.Handled = true;
                    break;
                case Key.Space:
                case Key.Enter:
                    if (l.SelectedItem != null)
                        JumpEvent(l.SelectedItem);
                    break;
            }
        }

        private void PopupList_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ListBox l = sender as ListBox;
                if (l.SelectedItem != null)
                {
                    UIElement element = l.InputHitTest(e.GetPosition(l)) as UIElement;
                    while (element != null)
                    {
                        if (element == l)
                            return;
                        object item = l.ItemContainerGenerator.ItemFromContainer(element);
                        if (!item.Equals(DependencyProperty.UnsetValue))
                            JumpEvent(l.SelectedItem);
                        element = (UIElement)VisualTreeHelper.GetParent(element);
                    }
                }
            }
        }
    }
}
