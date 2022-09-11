using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AIcore;
using AIcore.Types;

namespace RA2AI_Editor.PopupForms
{
    /// <summary>
    /// ReplaceForm.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceForm : Window
    {
        private static ReplaceForm currentform = null;
        private static bool isOpen = false;
        public static void Popup(Window owner, Replace replaceFunc)
        {
            if (isOpen)
            {
                currentform?.Activate();
                return;
            }
            isOpen = true;
            currentform = new ReplaceForm(replaceFunc);
            currentform.Owner = owner;
            currentform.Show();
        }

        public delegate int Replace(string src, string dest);

        public ReplaceForm(Replace replace)
        {
            InitializeComponent();
            unit1 = new Unit();
            unit2 = new Unit();
            tb_find.DataContext = unit1;
            tb_replace.DataContext = unit2;
            ReplaceEvent += replace;
        }

        private Unit unit1;
        private Unit unit2;
        public event Replace ReplaceEvent;

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceEvent == null)
                return;
            var i = ReplaceEvent.Invoke(unit1.NameLabel, unit2.NameLabel);
            tb_info.Text = string.Format(Local.Dictionary("STR_REPLACEINFO"), i);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isOpen = false;
        }
    }
}
