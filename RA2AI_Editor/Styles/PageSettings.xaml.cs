using AIcore;
using Library;
using RA2AI_Editor.PopupForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static RA2AI_Editor.MainWindow;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace RA2AI_Editor.Styles
{
    /// <summary>
    /// PageSettings.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettings : Page
    {
        public PageSettings()
        {
            InitializeComponent();
            this.DataContext = configData;
        }

        private void AddCustomGame_Click(object sender, RoutedEventArgs e)
        {
            string name = tb_customgame.Text;
            if (name == null || name.Length == 0)
            {
                MainWindow.MessageBoxShow(Local.Dictionary("MB_BLANKCUSTOMGAME"));
                return;
            }
            string newdir = Environment.CurrentDirectory + @"\Custom\" + name;
            if (Directory.Exists(newdir))
            {
                MainWindow.MessageBoxShow(Local.Dictionary("MB_INVALIDCUSTOMGAME"));
                return;
            }

            PathClass.CreateDir(newdir);
            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory + @"\" + MainWindow.GetGameXmlDirRelative()))
                Utils.FileCopy(file, newdir + @"\" + Path.GetFileName(file), true);
            MainWindow.configData?.CreateGame(Game.CreateCustomGameType(name));
        }

        private void DeleteCustomGame_Click(object sender, RoutedEventArgs e)
        {
            if (!Game.IsCustomGameType())
                return;
            if (MainWindow.MessageBoxShow(Local.Dictionary("MB_DELETECUSTOMGAME"), MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                string path = Environment.CurrentDirectory + @"\" + MainWindow.GetGameXmlDirRelative();
                foreach (string file in Directory.GetFiles(path))
                    Utils.FileDelete(file);
            }
            MainWindow.configData?.DeleteCurrentGame();
        }

        private void ARRectangle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = sender as Rectangle;
            ColorDialog cd = new ColorDialog
            {
                FullOpen = true
            };
            SolidColorBrush tmp = rec.Fill as SolidColorBrush;
            cd.Color = System.Drawing.Color.FromArgb(tmp.Color.A, tmp.Color.R, tmp.Color.G, tmp.Color.B);
            if (cd.ShowDialog() == DialogResult.OK)
            {
                rec.Fill = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                MainWindow.current_ai.RefreshAnalysisResult();
            }
            cd.Dispose();
        }

        private void RulesImport_Click(object sender, RoutedEventArgs e)
        {
            string path = Utils.SelectFileToOpen(Local.Dictionary("FILE_OPENRULES"));
            if (path != null)
            {
                if (File.Exists(path))
                {
                    IniClass rules = new IniClass(path);
                    UnitChooseForm ucf = new UnitChooseForm(rules, MainWindow.searchinterval);
                    ucf.Owner = MainWindow.Window;
                    ucf.ShowDialog();

                    switch (ucf.MBResult)
                    {
                        case MessageBoxResult.Yes:
                            if (!ucf.DontImportUnits)
                            {
                                Units.Update(ucf.AllList, ucf.BuildingList, ucf.Units, UnitChooseForm.KeepExistedUnits);
                                Units.Save(false);
                            }
                            if (!ucf.KeepExistedSides)
                                Sides.Update(ucf.SideList);
                            if (!ucf.KeepExistedHouses)
                                Countries.Update(ucf.HouseList);
                            break;
                        //case MessageBoxResult.No:
                        //    units = new Units(alllist, buildingslist, unitslist, UnitChooseForm.KeepExistedUnits);
                        //    units.Save(true);
                        //    break;
                        default:
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(Local.Dictionary("MB_FILENOTEXIST") + path + Local.Dictionary("MB_FILENOTEXIST2"),
                        Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private string SelectCSFFileToOpen()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                RestoreDirectory = true,
                Multiselect = false
            };
            openFileDialog.Title = Local.Dictionary("FILE_OPENCSF");
            openFileDialog.Filter = "Csf Files(*.csf)| *.csf|ALL Files.| *";
            openFileDialog.CheckFileExists = openFileDialog.CheckPathExists = true;
            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private void CsfImport_Click(object sender, RoutedEventArgs e)
        {
            string path = SelectCSFFileToOpen();
            if (path != null)
            {
                if (File.Exists(path))
                {
                    CsfClass csf = new CsfClass(path);
                    if (Units.ImportFromCsf(csf))
                    {
                        Countries.LoadCsfName(csf);
                        MessageBox.Show(Local.Dictionary("MB_CSFIMPORTCOMPLETE"), Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                        MessageBox.Show(Local.Dictionary("MB_INVALIDCSFFILE"), Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(Local.Dictionary("MB_FILENOTEXIST") + path + Local.Dictionary("MB_FILENOTEXIST2"),
                        Local.Dictionary("MB_HINT"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

    }
}
