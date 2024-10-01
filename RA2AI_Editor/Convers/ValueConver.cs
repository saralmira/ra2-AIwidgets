using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using AIcore.Types;
using AIcore;
using System.Windows.Media.Imaging;

namespace RA2AI_Editor.Convers
{
    public class IndentConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double colunwidth = 10;
            double left = 0.0;


            UIElement element = value as TreeViewItem;
            while (element.GetType() != typeof(TreeView))
            {
                element = (UIElement)VisualTreeHelper.GetParent(element);
                if (element.GetType() == typeof(TreeViewItem))
                    left += colunwidth;
            }
            return new Thickness(left, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class BoolToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToVisibleR : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectToBoolR : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectToCollapsed : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToReverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibleToReverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DateConvertToColor : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            var calendarDayButton = (CalendarDayButton)values;
            var dateTime = (DateTime)calendarDayButton.DataContext;
            if (!calendarDayButton.IsMouseOver && !calendarDayButton.IsSelected && !calendarDayButton.IsBlackedOut && (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday))
                return new SolidColorBrush(Color.FromArgb(255, 255, 47, 47));
            else
                return new SolidColorBrush(Color.FromArgb(255, 51, 51, 51));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32((string)value);
            }
            catch
            {
                return 0;
            }
        }
    }

    public class StringToInt : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt32((string)value);
            }
            catch
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value).ToString();
        }
    }

    public class UIntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((uint)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToUInt32((string)value);
            }
            catch
            {
                return 0;
            }
        }
    }

    public class UIntToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble((UInt32)value);
            }
            catch
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToUInt32((double)value);
            }
            catch
            {
                return 0;
            }
        }
    }

    public class ScriptActionToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringInList : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value? "√" : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "√" ? true : false;
        }
    }

    public class BoolToBackGround : IValueConverter
    {
        private static Brush TrueBrush = new SolidColorBrush(Color.FromRgb(0xfe, 0xf9, 0xeb));
        private static Brush FalseBrush = new SolidColorBrush(Color.FromRgb(211, 211, 211));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueBrush;
            else
                return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToBackGround2 : IValueConverter
    {
        private static Brush TrueBrush = new SolidColorBrush(Color.FromRgb(0xfc, 0xee, 0xb9));
        private static Brush FalseBrush = new SolidColorBrush(Color.FromRgb(201, 201, 201));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueBrush;
            else
                return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToBackGround3 : IValueConverter
    {
        private static Brush TrueBrush = new SolidColorBrush(Color.FromRgb(0xfa, 0xe3, 0x88));
        private static Brush FalseBrush = new SolidColorBrush(Color.FromRgb(160, 160, 160));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueBrush;
            else
                return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class UnitTypeToString : IValueConverter
    {
        private static string AircraftType = Local.Dictionary("TYPE_AIRCRAFT");
        private static string InfantryType = Local.Dictionary("TYPE_INFANTRY");
        private static string VehicleType = Local.Dictionary("TYPE_VEHICLE");
        private static string BuildingType = Local.Dictionary("TYPE_BUILDING");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((UnitType)value)
            {
                case UnitType.AircraftType:
                    return AircraftType;
                case UnitType.InfantryType:
                    return InfantryType;
                case UnitType.VehicleType:
                    return VehicleType;
                case UnitType.BuildingType:
                    return BuildingType;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToImage : IValueConverter
    {
        private static BitmapImage TrueImage = new BitmapImage(new Uri("pack://application:,,,/Images/checked.png"));
        private static BitmapImage FalseImage = null;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueImage;
            return FalseImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToForeGround : IValueConverter
    {
        private static Brush TrueBrush = new SolidColorBrush(Color.FromRgb(0xD9, 0x57, 0x00));
        private static Brush FalseBrush = new SolidColorBrush(Color.FromRgb(0xBB, 0xBB, 0xBB));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueBrush;
            else
                return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToForeGround2 : IValueConverter
    {
        private static Brush TrueBrush = new SolidColorBrush(Color.FromRgb(0x50, 0x50, 0x50));
        private static Brush FalseBrush = new SolidColorBrush(Color.FromRgb(0xAA, 0xAA, 0xAA));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return TrueBrush;
            else
                return FalseBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToWidth : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return 80;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ByteToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((byte)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToByte((string)value);
            }
            catch
            {
                return 0;
            }
        }
    }

    public class TriggerToVisible1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TriggerType t = (TriggerType)value;
            switch (t.Value)
            {
                case TriggerTypeEnum.None:
                case TriggerTypeEnum.EnermyPower:
                case TriggerTypeEnum.EnermyLackofPower:
                case TriggerTypeEnum.EnermyBonusCondition:
                case TriggerTypeEnum.IronCurtainReady:
                case TriggerTypeEnum.ChronoSphereReady:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TriggerToVisible2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TriggerType t = (TriggerType)value;
            switch (t.Value)
            {
                case TriggerTypeEnum.None:
                case TriggerTypeEnum.EnermyPower:
                case TriggerTypeEnum.EnermyLackofPower:
                case TriggerTypeEnum.IronCurtainReady:
                case TriggerTypeEnum.ChronoSphereReady:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TriggerToVisible3 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TriggerType t = (TriggerType)value;
            switch (t.Value)
            {
                case TriggerTypeEnum.None:
                case TriggerTypeEnum.EnermyPower:
                case TriggerTypeEnum.EnermyLackofPower:
                case TriggerTypeEnum.EnermyBonusCondition:
                case TriggerTypeEnum.IronCurtainReady:
                case TriggerTypeEnum.ChronoSphereReady:
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DatetimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((DateTime)value).ToString(Local.Dictionary("DATE_FORMAT"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleToHalf : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value) / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class YRGameToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((Game.GameTypeClass)value).GameType == Game.GameType.YR)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FalseWhileAnalyse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? true : !((IniAnalyse)value).ShowResults;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CollapsedWhileAnalyse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null && ((IniAnalyse)value).ShowResults) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AnalysisResultToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((AnalysisResult)value == AnalysisResult.Different)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AnalysisResultToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((AnalysisResult)value == AnalysisResult.Different)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AnalysisResultToBrush_Bg : IValueConverter
    {
        public static Brush BrushNone = new SolidColorBrush(Color.FromRgb(0xfe, 0xf9, 0xeb));
        public static Brush BrushSame = new SolidColorBrush(Color.FromRgb(225, 225, 225));
        public static Brush BrushDifferent = new SolidColorBrush(Color.FromRgb(255, 128, 64));
        public static Brush BrushOnlyInSource = new SolidColorBrush(Color.FromRgb(128, 255, 64));
        public static Brush BrushOnlyInTarget = new SolidColorBrush(Color.FromRgb(64, 128, 255));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((AnalysisResult)value)
            {
                case AnalysisResult.None:
                    return BrushNone;
                case AnalysisResult.Same:
                    return BrushSame;
                case AnalysisResult.Different:
                    return BrushDifferent;
                case AnalysisResult.OnlyInSource:
                    return BrushOnlyInSource;
                case AnalysisResult.OnlyInTarget:
                    return BrushOnlyInTarget;
                default:
                    return BrushNone;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AnalysisResultToBrush_MOver : IValueConverter
    {
        public static Brush BrushNone = new SolidColorBrush(Color.FromRgb(0xfc, 0xee, 0xb9));
        public static Brush BrushSame = new SolidColorBrush(Color.FromRgb(212, 212, 212));
        public static Brush BrushDifferent = new SolidColorBrush(Color.FromRgb(255, 96, 32));
        public static Brush BrushOnlyInSource = new SolidColorBrush(Color.FromRgb(96, 255, 32));
        public static Brush BrushOnlyInTarget = new SolidColorBrush(Color.FromRgb(32, 96, 255));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((AnalysisResult)value)
            {
                case AnalysisResult.None:
                    return BrushNone;
                case AnalysisResult.Same:
                    return BrushSame;
                case AnalysisResult.Different:
                    return BrushDifferent;
                case AnalysisResult.OnlyInSource:
                    return BrushOnlyInSource;
                case AnalysisResult.OnlyInTarget:
                    return BrushOnlyInTarget;
                default:
                    return BrushNone;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class AnalysisResultToBrush_MSelected : IValueConverter
    {
        public static Brush BrushNone = new SolidColorBrush(Color.FromRgb(0xfa, 0xe3, 0x88));
        public static Brush BrushSame = new SolidColorBrush(Color.FromRgb(195, 195, 195));
        public static Brush BrushDifferent = new SolidColorBrush(Color.FromRgb(255, 64, 0));
        public static Brush BrushOnlyInSource = new SolidColorBrush(Color.FromRgb(64, 255, 0));
        public static Brush BrushOnlyInTarget = new SolidColorBrush(Color.FromRgb(0, 64, 255));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((AnalysisResult)value)
            {
                case AnalysisResult.None:
                    return BrushNone;
                case AnalysisResult.Same:
                    return BrushSame;
                case AnalysisResult.Different:
                    return BrushDifferent;
                case AnalysisResult.OnlyInSource:
                    return BrushOnlyInSource;
                case AnalysisResult.OnlyInTarget:
                    return BrushOnlyInTarget;
                default:
                    return BrushNone;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
