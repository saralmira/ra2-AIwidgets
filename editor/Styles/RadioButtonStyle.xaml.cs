using AIcore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace RA2AI_Editor.Styles
{
    public partial class RadioButtonStyle
    {
        private void SliderCheckBox_KeyDown(object sender, KeyEventArgs e)
        {
            Utils.MoveFocusWithKey(sender as CheckBox, e);
        }
    }
}
