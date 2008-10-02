/// Copyright (C) 2008 Matthew Ng
/// 
/// This program is free software; you can redistribute it and/or
/// modify it under the terms of the GNU General Public License
/// as published by the Free Software Foundation; either version 2
/// of the License, or (at your option) any later version.
/// 
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License
/// along with this program; if not, write to the Free Software
/// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Threading;


namespace MoonView
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WinMain : Window
    {
        private ClsNavTracker _navTracker;
        private WinView _viewWindow;
        private Binding _fsItemBind;        
        private ClsFileSystemItemCollection _fsColl = new ClsFileSystemItemCollection();

        //private Binding _textBind;
        //private PhotoCollection Photos;

        public WinMain(string filePath)
        {
            InitializeComponent();
            //ThumbnailCache.Initialize();

            //Set current path
            //TODO Open last path
            string path;
            if (filePath != null)
                path = filePath;
            else
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);            

            _viewWindow = new WinView();
            ClsThumbnailLoader.WinMain = this;
            ClsImageCache.WinMain = this;
            _navTracker = new ClsNavTracker(path);

            _fsItemBind = new Binding();
            _fsItemBind.Source = _fsColl;
            ThumbnailListBox.SetBinding(ListView.ItemsSourceProperty, _fsItemBind);

            //_textBind = new Binding();
            //_textBind.Source = _fsColl;
            //_textBind.Path = new PropertyPath("CountStr");
            //TextBox_Path.SetBinding(TextBox.TextProperty, _textBind);

            this.Load(path);
            //TODO Show selected file after open
            //if (filePath != null && Utility.IsSupported(System.IO.Path.GetExtension(filePath))            
        }

        private void Shutdown()
        {
            ClsThumbnailLoader.Stop();
            Application.Current.Shutdown();
        }

        private void Load(string path)
        {
            Load(path, true);
        }

        private void Load(string path, bool addToNav)
        {
            string imgPath = null;
            string dirPath;


            if (File.Exists(path))
            {
                if (Utility.IsArchive(System.IO.Path.GetExtension(path)))
                    dirPath = path;
                else
                {
                    dirPath = System.IO.Path.GetDirectoryName(path);
                    if (Utility.IsSupported(System.IO.Path.GetExtension(path)))
                        imgPath = path;
                }
            }
            else if (Directory.Exists(path))
                dirPath = path;
            else
                return; //TODO Log path not found error message

            if (addToNav)
                _navTracker.AddCurrentPath(dirPath);
            TextBox_Path.Text = dirPath; //TODO Use binding instead of set?                    

            ClsThumbnailLoader.Stop();            
            _fsColl.Update(dirPath);

            if (imgPath != null) //Show image
            {
                foreach (ClsFileSystemItem item in _fsColl)
                {
                    if (System.IO.Path.GetFileName(item.Path).Equals(System.IO.Path.GetFileName(imgPath))) //TODO Need refactor
                    {
                        ThumbnailListBox.SelectedItem = item;
                        _viewWindow.Show(ThumbnailListBox);
                        break;
                    }
                }
            }

            ClsThumbnailLoader.Load();
            UpdateNavBtns();
        }

        private void UpdateNavBtns()
        {
            BtnBack.IsEnabled = _navTracker.HasLastPath();
            BtnNext.IsEnabled = _navTracker.HasNextPath();
            BtnUp.IsEnabled = (_fsColl.ParentPath != null);
        }

        #region Events
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void MnuItmExit_Click(object sender, RoutedEventArgs e)
        {
            this.Shutdown();
        }

        private void BtnPathEnter_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Path.Text.Length > 0)
                Load(TextBox_Path.Text);
        }

        private void ThumbnailListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(ThumbnailListBox.SelectedItem.GetType().ToString());
            if (ThumbnailListBox.SelectedItem is ClsDirectoryItem)
            {
                string path = ((ClsDirectoryItem)ThumbnailListBox.SelectedItem).Path;
                TextBox_Path.Text = path;
                Load(path);
            }
            else if (ThumbnailListBox.SelectedItem is ClsFileItem)
            {
                ClsFileItem item = (ClsFileItem)ThumbnailListBox.SelectedItem;
                if (Utility.IsSupported(item.Extension))
                    _viewWindow.Show(ThumbnailListBox);
                if (Utility.IsArchive(item.Extension))
                    Load(item.Path);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Shutdown();
        }

        private void TextBox_Path_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && TextBox_Path.Text.Length > 0)
                Load(TextBox_Path.Text);
        }

        private void ThumbnailListBox_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine("ScrollViewer KeyDown: {0}, SelectIndex: {1}", e.Key.ToString(), ThumbnailListBox.SelectedIndex.ToString());
            if (e.Key == Key.Left && ThumbnailListBox.SelectedIndex > 0)
                ThumbnailListBox.SelectedIndex--;
            else if (e.Key == Key.Right && ThumbnailListBox.SelectedIndex < ThumbnailListBox.Items.Count - 1)
                ThumbnailListBox.SelectedIndex++;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Load(_navTracker.GetLastPath(), false);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {            
            this.Load(_navTracker.GetNextPath(), false);
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            this.Load(_navTracker.GetCurrentPath(), false);
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            if (_fsColl.ParentPath != null)
                this.Load(_fsColl.ParentPath);
        }
        #endregion
    }
}
