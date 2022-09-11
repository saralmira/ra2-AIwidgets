using AIcore;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SearchTextBox;
using System.Collections.Generic;
using Library;

namespace RA2AI_Editor.PopupForms
{
    /// <summary>
    /// UnitChooseForm.xaml 的交互逻辑
    /// </summary>
    public partial class UnitChooseForm : Window
    {
        public UnitChooseForm(IniClass rules, UInt32 miliseconds = 100)
        {
            InitializeComponent();

            BuildingList = new ObservableCollection<Unit>();
            InfantryList = new ObservableCollection<Unit>();
            VehicleList = new ObservableCollection<Unit>();
            AircraftList = new ObservableCollection<Unit>();
            SideList = new ObservableCollection<Side>();
            HouseList = new ObservableCollection<Country>();
            int i_seq = 0;
            string name;
            foreach (string key in rules.GetKeys("BuildingTypes"))
            {
                name = rules.ReadValueWithoutNotes("BuildingTypes", key);
                if (AIcore.Units.Contains(BuildingList, name))
                    continue;
                BuildingList.Add(new Unit(UnitType.BuildingType, i_seq++, name, key, rules));
            }
            i_seq = 0;
            foreach (string key in rules.GetKeys("InfantryTypes"))
            {
                name = rules.ReadValueWithoutNotes("InfantryTypes", key);
                if (AIcore.Units.Contains(InfantryList, name))
                    continue;
                InfantryList.Add(new Unit(UnitType.InfantryType, i_seq++, name, key, rules));
            }
            i_seq = 0;
            foreach (string key in rules.GetKeys("VehicleTypes"))
            {
                name = rules.ReadValueWithoutNotes("VehicleTypes", key);
                if (AIcore.Units.Contains(VehicleList, name))
                    continue;
                VehicleList.Add(new Unit(UnitType.VehicleType, i_seq++, name, key, rules));
            }
            i_seq = 0;
            foreach (string key in rules.GetKeys("AircraftTypes"))
            {
                name = rules.ReadValueWithoutNotes("AircraftTypes", key);
                if (AIcore.Units.Contains(AircraftList, name))
                    continue;
                AircraftList.Add(new Unit(UnitType.AircraftType, i_seq++, name, key, rules));
            }
            Utils.AppendToList(BuildingList, ref AllList);
            Utils.AppendToList(InfantryList, ref AllList);
            Utils.AppendToList(VehicleList, ref AllList);
            Utils.AppendToList(AircraftList, ref AllList);
            Utils.AppendToList(InfantryList, ref Units);
            Utils.AppendToList(VehicleList, ref Units);
            Utils.AppendToList(AircraftList, ref Units);
            Utils.Sort(AllList, u => u.Name);
            Utils.Sort(BuildingList, u => u.Name);
            Utils.Sort(Units, u => u.Name);

            IList<string> slist = rules.GetKeys("Sides");
            for (int i = 0; i < slist.Count; ++i)
            {
                SideList.Add(new Side()
                {
                    Index = i + 1,
                    Name = slist[i],
                    Description = GetDefaultDesc(slist[i]),
                    IsEnabled = (slist[i] == "Civilian" || slist[i] == "Mutant") ? false : true
                });
            }
            IList<string> clist = rules.GetValuesWithoutNotes("Countries");
            for (int i = 0; i < clist.Count; ++i)
            {
                HouseList.Add(new Country()
                {
                    Index = i,
                    Name = clist[i],
                    UIName = rules.ReadValueWithoutNotes(clist[i], "UIName"),
                    Description = rules.ReadValueWithoutNotes(clist[i], "Name"),
                    Side = Sides.GetSide(rules.ReadValueWithoutNotes(clist[i], "Side"), SideList),
                    IsEnabled = rules.ReadBoolValue(clist[i], "Multiplay", false),
                });
            }

            list.ItemsSource = AllList;
            list_sides.ItemsSource = SideList;
            list_houses.ItemsSource = HouseList;
            modeData = new ModeData();
            context.DataContext = modeData;
            searchbox.InstantSearchDelay = new Duration(new TimeSpan(miliseconds * 10000));
            MBResult = MessageBoxResult.Cancel;
            KeepExistedHouses = KeepExistedSides = KeepExistedUnits = false;

        }

        public static MessageBoxResult ShowForm(IniClass rules, UInt32 miliseconds = 100)
        {
            UnitChooseForm ucf = new UnitChooseForm(rules, miliseconds);
            if (Application.Current.MainWindow != null)
                ucf.Owner = Application.Current.MainWindow;
            ucf.ShowDialog();
            return ucf.MBResult;
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            IsSaveAll = true;
            MBResult = MessageBoxResult.Yes;
            Close();
        }

        private void SavePart_Click(object sender, RoutedEventArgs e)
        {
            IsSaveAll = false;
            MBResult = MessageBoxResult.No;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            IsSaveAll = false;
            MBResult = MessageBoxResult.Cancel;
            Close();
        }

        public bool IsSaveAll;
        public MessageBoxResult MBResult;
        public static bool KeepExistedUnits;
        public bool DontImportUnits;
        public bool KeepExistedSides;
        public bool KeepExistedHouses;
        private ListSortDirection _sortDirection;
        private GridViewColumnHeader _sortColumn;
        public ObservableCollection<Unit> Units;
        public ObservableCollection<Unit> AllList;
        public ObservableCollection<Unit> BuildingList;
        public ObservableCollection<Unit> VehicleList;
        public ObservableCollection<Unit> InfantryList;
        public ObservableCollection<Unit> AircraftList;
        public ObservableCollection<Side> SideList;
        public ObservableCollection<Country> HouseList;
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
                    header = "SIsEnabled";
            }
            else
                header = "SIsEnabled";

            ICollectionView resultDataView = CollectionViewSource.GetDefaultView(
                                                       (sender as ListView).ItemsSource);
            resultDataView.SortDescriptions.Clear();
            resultDataView.SortDescriptions.Add(
                                        new SortDescription(header, _sortDirection));
        }

        private ModeData modeData;
        private enum Mode
        {
            None,
            Check,
            UnCheck
        }

        private string GetDefaultDesc(string name)
        {
            switch (name)
            {
                case "GDI": return "Allied";
                case "Nod": return "Soviet";
                case "ThirdSide": return "Yuri";
            }
            return name;
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem != null && modeData.mode != Mode.None)
            {
                Unit u = (Unit)list.SelectedItem;
                u.IsEnabled = modeData.mode == Mode.Check ? true : false;
            }
        }

        private class ModeData : INotifyPropertyChanged
        {
            public ModeData()
            {
                mode = Mode.None;
            }

            private Mode _mode;
            public Mode mode { get { return _mode; } set { _mode = value; PropertyChange(this); } }
            public bool mode1 { get { return mode == Mode.None; } set { if (value) mode = Mode.None; } }
            public bool mode2 { get { return mode == Mode.Check; } set { if (value) mode = Mode.Check; } }
            public bool mode3 { get { return mode == Mode.UnCheck; } set { if (value) mode = Mode.UnCheck; } }

            protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {

            }
            protected void PropertyChange(object sender)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("mode1"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("mode2"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("mode3"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void SearchTextBox_Search(object sender, RoutedEventArgs e)
        {
            string stxt = ((SearchTextBox.SearchTextBox)e.Source).Text;
            ObservableCollection<Unit> l = list.ItemsSource as ObservableCollection<Unit>;
            if (stxt.Length == 0)
            {
                foreach (Unit u in l)
                    if (u.UIVisibility != Visibility.Visible)
                        u.UIVisibility = Visibility.Visible;
            }
            else
            {
                foreach (Unit u in l)
                {
                    if (u.FuzzyLogic(stxt))
                        u.UIVisibility = Visibility.Visible;
                    else
                        u.UIVisibility = Visibility.Collapsed;
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                foreach (Unit u in Units)
                    u.IsEnabled = true;
            }
            else
            {
                foreach (Unit u in Units)
                    u.IsEnabled = false;
            }
        }

        private void KeepExistedUnits_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                KeepExistedUnits = true;
            }
            else
            {
                KeepExistedUnits = false;
            }
        }

        private void DontImportUnits_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                DontImportUnits = true;
                list.IsEnabled = false;
            }
            else
            {
                DontImportUnits = false;
                list.IsEnabled = true;
            }
        }

        private void KeepExistedSides_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                KeepExistedSides = true;
                list_sides.IsEnabled = false;
            }
            else
            {
                KeepExistedSides = false;
                list_sides.IsEnabled = true;
            }
        }

        private void KeepExistedHouses_Checked(object sender, RoutedEventArgs e)
        {
            if (((CheckBox)sender).IsChecked == true)
            {
                KeepExistedHouses = true;
                list_houses.IsEnabled = false;
            }
            else
            {
                KeepExistedHouses = false;
                list_houses.IsEnabled = true;
            }
        }
    }
}
