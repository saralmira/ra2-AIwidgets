using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using AIcore;

namespace RA2AI_Editor.PopupForms
{
    /// <summary>
    /// BuildingIDForm.xaml 的交互逻辑
    /// </summary>
    public partial class BuildingIDForm : Window
    {
        public BuildingIDForm(ObservableCollection<Unit> buildings, string current, UInt32 miliseconds = 100)
        {
            InitializeComponent();
            list.ItemsSource = buildings;
            Result = null;
            searchbox.InstantSearchDelay = new Duration(new TimeSpan(miliseconds * 10000));
            ScrollToSelectedItemEvent = ScrollToItem;
            int icur = Utils.GetBuildingSeqIndexFromID(current);
            TargetChooseMode m = Utils.GetBuildingModeFromID(current);
            modeData = new GridData(m, icur);
            maingd.DataContext = modeData;
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem is Unit)
            {
                Unit u = (Unit)list.SelectedItem;
                modeData.Index = u.SequenceIndex;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = modeData.SResult;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private GridData modeData;

        public delegate void ScrollToSelectedItem(Unit u);
        public static ScrollToSelectedItem ScrollToSelectedItemEvent;
        public string Result;
        private void ScrollToItem(Unit u)
        {
            list.ScrollIntoView(u);
        }

        private class GridData : INotifyPropertyChanged
        {
            public GridData(TargetChooseMode m, int id)
            {
                PropertyChanged += PropertyChangedEvent;

                mode = m;
                STEXT= id.ToString();
            }

            private TargetChooseMode _mode;
            public TargetChooseMode mode { get { return _mode; } set { _mode = value; PropertyChange(this); } }
            public bool bmode1 { get { return mode == TargetChooseMode.MinThreat; } set { if (value) { mode = TargetChooseMode.MinThreat; PropertyChange(this); } } }
            public bool bmode2 { get { return mode == TargetChooseMode.MaxThreat; } set { if (value) { mode = TargetChooseMode.MaxThreat; PropertyChange(this); } } }
            public bool bmode3 { get { return mode == TargetChooseMode.MinDistance; } set { if (value) { mode = TargetChooseMode.MinDistance; PropertyChange(this); } } }
            public bool bmode4 { get { return mode == TargetChooseMode.MaxDistance; } set { if (value) { mode = TargetChooseMode.MaxDistance; PropertyChange(this); } } }

            private int _index;
            public int Index { get { return _index; } set { _index = value; PropertyChange(this); } }
            public string SIndex
            {
                get
                {
                    return Index.ToString();
                }
                set
                {
                    try
                    {
                        _index = Convert.ToInt32(value);
                        SelectedBuilding = Units.FindUnitFromBuildings(_index);
                        if (SelectedBuilding == null || SelectedBuilding == Units.NullUnit)
                            Index = _index;
                    }
                    catch
                    {
                    }
                }
            }

            public string STEXT
            {
                get
                {
                    return Index.ToString();
                }
                set
                {
                    SIndex = value;
                }
            }

            public string SResult
            {
                get
                {
                    return (Index + (int)mode).ToString();
                }
            }

            private Unit _SelectedBuilding;
            public Unit SelectedBuilding
            {
                get
                {
                    return _SelectedBuilding;
                }
                set
                {
                    _SelectedBuilding = value;
                    if (value != null && value != Units.NullUnit)
                    {
                        Index = value.SequenceIndex;
                        PropertyChangeUI(this);
                        ScrollToSelectedItemEvent(value);
                    }
                }
            }

            protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {

            }
            protected void PropertyChange(object sender)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("bmode1"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("bmode2"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("bmode3"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("bmode4"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("Index"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("SResult"));
                }
            }

            protected void PropertyChangeUI(object sender)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("SelectedBuilding"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("STEXT"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            string stxt = ((SearchTextBox.SearchTextBox)e.Source).Text;
            if (stxt.Length == 0)
            {
                foreach (Unit u in list.ItemsSource)
                    u.UIVisibility = Visibility.Visible;
            }
            else
            {
                foreach (Unit u in list.ItemsSource)
                {
                    if (u.FuzzyLogic(stxt))
                        u.UIVisibility = Visibility.Visible;
                    else
                        u.UIVisibility = Visibility.Collapsed;
                }
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
            }

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView(
                                                       (sender as ListView).ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(
                                        new SortDescription(header, _sortDirection));
        }
    }
}
