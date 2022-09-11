using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIcore;
using AIcore.Types;

namespace RA2AI_Editor.Model
{
    public class AITreeModel : INotifyPropertyChanged
    {
        public AITreeModel()
        {
            TaskForces = new ObservableCollection<TaskForce>();
            ScriptTypes = new ObservableCollection<ScriptType>();

            PropertyChanged += PropertyChangedEvent;
        }

        public ObservableCollection<TaskForce> TaskForces { get; set; }
        public ObservableCollection<ScriptType> ScriptTypes { get; set; }

        private Country _House = Countries.All;
        public Country House
        {
            get { return _House; }
            set
            {
                _House = value;
                PropertyChange("House");
            }
        }
        private Side _Side = Sides.AllSide;
        public Side Side
        {
            get { return _Side; }
            set
            {
                _Side = value;
                PropertyChange("Side");
            }
        }

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
