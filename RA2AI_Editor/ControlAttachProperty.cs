using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RA2AI_Editor
{
    public class ControlAttachProperty
    {
        #region FocusBorderBrush 焦点边框色，输入控件

        public static readonly DependencyProperty FocusBorderBrushProperty = DependencyProperty.RegisterAttached(
            "FocusBorderBrush", typeof(Brush), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));
        public static void SetFocusBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(FocusBorderBrushProperty, value);
        }
        public static Brush GetFocusBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(FocusBorderBrushProperty);
        }

        #endregion

        #region MouseOverBorderBrush 鼠标进入边框色，输入控件

        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.RegisterAttached("MouseOverBorderBrush", typeof(Brush), typeof(ControlAttachProperty),
                new FrameworkPropertyMetadata(Brushes.Transparent,
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Sets the brush used to draw the mouse over brush.
        /// </summary>
        public static void SetMouseOverBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(MouseOverBorderBrushProperty, value);
        }

        /// <summary>
        /// Gets the brush used to draw the mouse over brush.
        /// </summary>
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        [AttachedPropertyBrowsableForType(typeof(CheckBox))]
        [AttachedPropertyBrowsableForType(typeof(RadioButton))]
        [AttachedPropertyBrowsableForType(typeof(DatePicker))]
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        [AttachedPropertyBrowsableForType(typeof(RichTextBox))]
        public static Brush GetMouseOverBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(MouseOverBorderBrushProperty);
        }

        #endregion

        #region AttachContentProperty 附加组件模板
        /// <summary>
        /// 附加组件模板
        /// </summary>
        public static readonly DependencyProperty AttachContentProperty = DependencyProperty.RegisterAttached(
            "AttachContent", typeof(ControlTemplate), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

        public static ControlTemplate GetAttachContent(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(AttachContentProperty);
        }

        public static void SetAttachContent(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(AttachContentProperty, value);
        }
        #endregion

        #region WatermarkProperty 水印
        /// <summary>
        /// 水印
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.RegisterAttached(
            "Watermark", typeof(string), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(""));

        public static string GetWatermark(DependencyObject d)
        {
            return (string)d.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }
        #endregion

        #region CornerRadiusProperty Border圆角
        /// <summary>
        /// Border圆角
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
            "CornerRadius", typeof(CornerRadius), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

        public static CornerRadius GetCornerRadius(DependencyObject d)
        {
            return (CornerRadius)d.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }
        #endregion

        #region LabelProperty TextBox的头部Label
        /// <summary>
        /// TextBox的头部Label
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached(
            "Label", typeof(string), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static string GetLabel(DependencyObject d)
        {
            return (string)d.GetValue(LabelProperty);
        }

        public static void SetLabel(DependencyObject obj, string value)
        {
            obj.SetValue(LabelProperty, value);
        }
        #endregion

        #region LabelTemplateProperty TextBox的头部Label模板
        /// <summary>
        /// TextBox的头部Label模板
        /// </summary>
        public static readonly DependencyProperty LabelTemplateProperty = DependencyProperty.RegisterAttached(
            "LabelTemplate", typeof(ControlTemplate), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static ControlTemplate GetLabelTemplate(DependencyObject d)
        {
            return (ControlTemplate)d.GetValue(LabelTemplateProperty);
        }

        public static void SetLabelTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(LabelTemplateProperty, value);
        }
        #endregion
    }
}
