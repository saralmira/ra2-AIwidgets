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
using System.Windows.Shapes;
using AIcore;

namespace RA2AI_Editor.PopupForms
{
    /// <summary>
    /// GameTypeChoose.xaml 的交互逻辑
    /// </summary>
    public partial class GameTypeChoose : Window
    {
        public GameTypeChoose(Game.GameType def = Game.GameType.YR)
        {
            InitializeComponent();
            InitGame(def == Game.GameType.RA ? Game.DefaultRAGame : Game.DefaultYRGame);

            //gd = new GridData(def);
            //grid.DataContext = gd;
            //defvalue = def;
            ChoseGame = Game.DefaultUnknownGame;
        }

        private void InitGame(Game.GameTypeClass def)
        {
            foreach (Game.GameTypeClass game in Game.GameLists)
            {
                RadioButton rb = new RadioButton
                {
                    Content = game.Description,
                    Margin = new Thickness(5, 2, 5, 2),
                    GroupName = "MODE",
                    Style = (Style)this.FindResource("RadioButtonItemStyle")
                };
                rb.Tag = game;
                if (game == def)
                    rb.IsChecked = true;
                else
                    rb.IsChecked = false;
                gamepanel.Children.Add(rb);
            }
        }

        public Game.GameTypeClass ChoseGame { get; private set; }
        //public Game.GameType GameType { get { return gd.GameType; } }
        //private Game.GameType defvalue;
        //private GridData gd;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton rb in gamepanel.Children)
                if (rb.IsChecked == true)
                    ChoseGame = rb.Tag as Game.GameTypeClass;
            this.DialogResult = true;
            Close();
        }

        public static Game.GameTypeClass Choose(Game.GameType def = Game.GameType.YR)
        {
            GameTypeChoose gtc = new GameTypeChoose(def);
            if (Application.Current.MainWindow.IsVisible)
                gtc.Owner = Application.Current.MainWindow;
            gtc.ShowDialog();
            return gtc.DialogResult == true ? gtc.ChoseGame : null;
        }

        private class GridData : INotifyPropertyChanged
        {
            public GridData(Game.GameType def)
            {
                PropertyChanged += PropertyChangedEvent;
                GameType = def;
            }

            private Game.GameType _gt;
            public Game.GameType GameType { get { return _gt; } set { _gt = value; PropertyChanged.Invoke(this, new PropertyChangedEventArgs("GameType")); } }
            //public bool gmode1 { get { return GameType == Game.GameType.TS; } set { if (value) { GameType = Game.GameType.TS; PropertyChange(this); } } }
            public bool gmode2 { get { return GameType == Game.GameType.RA; } set { if (value) { GameType = Game.GameType.RA; PropertyChange(this); } } }
            public bool gmode3 { get { return GameType == Game.GameType.YR; } set { if (value) { GameType = Game.GameType.YR; PropertyChange(this); } } }

            protected void PropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {

            }
            protected void PropertyChange(object sender)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("gmode1"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("gmode2"));
                    PropertyChanged.Invoke(sender, new PropertyChangedEventArgs("gmode3"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

    }
}
