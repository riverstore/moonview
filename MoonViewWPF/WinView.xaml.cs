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
using System.Windows.Shapes;
using System.ComponentModel;

namespace MoonView
{
    /// <summary>
    /// Interaction logic for ViewWindow.xaml
    /// </summary>
    public partial class WinView : Window
    {
        public enum ScaleType
        {
            Zoom,
            FitWidth,
            FitHeight
        }

        private bool _hidden;
        private double _scale = 1;
        private ScaleType _scaleType = ScaleType.Zoom;
        private BitmapImage _image;
        private ListBox _listBox;

        public WinView()
        {
            InitializeComponent();
        }

        public BitmapImage ViewImage
        {
            get { return _image; }
        }

        public void Show(ListBox listBox)
        {
            _listBox = listBox;
            _listBox.SelectionChanged += new SelectionChangedEventHandler(_listBox_SelectionChanged);
            _listBox_SelectionChanged(null, null);

            if (_hidden)
            {
                IntPtr winPtr = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                ShowWindowAsync(winPtr, 1);
            }
            else
                this.Show();
        }

        private void _listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listBox.SelectedItem is ClsFileItem)
            {
                ClsFileItem item = (ClsFileItem)_listBox.SelectedItem;
                if (Utility.IsSupported(item.Extension))
                {
                    _image = item.Image;
                    ImgView.Source = _image;
                    this.Title = string.Format("ViewWindow - {0}", item.Path);
                    Rescale();
                }
            }
        }

        //http://forums.msdn.microsoft.com/en/wpf/thread/047f4620-558e-4441-9c67-9e9ffe321561/
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _hidden = true;
            if (_listBox != null)
                _listBox.SelectionChanged -= new SelectionChangedEventHandler(_listBox_SelectionChanged);

            e.Cancel = true;
            IntPtr winPtr = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            ShowWindowAsync(winPtr, 0);
        }

        private void BtnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _scale = _scale * 1.1;
            _scaleType = ScaleType.Zoom;
            Rescale();
        }

        private void BtnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _scale = _scale / 1.1;
            _scaleType = ScaleType.Zoom;
            Rescale();
        }

        private void BtnFitWidth_Click(object sender, RoutedEventArgs e)
        {
            _scaleType = ScaleType.FitWidth;
            Rescale();
        }

        private void BtnFitHeight_Click(object sender, RoutedEventArgs e)
        {
            _scaleType = ScaleType.FitHeight;
            Rescale();
        }

        private void BtnNoZoom_Click(object sender, RoutedEventArgs e)
        {
            _scale = 1.0;
            _scaleType = ScaleType.Zoom;
            Rescale();
        }

        private void Rescale()
        {
            if (_scaleType == ScaleType.FitHeight)
            {
                _scale = (SViewer_Image.ActualHeight - 10) / _image.Height;
            }
            else if (_scaleType == ScaleType.FitWidth)
            {
                _scale = (SViewer_Image.ActualWidth - 20) / _image.Width;
            }
            ImgView.Width = _image.Width * _scale;
            ImgView.Height = _image.Height * _scale;
        }

        private void ButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            PrevImage();
        }

        private void PrevImage()
        {
            for (int i = _listBox.SelectedIndex - 1; i >= 0; i--)
            {
                if (_listBox.Items[i] is ClsFileItem)
                {
                    ClsFileItem item = (ClsFileItem)_listBox.Items[i];
                    if (Utility.IsSupported(item.Extension))
                    {
                        _listBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            NextImage();
        }

        private void NextImage()
        {
            for (int i = _listBox.SelectedIndex + 1; i < _listBox.Items.Count; i++)
            {
                if (_listBox.Items[i] is ClsFileItem)
                {
                    ClsFileItem item = (ClsFileItem)_listBox.Items[i];
                    if (Utility.IsSupported(item.Extension))
                    {
                        _listBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rescale();
        }

        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            SViewer_Image.Focus();
            if (e.Key == Key.Left)   
                PrevImage();
            else if (e.Key == Key.Right)
                NextImage();
        }

        private void SViewer_Image_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                NextImage();
            else if (e.Key == Key.Right)
                NextImage();
            else if (e.Key == Key.Left)
                PrevImage();
            else if (e.Key == Key.Add)
            {
                _scale = _scale * 1.1;
                _scaleType = ScaleType.Zoom;
                Rescale();
            }
            else if (e.Key == Key.Subtract)
            {
                _scale = _scale / 1.1;
                _scaleType = ScaleType.Zoom;
                Rescale();
            }
        }
    }
}

