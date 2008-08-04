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
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;

namespace MoonView
{
    public class ClsThumbnailImage : IfThumbnailItem, INotifyPropertyChanged
    {
        private string _path;
        private string _thumbnailKey;

        public string Extension
        {
            get { return Path.GetExtension(_path); }
        }

        public string ThumbnailKey
        {
            set { _thumbnailKey = value; }
            get { return _thumbnailKey; }
        }

        public BitmapImage Thumbnail
        {
            get { return ClsThumbnailCache.GetThumbnail(this); }
        }

        public void OnThumbnailLoaded()
        {
            NotifyPropertyChanged("Thumbnail");
        }

        public Stream Stream
        {
            get { return File.OpenRead(_path); }
        }

        public ClsThumbnailImage(string path)
        {
            _path = path;
            _thumbnailKey = string.Format("{0}({1},Thumbnail)", path , File.GetLastWriteTime(path).ToString());
            ClsThumbnailLoader.Enqueue(this);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
        #endregion
    }
}
