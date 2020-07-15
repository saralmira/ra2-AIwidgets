using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace RA2AI_Editor.Model
{
    public class HistoryItemInfo : INotifyPropertyChanged
    {
        public Brush UserBackground { get; set; }
        public Brush UserBackground2 { get; set; }
        private string header;
        public string Header {
            get { return header; }
            set { header = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Header")); } }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Mark { get; set; }
        public string BtnName { get; set; }
        public bool IsEnabled { get; set; }
        public Visibility BtnVisibility { get; set; }
        private int width;
        public int Width
        {
            get { return width; }
            set
            {
                width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Width"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

}
