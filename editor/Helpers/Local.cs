using AIcore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RA2AI_Editor
{
    public class Local
    {
        public static string Dictionary(string key)
        {
            return (string)Application.Current.FindResource(key);
        }

        public static ListBox FindListBoxInElement(UIElementCollection uicollection, string name)
        {
            foreach (UIElement u in uicollection)
            {
                if (u is ListBox)
                {
                    ListBox lb = (ListBox)u;
                    if (lb.Name == name)
                        return lb;
                }
            }
            return null;
        }


        public static CommandStack GlobalCommandStack = new CommandStack();
    }
}
