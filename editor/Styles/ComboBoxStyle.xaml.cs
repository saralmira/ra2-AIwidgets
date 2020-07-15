using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using AIcore;

namespace RA2AI_Editor.Styles
{
    public partial class ComboBoxStyle
    {
        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            switch (e.Key)
            {
                case Key.Space:
                    cb.IsDropDownOpen = !cb.IsDropDownOpen;
                    break;
            }
        }

        private void ComboBoxItem_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            switch (e.Key)
            {
                case Key.Space:
                    cbi.IsSelected = true;
                    break;
            }
        }
    }
}
