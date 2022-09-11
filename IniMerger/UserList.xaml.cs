using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IniMerger
{
    /// <summary>
    /// UserList.xaml 的交互逻辑
    /// </summary>
    public partial class UserList : UserControl
    {
        private static StackPanel sp;
        private ListElement le;

        public UserList()
        {
            InitializeComponent();
            sp = src_list;
            le = new ListElement();
        }

        public void Add(UElement item)
        {
            if (item.Title != null)
            {
                Border bd = new Border();
                bd.BorderThickness = new Thickness(2);
                bd.BorderBrush = Brushes.LightSteelBlue;
                TextBlock t_tb = new TextBlock() { Text = item.Title, TextWrapping = TextWrapping.Wrap };
                bd.Child = t_tb;
                sp.Children.Add(bd);
            }
            
            foreach(string str in item.elelist)
                le.Add(str);
            foreach (string str in item.elenewlist)
                le.Add_New(str);
            foreach (string str in item.eledeclist)
                le.Add_Dec(str);
        }

        public void Clear()
        {
            le.Clear();
        }

        public class UElement
        {
            public string Title;
            public List<string> elelist;
            public List<string> elenewlist;
            public List<string> eledeclist;

            public UElement(string title = null)
            {
                Title = (title == null ? null : title);
                elelist = new List<string>();
                elenewlist = new List<string>();
                eledeclist = new List<string>();
            }

            public void Add(string item)
            {
                elelist.Add(item);
            }

            public void Add_New(string item)
            {
                elenewlist.Add(item);
            }

            public void Add_Dec(string item)
            {
                eledeclist.Add(item);
            }
        }

        class ListElement
        {
            //public List<string> inilist;

            public ListElement()
            {
                //inilist = new List<string>();
            }

            public void Add(string item)
            {
                //if (!inilist.Contains(item))
                //{
                //    inilist.Add(item);
                    TextBlock tb = new TextBlock() { Text = item, TextWrapping = TextWrapping.Wrap };
                    tb.MouseEnter += Effect_MouseEnter;
                    tb.MouseLeave += Effect_MouseLeave;
                    sp.Children.Add(tb);
                //}
            }

            public void Add_New(string item)
            {
                //if (!inilist.Contains(item))
                //{
                //    inilist.Add(item);
                    TextBlock tb = new TextBlock() { Text = item, TextWrapping = TextWrapping.Wrap };
                    tb.Background = System.Windows.Media.Brushes.LightGreen;
                    tb.MouseEnter += Effect_MouseEnter2;
                    tb.MouseLeave += Effect_MouseLeave2;
                    sp.Children.Add(tb);
                //}
            }

            public void Add_Dec(string item)
            {
                //if (!inilist.Contains(item))
                //{
                //    inilist.Add(item);
                    TextBlock tb = new TextBlock() { Text = item, TextWrapping = TextWrapping.Wrap };
                    tb.Background = System.Windows.Media.Brushes.LightPink;
                    tb.MouseEnter += Effect_MouseEnter3;
                    tb.MouseLeave += Effect_MouseLeave3;
                    sp.Children.Add(tb);
                //}
            }

            public void Clear()
            {
                //inilist.Clear();
                sp.Children.Clear();
            }

            private void Effect_MouseEnter(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.LightGray;
                }
            }

            private void Effect_MouseLeave(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.White;
                }
            }

            private void Effect_MouseEnter2(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.Green;
                }
            }

            private void Effect_MouseLeave2(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.LightGreen;
                }
            }

            private void Effect_MouseEnter3(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.Red;
                }
            }

            private void Effect_MouseLeave3(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.LightPink;
                }
            }
        }
    }
}
