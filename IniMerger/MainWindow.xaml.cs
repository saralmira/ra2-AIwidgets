using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Library;

namespace IniMerger
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        enum BtnState
        {
            None,
            MouseEnter,
            MouseDown,
            MouseDownOutside
        }

        private BtnState CurState = BtnState.None;

        private void SetImageState(BtnState state)
        {
            switch (state)
            {
                case BtnState.None:
                case BtnState.MouseDownOutside:
                case BtnState.MouseDown:
                    //imae_grid.Background = System.Windows.Media.Brushes.White;
                    ini_image.Margin = new Thickness(5);
                    break;
                case BtnState.MouseEnter:
                    //imae_grid.Background = System.Windows.Media.Brushes.LightGray;
                    ini_image.Margin = new Thickness(3);
                    break;
            }
        }

        private void Ini_image_MouseEnter(object sender, MouseEventArgs e)
        {
            switch(CurState)
            {
                case BtnState.None:
                    CurState = BtnState.MouseEnter;
                    SetImageState(CurState);
                    break;
                case BtnState.MouseDownOutside:
                    if (e.LeftButton == MouseButtonState.Pressed)
                        CurState = BtnState.MouseDown;
                    else
                        CurState = BtnState.MouseEnter;
                    SetImageState(CurState);
                    break;
            }
        }

        private void Ini_image_MouseLeave(object sender, MouseEventArgs e)
        {
            switch (CurState)
            {
                case BtnState.MouseEnter:
                    CurState = BtnState.None;
                    SetImageState(CurState);
                    break;
                case BtnState.MouseDown:
                    CurState = BtnState.MouseDownOutside;
                    SetImageState(CurState);
                    break;
            }
        }

        private void Ini_image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            switch (CurState)
            {
                case BtnState.MouseDown:
                    CurState = BtnState.MouseEnter;
                    SetImageState(CurState);
                    Target_Ini_Choose();
                    break;
            }
        }

        private void Ini_image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            switch (CurState)
            {
                case BtnState.MouseEnter:
                    CurState = BtnState.MouseDown;
                    SetImageState(CurState);
                    break;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            ini_image.Source = BitmapToBitmapImage(Properties.Resources.ini_icon);
            IniPane = ini_src_list;
            le = new ListElement();
            Validate();
        }

        public static StackPanel IniPane { get; private set; }

        private ListElement le;

        private void File_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) != null)
                e.Effects = DragDropEffects.Copy;
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            foreach(string str in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (File.Exists(str))
                {
                    ini_target_tb.Text = str;
                    break;
                }
            }
        }

        // Bitmap --> BitmapImage
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png); // 坑点：格式选Bmp时，不带透明度

                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
                // Force the bitmap to load right now so we can dispose the stream.
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }

        // BitmapImage --> Bitmap
        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private void TB_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(DataFormats.FileDrop) != null)
                e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void Target_Ini_Choose()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Ini Files (*.ini)|*.ini|All Files|*",
                Multiselect = false
            };
            var result = openFileDialog.ShowDialog();
            if (result == true)
            {
                ini_target_tb.Text = openFileDialog.FileName;
            }
        }

        private void SourceClear_Click(object sender, RoutedEventArgs e)
        {
            ini_target_tb.Clear();
            le.Clear();
            if (result != null)
                result.Clear();
            Validate();
        }

        private void Ini_src_list_Drop(object sender, DragEventArgs e)
        {
            foreach (string str in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (File.Exists(str))
                {
                    le.Add(str);
                }
            }
            if (result != null)
                result.Clear();
            Validate();
        }

        private void Ini_target_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            result = null;
            Validate();
        }

        class ListElement
        {
            public List<string> inilist;

            public ListElement()
            {
                inilist = new List<string>();
            }

            public void Add(string inipath)
            {
                if (!inilist.Contains(inipath))
                {
                    inilist.Add(inipath);
                    TextBlock tb = new TextBlock() { Text = inipath, TextWrapping = TextWrapping.Wrap };
                    tb.MouseEnter += Effect_MouseEnter;
                    tb.MouseLeave += Effect_MouseLeave;
                    MainWindow.IniPane.Children.Add(tb);
                }
            }

            public void Clear()
            {
                inilist.Clear();
                MainWindow.IniPane.Children.Clear();
            }

            private void Effect_MouseEnter(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.LightGray;
                }
            }

            private void Effect_MouseLeave(object sender, MouseEventArgs e)
            {
                if (sender is TextBlock)
                {
                    ((TextBlock)sender).Background = System.Windows.Media.Brushes.White;
                }
            }
        }

        private void Validate()
        {
            if (result == null || !result.IsEnabled)
            { btn_cover.IsEnabled = btn_inter.IsEnabled = btn_merge.IsEnabled = false; }
            else
            { btn_cover.IsEnabled = btn_inter.IsEnabled = btn_merge.IsEnabled = true; }
        }

        private void Analyse_Click(object sender, RoutedEventArgs e)
        {
            result = new IniAnalyse(ini_target_tb.Text, le.inilist);
            result.Output(result_list);
            Validate();
        }

        private void Merge_Click(object sender, RoutedEventArgs e)
        {
            if (result.IsEnabled)
            {
                if (((Button)sender) == btn_inter)
                {
                    result.Start_Intersection();
                }
                else
                {
                    List<string> filter = new List<string>();
                    List<string> slist = new List<string>();
                    if ((bool)rb_ai.IsChecked)
                    {
                        slist.Add("TaskForces");
                        slist.Add("ScriptTypes");
                        slist.Add("TeamTypes");
                        filter.Add("AITriggerTypes");
                    }
                    else if ((bool)rb_art.IsChecked)
                    {
                        slist.Add("Movies");
                    }
                    else if ((bool)rb_rules.IsChecked)
                    {
                        slist.Add("InfantryTypes");
                        slist.Add("VehicleTypes");
                        slist.Add("AircraftTypes");
                        slist.Add("BuildingTypes");
                        slist.Add("TerrainTypes");
                        slist.Add("SmudgeTypes");
                        slist.Add("OverlayTypes");
                        slist.Add("Animations");
                        slist.Add("VoxelAnims");
                        slist.Add("Particles");
                        slist.Add("ParticleSystems");
                        slist.Add("SuperWeaponTypes");
                        slist.Add("Warheads");
                        slist.Add("WeaponTypes");
                        slist.Add("Tiberiums");
                        slist.Add("#include");
                        slist.Add("VariableNames");
                        slist.Add("TunnelTypes");
                    }
                    else if ((bool)rb_sound.IsChecked)
                    {
                        slist.Add("SoundList");
                    }
                    else if ((bool)rb_theme.IsChecked)
                    {
                        slist.Add("Themes");
                    }
                    else if ((bool)rb_eva.IsChecked)
                    {
                        slist.Add("DialogList");
                    }
                    else if ((bool)rb_other.IsChecked)
                    {

                    }
                    if (((Button)sender) == btn_cover)
                        result.Start_Cover(filter, slist);
                    else
                        result.Start_Merge(slist);
                }
                MessageBox.Show((string)Application.Current.FindResource("h_finish"), 
                    (string)Application.Current.FindResource("h_hint"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Validate();
        }

        private IniAnalyse result;

        public enum AOptions
        {
            none,
            intersection,
            merge,
            cover
        }

        public class IniAnalyse
        {
            IniClass ctarget;
            List<IniClass> csrc;
            List<UserList.UElement> ue;

            public IniAnalyse(string target, List<string> src)
            {
                if (File.Exists(target))
                {
                    IsEnabled = true;
                    ctarget = new IniClass(target);
                    csrc = new List<IniClass>();
                    ue = new List<UserList.UElement>();
                    IList<string> t_sec = ctarget.GetSections();

                    foreach(string str in src)
                    {
                        if (File.Exists(str))
                        {
                            IniClass iclass = new IniClass(str);
                            UserList.UElement ele = new UserList.UElement(System.IO.Path.GetFileName(str) + " 中：");
                            csrc.Add(iclass);
                            IList<string> t_tar = iclass.GetSections();
                            foreach (string t_str in t_tar)
                            {
                                if (!t_sec.Contains(t_str))
                                {
                                    ele.Add_New(t_str);
                                }
                                else if (ctarget.ReadSection(t_str) != iclass.ReadSection(t_str))
                                {
                                    ele.Add(t_str);
                                }
                            }
                            foreach (string t_str in t_sec)
                            {
                                if (!t_tar.Contains(t_str))
                                {
                                    ele.Add_Dec(t_str);
                                }
                            }
                            ue.Add(ele);
                        }
                    }
                }
                else
                    IsEnabled = false;
            }

            public void Output(UserList ulist)
            {
                if (IsEnabled)
                {
                    ulist.Clear();
                    foreach (UserList.UElement uelement in ue)
                    {
                        ulist.Add(uelement);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sortlist">数字排序的section</param>
            public void Start_Merge(List<string> sortlist, string post = "_merge.ini")
            {
                string output_path = PathClass.FileRename(ctarget.GetPath(), 
                        System.IO.Path.GetFileNameWithoutExtension(ctarget.GetPath()) + post);

                File.Copy(ctarget.GetPath(), output_path, true);
                IniClass oclass = new IniClass(output_path);

                foreach (IniClass ic in csrc)
                {
                    foreach(string section in ic.GetSections())
                    {
                        if (sortlist.Contains(section))
                        {
                            int max = 1, tmp;
                            foreach (string key in oclass.GetKeys(section))
                            {
                                try
                                {
                                    tmp = Convert.ToInt32(key);
                                    if (tmp >= max)
                                        max = tmp + 1;
                                }
                                catch { }
                            }
                            IList<string> cvalues = oclass.GetValues(section);
                            foreach (string value in ic.GetValues(section))
                            {
                                if (cvalues.Contains(value))
                                    continue;
                                oclass.WriteValue(section, max.ToString(), value);
                                ++max;
                            }
                        }
                        else
                        {
                            foreach (string key in ic.GetKeys(section))
                            { oclass.WriteValue(section, key, ic.ReadValue(section, key)); }
                        }
                    }
                }
                oclass.Save();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="filter">保留的section，既保留原来的内容也包括新内容</param>
            /// <param name="sortlist">数字排序的section</param>
            public void Start_Cover(List<string> filter, List<string> sortlist, string output_path = null)
            {
                if (output_path == null)
                    output_path = PathClass.FileRename(ctarget.GetPath(), Path.GetFileNameWithoutExtension(ctarget.GetPath()) + "_cover.ini");

                File.Copy(ctarget.GetPath(), output_path, true);
                IniClass oclass = new IniClass(output_path);

                foreach (IniClass ic in csrc)
                {
                    foreach (string section in ic.GetSections())
                    {
                        if (filter.Contains(section))
                        {
                            foreach (string key in ic.GetKeys(section))
                            { oclass.WriteValue(section, key, ic.ReadValue(section, key)); }
                        }
                        else if (sortlist.Contains(section))
                        {
                            int max = 1, tmp;
                            foreach (string key in oclass.GetKeys(section))
                            {
                                try
                                {
                                    tmp = Convert.ToInt32(key);
                                    if (tmp >= max)
                                        max = tmp + 1;
                                }
                                catch { }
                            }
                            IList<string> cvalues = oclass.GetValues(section);
                            foreach (string value in ic.GetValues(section))
                            {
                                if (cvalues.Contains(value))
                                    continue;
                                oclass.WriteValue(section, max.ToString(), value);
                                ++max;
                            }
                        }
                        else
                        {
                            oclass.WriteValue(section, null, null);
                            foreach (string key in ic.GetKeys(section))
                            { oclass.WriteValue(section, key, ic.ReadValue(section, key)); }
                        }
                    }
                }
                oclass.Save();
            }

            public void Start_Intersection()
            {
                IList<string> sections = ctarget.GetSections();
                string target_name = System.IO.Path.GetFileNameWithoutExtension(ctarget.GetPath());

                foreach (IniClass ic in csrc)
                {
                    string output_path = PathClass.FileRename(ctarget.GetPath(), target_name + "_" +
                        System.IO.Path.GetFileNameWithoutExtension(ic.GetPath()) + ".ini");
                    if (File.Exists(output_path))
                        File.Delete(output_path);
                    IniClass oclass = new IniClass(output_path);
                    foreach (string section in ic.GetSections())
                    {
                        if (sections.Contains(section))
                        {
                            IList<string> keys = ctarget.GetKeys(section);
                            foreach (string key in ic.GetKeys(section))
                            {
                                if (keys.Contains(key))
                                {
                                    string value = ic.ReadValueWithoutNotes(section, key);
                                    if (ctarget.ReadValueWithoutNotes(section, key) != value)
                                        oclass.WriteValue(section, key, value);
                                }
                                else
                                {
                                    oclass.WriteValue(section, key, ic.ReadValue(section, key));
                                }
                            }
                        }
                        else
                        {
                            foreach (string key in ic.GetKeys(section))
                                oclass.WriteValue(section, key, ic.ReadValue(section, key));
                        }
                    }
                    oclass.Save();
                }
            }

            public void UseRules(string output, int type)
            {
                List<string> filter = new List<string>();
                List<string> slist = new List<string>();

                if (type == 1) // ai
                {
                    slist.Add("TaskForces");
                    slist.Add("ScriptTypes");
                    slist.Add("TeamTypes");
                    filter.Add("AITriggerTypes");
                }
                else if (type == 2) // art
                {
                    slist.Add("Movies");
                }
                else if (type == 3) // rules
                {
                    slist.Add("InfantryTypes");
                    slist.Add("VehicleTypes");
                    slist.Add("AircraftTypes");
                    slist.Add("BuildingTypes");
                    slist.Add("TerrainTypes");
                    slist.Add("SmudgeTypes");
                    slist.Add("OverlayTypes");
                    slist.Add("Animations");
                    slist.Add("VoxelAnims");
                    slist.Add("Particles");
                    slist.Add("ParticleSystems");
                    slist.Add("SuperWeaponTypes");
                    slist.Add("Warheads");
                    slist.Add("WeaponTypes");
                    slist.Add("Tiberiums");
                    slist.Add("#include");
                    slist.Add("VariableNames");
                    slist.Add("TunnelTypes");
                }
                else if (type == 4) // sound
                {
                    slist.Add("SoundList");
                }
                else if (type == 5) // theme
                {
                    slist.Add("Themes");
                }
                else if (type == 6) // eva
                {
                    slist.Add("DialogList");
                }
                else if (type == 0)
                    return;

                Start_Cover(filter, slist, output);
            }

            public bool IsEnabled { get; private set; }

            public void Clear()
            {
                csrc.Clear();
                ue.Clear();
                IsEnabled = false;
            }
        }

    }
}
