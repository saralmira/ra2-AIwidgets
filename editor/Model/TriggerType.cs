using System;

namespace AIcore
{
    public class TriggerType
    {
        public TriggerType()
        {
            Value = TriggerTypeEnum.None;
            Name = Description = "";
        }

        public bool CompareWith(TriggerType a)
        {
            return a.Value == Value;
        }

        public TriggerTypeEnum Value;
        public string SValue
        {
            get { return ((int)Value).ToString(); }
            set { try { Value = (TriggerTypeEnum)Convert.ToInt32(value); } catch { Value = TriggerTypeEnum.None; } }
        }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
