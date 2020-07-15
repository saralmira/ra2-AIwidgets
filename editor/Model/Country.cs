using System;

namespace AIcore
{
    public class Country
    {
        public Country()
        {
            Index = 0;
            Description = Name = "";
            Side = Sides.AllSide;
            IsEnabled = true;
        }

        public Country(string name, Side side)
        {
            if (name == null)
                name = "";
            NameOrNone = name;
            Side = side;
            IsEnabled = true;
        }

        public string NameOrNone
        {
            get
            {
                if (Name == null || Name.Length == 0)
                    return "<none>";
                return Name;
            }
            private set
            {
                Name = value;
            }
        }
        public string NameOrAll
        {
            get
            {
                if (Name == null || Name.Length == 0)
                    return "<all>";
                return Name;
            }
        }
        public string Name { get; set; }

        public int Index { get; set; }
        public string SIndex
        {
            get
            {
                return Index.ToString();
            }
            set
            {
                Index = Convert.ToInt32(value);
            }
        }

        public Side Side { get; set; }
        public string SSide
        {
            get
            {
                return Side.Name;
            }
            set
            {
                Side = Sides.GetSide(value);
            }
        }

        public string Description { get; set; }
        public string DescriptionOrNone
        {
            get
            {
                if (Description == null || Description.Length == 0)
                    return "<none>";
                return Description;
            }
            private set
            {
                Description = value;
            }
        }
        public string DescriptionOrAll
        {
            get
            {
                if (Description == null || Description.Length == 0)
                    return "<all>";
                return Description;
            }
            private set
            {
                Description = value;
            }
        }

        public bool IsEnabled { get; set; }
        public string UIName;
    }
}
