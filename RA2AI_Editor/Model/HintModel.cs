using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RA2AI_Editor.Model
{
    public interface IYourViewModel
    {
        IEnumerable<string> Names { get; }
        string SelectedName { get; set; }
    }
}
