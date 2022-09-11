using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RA2AI_Editor.Styles
{
    public partial class ScrollViewer
    {
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                (sender as System.Windows.Controls.ScrollViewer).LineUp();
            else if (e.Delta < 0)
                (sender as System.Windows.Controls.ScrollViewer).LineDown();
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

        //private void Popup_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    Popup p = sender as Popup;
        //    p.IsOpen = false;
        //}
    }
}
