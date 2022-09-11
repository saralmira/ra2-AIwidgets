using AIcore;
using AIcore.Types;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace RA2AI_Editor.Styles
{
    public partial class TextBox
    {
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag != null && btn.Tag is string)
            {
                btn.Tag = "";
            }
        }

        private void ACBPreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            if ((e.Key == Key.Enter || e.Key == Key.Space) && acb.IsDropDownOpen)
            {
                e.Handled = true;
                acb.IsDropDownOpen = false;
            }
        }

        private void ACBMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Script_Popup(sender as AutoCompleteBox);
            //AutoCompleteBox b = (AutoCompleteBox)sender;
            //b.SetBinding(AutoCompleteBox.ItemsSourceProperty, new Binding() { Source = Scripts.ScriptList, Mode = BindingMode.OneWay });
            //b.IsDropDownOpen = true;
        }

        private void ACBMouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            Parameter_Popup(sender as AutoCompleteBox);
        }

        private void Script_Popup(AutoCompleteBox acb)
        {
            acb.IsDropDownOpen = true;
        }

        private void Parameter_Popup(AutoCompleteBox acb)
        {
            ScriptType.ScriptTypeData std = (ScriptType.ScriptTypeData)acb.DataContext;
            ObservableCollection<ScriptItem.Parameter> p = std.GetParametersAllowed();
            if (p != null)
            {
                if (p == Scripts.ParamBuildingID)
                {
                    acb.IsDropDownOpen = false;
                    PopupForms.BuildingIDForm bf = new PopupForms.BuildingIDForm(Units.GetCleanBuildingList(), acb.Text, MainWindow.searchinterval);
                    if (Application.Current.MainWindow != null)
                        bf.Owner = Application.Current.MainWindow;
                    bf.ShowDialog();
                    if (bf.Result != null)
                        acb.Text = bf.Result;
                    return;
                }
                else if (p == Scripts.ParamScripts && !acb.IsDropDownOpen)
                {
                    Scripts.ParamScripts.Clear();
                    for (int i = 0; i < AI.scripttypes_info.Count; ++i)
                        Scripts.ParamScripts.Add(new ScriptItem.Parameter() { Param = i.ToString(), Description = AI.scripttypes_info[i].PName });
                }
                else if (p == Scripts.ParamTeams && !acb.IsDropDownOpen)
                {
                    Scripts.ParamTeams.Clear();
                    for (int i = 0; i < AI.teamtypes_info.Count; ++i)
                        Scripts.ParamTeams.Add(new ScriptItem.Parameter() { Param = i.ToString(), Description = AI.teamtypes_info[i].PName });
                }
                if (acb.ItemsSource == null || acb.Tag == null || std.Action != (int)acb.Tag)
                {
                    acb.Tag = std.Action;
                    acb.SetBinding(AutoCompleteBox.ItemsSourceProperty, new Binding() { Source = p, Mode = BindingMode.OneWay });
                }
                if (p.Count == 1 && p[0].Param.Length > 0)
                    acb.Text = p[0].Param;
            }
            acb.IsDropDownOpen = true;
        }

        private void Script_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Space:
                    if (!acb.IsDropDownOpen)
                    {
                        Script_Popup(acb);
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (acb.IsDropDownOpen)
                    {
                        acb.IsDropDownOpen = false;
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void Parameter_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            AutoCompleteBox acb = sender as AutoCompleteBox;
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                case Key.Space:
                    if (!acb.IsDropDownOpen)
                    { 
                        Parameter_Popup(acb);
                        e.Handled = true;
                    }
                    break;
                case Key.Escape:
                    if (acb.IsDropDownOpen)
                    { 
                        acb.IsDropDownOpen = false;
                        e.Handled = true;
                    }
                    break;
            }
        }
    }

    public class ControlAttachProperty
    {
        #region 圆角

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 删除按妞区域模板

        public static ControlTemplate GetAttachTemplate(DependencyObject obj)
        {
            return (ControlTemplate)obj.GetValue(AttachTemplateProperty);
        }

        public static void SetAttachTemplate(DependencyObject obj, ControlTemplate value)
        {
            obj.SetValue(AttachTemplateProperty, value);
        }

        // Using a DependencyProperty as the backing store for AttachTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttachTemplateProperty =
            DependencyProperty.RegisterAttached("AttachTemplate", typeof(ControlTemplate), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 用户名水印

        public static string GetUserNameWaterMark(DependencyObject obj)
        {
            return (string)obj.GetValue(UserNameWaterMarkProperty);
        }

        public static void SetUserNameWaterMark(DependencyObject obj, string value)
        {
            obj.SetValue(UserNameWaterMarkProperty, value);
        }

        // Using a DependencyProperty as the backing store for UserNameWaterMark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameWaterMarkProperty =
            DependencyProperty.RegisterAttached("UserNameWaterMark", typeof(string), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 密码水印

        public static string GetPasswordWaterMark(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordWaterMarkProperty);
        }

        public static void SetPasswordWaterMark(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordWaterMarkProperty, value);
        }

        // Using a DependencyProperty as the backing store for PasswordWaterMark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordWaterMarkProperty =
            DependencyProperty.RegisterAttached("PasswordWaterMark", typeof(string), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 用户头像(未被点击时)

        public static string GetUserIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(UserIconProperty);
        }

        public static void SetUserIcon(DependencyObject obj, string value)
        {
            obj.SetValue(UserIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for UserIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserIconProperty =
            DependencyProperty.RegisterAttached("UserIcon", typeof(string), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 用户头像(点击时)

        public static string GetUserIconPress(DependencyObject obj)
        {
            return (string)obj.GetValue(UserIconPressProperty);
        }

        public static void SetUserIconPress(DependencyObject obj, string value)
        {
            obj.SetValue(UserIconPressProperty, value);
        }

        // Using a DependencyProperty as the backing store for UserIconPress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserIconPressProperty =
            DependencyProperty.RegisterAttached("UserIconPress", typeof(string), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 密码图标(未被点击时)

        public static string GetPassWordIcon(DependencyObject obj)
        {
            return (string)obj.GetValue(PassWordIconProperty);
        }

        public static void SetPassWordIcon(DependencyObject obj, string value)
        {
            obj.SetValue(PassWordIconProperty, value);
        }

        // Using a DependencyProperty as the backing store for PassWordIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PassWordIconProperty =
            DependencyProperty.RegisterAttached("PassWordIcon", typeof(string), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 密码图标(点击时)

        public static string GetPasswordIconPress(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordIconPressProperty);
        }

        public static void SetPasswordIconPress(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordIconPressProperty, value);
        }

        // Using a DependencyProperty as the backing store for PasswordIconPress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordIconPressProperty =
            DependencyProperty.RegisterAttached("PasswordIconPress", typeof(string), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 删除按妞背景图片

        public static ImageBrush GetDeleteButtonBG(DependencyObject obj)
        {
            return (ImageBrush)obj.GetValue(DeleteButtonBGProperty);
        }

        public static void SetDeleteButtonBG(DependencyObject obj, ImageBrush value)
        {
            obj.SetValue(DeleteButtonBGProperty, value);
        }

        // Using a DependencyProperty as the backing store for DeleteButtonBG.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeleteButtonBGProperty =
            DependencyProperty.RegisterAttached("DeleteButtonBG", typeof(ImageBrush), typeof(ControlAttachProperty), new PropertyMetadata(null));

        #endregion

        #region 定义是否开启绑定事件


        public static bool GetIsCommandClearTextEvent(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCommandClearTextEventProperty);
        }

        public static void SetIsCommandClearTextEvent(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCommandClearTextEventProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsCommandClearTextEvent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCommandClearTextEventProperty =
            DependencyProperty.RegisterAttached("IsCommandClearTextEvent", typeof(bool), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(false, IsCommandClearTextEventChanged));

        private static void IsCommandClearTextEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
        #endregion

        #region 是否显示密码样式

        public static bool GetIsVisiblityPassword(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsVisiblityPasswordProperty);
        }

        public static void SetIsVisiblityPassword(DependencyObject obj, bool value)
        {
            obj.SetValue(IsVisiblityPasswordProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsVisiblityPassword.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsVisiblityPasswordProperty =
            DependencyProperty.RegisterAttached("IsVisiblityPassword", typeof(bool), typeof(ControlAttachProperty), new PropertyMetadata(false));

        #endregion

        #region 清除事件命令

        public static bool GetIsClearTextButtonBehaviorEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsClearTextButtonBehaviorEnabledProperty);
        }

        public static void SetIsClearTextButtonBehaviorEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsClearTextButtonBehaviorEnabledProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsClearTextButtonBehaviorEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsClearTextButtonBehaviorEnabledProperty =
            DependencyProperty.RegisterAttached("IsClearTextButtonBehaviorEnabled", typeof(bool), typeof(ControlAttachProperty), new FrameworkPropertyMetadata(false, IsClearTextButtonBehaviorEnabledChanged));

        /// <summary>
        /// 当附加属性值发生改变时，调用此方法
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsClearTextButtonBehaviorEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as Button;
            if (e.OldValue != e.NewValue)
            {
                //当命令触发的时候，会向上传递，而此时这个命令外围就是自己本身
                button.CommandBindings.Add(ClearTextCommandBinding);
            }
        }

        /**
         * 当命令触发的时候，会一级一级向上传递，当传递到命令关联者时，会处理这个命令
         */

        /// <summary>
        /// 创建一个命令
        /// </summary>
        public static RoutedUICommand ClearTextCommand { get; private set; }

        /// <summary>
        /// 命令绑定关联
        /// </summary>
        private static readonly CommandBinding ClearTextCommandBinding;

        private static void ClearButtonClick(object sender, ExecutedRoutedEventArgs e)
        {
            var tbox = e.Parameter as FrameworkElement;
            if (tbox == null) return;
            if (e.Parameter is TextBox)
            {
                ((TextBox)e.Parameter).Clear();
            }

            tbox.Focus();
        }

        #endregion

        static ControlAttachProperty()
        {
            ClearTextCommand = new RoutedUICommand();

            ClearTextCommandBinding = new CommandBinding();
            //将者命令加入到这个命令关联中，如果某个控件调用了这个命令，只要他所在的层级中有关联这个命令关联对象，那么这个命令对象就会对其进行处理
            ClearTextCommandBinding.Command = ClearTextCommand;
            ClearTextCommandBinding.Executed += ClearButtonClick;
        }
    }
}
