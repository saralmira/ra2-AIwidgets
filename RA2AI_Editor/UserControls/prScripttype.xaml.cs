using AIcore.Types;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RA2AI_Editor.UserControls
{
    /// <summary>
    /// prScripttype.xaml 的交互逻辑
    /// </summary>
    public partial class prScripttype : UserControl
    {
        public prScripttype()
        {
            InitializeComponent();
        }

        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(prScripttype), new PropertyMetadata(""));

    }
}
