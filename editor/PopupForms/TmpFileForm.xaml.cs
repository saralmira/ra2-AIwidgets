using AIcore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace RA2AI_Editor.PopupForms
{
    /// <summary>
    /// TmpFileForm.xaml 的交互逻辑
    /// </summary>
    public partial class TmpFileForm : Window
    {
        public TmpFileForm(ObservableCollection<FileInfoClass> files)
        {
            InitializeComponent();
            Flag = true;
            ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Multiselect = false;
            ofd.Title = Local.Dictionary("FILE_SAVE");
            ofd.CheckFileExists = false;
            list.ItemsSource = files;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            while (ofd.ShowDialog() == true)
            {
                if (File.Exists(ofd.FileName) && MainWindow.MessageBoxShow(Local.Dictionary("MB_REPLACE1")+ ofd.FileName + Local.Dictionary("MB_REPLACE2"),MessageBoxButton.YesNo,MessageBoxImage.Information,MessageBoxResult.No) != MessageBoxResult.Yes)
                    continue;

                Button btn = (Button)sender;
                MainWindow.TmpFileEvent((string)btn.Tag, ofd.FileName);
                break;
            }
        }

        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        private void List_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = e.OriginalSource as GridViewColumnHeader;
            if (column == null || column.Column == null)
                return;

            if (_sortColumn == column)
            {
                // Toggle sorting direction 
                _sortDirection = _sortDirection == ListSortDirection.Ascending ?
                                                   ListSortDirection.Descending :
                                                   ListSortDirection.Ascending;
            }
            else
            {
                // Remove arrow from previously sorted header 
                if (_sortColumn != null && _sortColumn.Column != null)
                {
                    _sortColumn.Column.HeaderTemplate = null;
                }

                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
            }

            if (_sortDirection == ListSortDirection.Ascending)
            {
                column.Column.HeaderTemplate = Resources["ArrowUp"] as DataTemplate;
            }
            else
            {
                column.Column.HeaderTemplate = Resources["ArrowDown"] as DataTemplate;
            }

            string header = string.Empty;

            // if binding is used and property name doesn't match header content 
            Binding b = _sortColumn.Column.DisplayMemberBinding as Binding;
            if (b != null)
            {
                header = b.Path.Path;
                if (header == null || header.Length == 0)
                    header = "FileLastWriteTime";
            }
            else
                header = "FileLastWriteTime";

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView(
                                                       (sender as ListView).ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(
                                        new SortDescription(header, _sortDirection));
        }

        private OpenFileDialog ofd;
        private bool Flag;

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (Flag && MainWindow.MessageBoxShow(Local.Dictionary("MB_DELETECONFIRM"), MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes)
                return;
            Flag = false;
            Button btn = (Button)sender;
            MainWindow.TmpFileDeleteEvent((string)btn.Tag, null);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }
    }
}
