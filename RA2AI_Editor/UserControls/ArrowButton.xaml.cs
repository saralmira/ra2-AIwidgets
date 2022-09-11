using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RA2AI_Editor.UserControls
{
    /// <summary>
    /// ArrowButton.xaml 的交互逻辑
    /// </summary>
    public partial class ArrowButton : UserControl
    {
        public ArrowButton()
        {
            InitializeComponent();

            btnClass = new BtnClass(this);
            grid.DataContext = btnClass;
        }

        public void Transform()
        {
            btnClass.State = !btnClass.State;
            Column = btnClass.State ? 0 : 1;
        }

        public int GetColumn()
        {
            return btnClass.State ? 0 : 1;
        }

        public void AddClickEvent(RoutedEventHandler clickEvent)
        {
            btn.Click += clickEvent;
        }

        public bool State { get { return btnClass.State; } }

        public Expander expander;

        private BtnClass btnClass;

        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.Register(
            "Column", typeof(int), typeof(ArrowButton),
              new FrameworkPropertyMetadata());

        public int Column
        {
            get
            {
                return (int)GetValue(ColumnProperty);
            }
            set
            {
                SetValue(ColumnProperty, value);
            }
        }

        private class BtnClass : INotifyPropertyChanged
        {
            public BtnClass(ArrowButton ab)
            {
                State = true;
                AB = ab;
                PropertyChanged += PropertyChangedEvent;
            }

            private bool _State;
            public bool State { get { return _State; }set { _State = value; PropertyChange("State"); } }

            private ArrowButton _ab;
            public ArrowButton AB { get { return _ab; } set { _ab = value; PropertyChange("AB"); } }

            protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {

            }
            protected void PropertyChange(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}
