using System.Collections.ObjectModel;

namespace AIcore
{
    public class ScriptItem
    {
        public ScriptItem(ScriptAction action)
        {
            scriptAction = action;
            Name = action.ToString();
            Summary = Description = "";
            IsAllowedInSkirmish = false;
            ParamAllowed = new ObservableCollection<Parameter>();
        }

        public ScriptAction scriptAction;
        public string SAction { get { return ((int)scriptAction).ToString(); } }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public bool IsAllowedInSkirmish;
        public ObservableCollection<Parameter> ParamAllowed;

        public string GetParamDescription(string param)
        {
            foreach (Parameter p in ParamAllowed)
                if (p.Param == param)
                    return p.Description;
            return "";
        }

        public class Parameter
        {
            public string Param { get; set; }
            public string Description { get; set; }
        }
    }
}
