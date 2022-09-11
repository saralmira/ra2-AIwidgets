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
    /// StateButton.xaml 的交互逻辑
    /// </summary>
    public partial class StateButton : UserControl
    {
        public StateButton()
        {
            InitializeComponent();
        }

        public void Initialize(string text, int max,RoutedEventHandler clickEvent, byte initcolor = 111, byte alpha = 0xAA)
        {
            btn.Content = text;
            this.max = max;
            int layer = max % 6 > 0 ? max / 6 + 1 : max / 6;
            byte gap = (byte)((0xFF - initcolor) / layer);
            brushes = new SolidColorBrush[6 * layer];
            for (byte i = 0; i < layer; ++i)
            {
                brushes[i] = new SolidColorBrush(Color.FromArgb(alpha, initcolor, 0xFF, (byte)(initcolor + i * gap)));
                brushes[i + layer] = new SolidColorBrush(Color.FromArgb(alpha, initcolor, (byte)(0xFF - i * gap), 0xFF));
                brushes[i + 2 * layer] = new SolidColorBrush(Color.FromArgb(alpha, (byte)(initcolor + i * gap), initcolor, 0xFF));
                brushes[i + 3 * layer] = new SolidColorBrush(Color.FromArgb(alpha, 0xFF, initcolor, (byte)(0xFF - i * gap)));
                brushes[i + 4 * layer] = new SolidColorBrush(Color.FromArgb(alpha, 0xFF, (byte)(initcolor + i * gap), initcolor));
                brushes[i + 5 * layer] = new SolidColorBrush(Color.FromArgb(alpha, (byte)(0xFF - i * gap), 0xFF, initcolor));
            }
            OState = State = 0;
            btn.Click += clickEvent;
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            OState = State++;
            if (State >= max)
                State = 0;
        }

        private SolidColorBrush[] brushes;

        private int max;
        private int _state;
        public int State
        {
            get { return _state; }
            set { _state = value; btn.Background = brushes[value]; }
        }
        public int OState;
    }
}
