using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace RA2AI_Editor.Misc
{
    public class IDValidationRule : ValidationRule
    {
        private static readonly ValidationResult DefResult = new ValidationResult(true, null);

        private bool ValidateLogic(string str)
        {
            if (str.StartsWith("-"))
                return false;
            return Regex.IsMatch(str.Replace("-", ""), @"^[0-9A-Za-z]+$");
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!ValidateLogic(value.ToString()))
                return new ValidationResult(false, Local.Dictionary("EXC_INVALIDID"));
            return DefResult;
        }
    }

    public class NameValidationRule : ValidationRule
    {
        private static readonly char[] InvalidChars = { '[', ']', ';', '=' };
        private static readonly ValidationResult DefResult = new ValidationResult(true, null);

        private bool ValidateLogic(string str)
        {
            return str.IndexOfAny(InvalidChars) < 0;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!ValidateLogic(value.ToString()))
                return new ValidationResult(false, Local.Dictionary("EXC_INVALIDCHAR"));
            return DefResult;
        }
    }

    public class TriggerNameValidationRule : ValidationRule
    {
        private static readonly char[] InvalidChars = { '[', ']', ';', '=', ',' };
        private static readonly ValidationResult DefResult = new ValidationResult(true, null);

        private bool ValidateLogic(string str)
        {
            return str.IndexOfAny(InvalidChars) < 0;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (!ValidateLogic(value.ToString()))
                return new ValidationResult(false, Local.Dictionary("EXC_INVALIDCHARINAI"));
            return DefResult;
        }
    }
}
