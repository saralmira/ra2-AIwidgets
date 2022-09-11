using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using RA2AI_Editor.Controls;

namespace RA2AI_Editor.Data
{
  public class SearchModeList
    {
        private static SearchModeList dataInit;

        public static SearchModeList Instance
        {
            get
            {
                if (dataInit == null)
                    dataInit = new SearchModeList();
                return dataInit;
            }
        }

        public static SearchModeList InstanceLimited
        {
            get
            {
                if (dataInit == null)
                    dataInit = new SearchModeList();
                return dataInit;
            }
        }

        public SearchModeList()
        {
            
        }
        public ObservableCollection<MultiComboBox.MultiCbxBaseData> MultiComboBoxListData;
    }
}
