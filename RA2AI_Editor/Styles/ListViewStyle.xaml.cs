using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AIcore;
using AIcore.Types;
using RA2AI_Editor.PopupForms;

namespace RA2AI_Editor.Styles
{
    partial class ListViewStyle
    {
        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        private void ListViewSortOut_Click(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader column) || column.Column == null)
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
                _sortColumn = column;
                _sortDirection = ListSortDirection.Ascending;
            }

            // if binding is used and property name doesn't match header content 

            string header;
            if (_sortColumn.Column.DisplayMemberBinding is Binding b)
                header = b.Path.Path;
            else
                header = nameof(TagType.PTag);

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView(
                                                       (sender as ListView).ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(new SortDescription(header, _sortDirection));
        }
    }
}
