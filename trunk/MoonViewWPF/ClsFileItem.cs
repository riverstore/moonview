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
using System.Threading;
using System.ComponentModel;

namespace MoonView
{
    public class ClsFileItem : ClsFileSystemItem, INotifyPropertyChanged, IfThumbnailItem, IfImageItem
    {
        protected string _imageKey;
        protected string _thumbnailKey;
        protected BitmapImage _thumbnail;

        public virtual string FileName
        {
            get { return System.IO.Path.GetFileName(_path); }
        }

        public virtual string Extension
        {
            get { return System.IO.Path.GetExtension(_path).ToUpper().Replace(".", ""); }
        }

        public virtual Stream Stream
        {
            get { return File.OpenRead(_path); }
        }

        public virtual string ImageKey
        {
            set { _imageKey = value; }
            get { return _imageKey; }
        }

        public virtual string ThumbnailKey
        {
            set { _thumbnailKey = value; }
            get { return _thumbnailKey; }
        }

        public virtual BitmapImage Thumbnail
        {
            get { return ClsThumbnailCache.GetThumbnail(this); }
        }

        public virtual void OnThumbnailLoaded()
        {
            NotifyPropertyChanged("Thumbnail");
        }

        public BitmapImage Image
        {
            get { return ClsImageCache.GetImage(this); }
        }

        public ClsFileItem(string path)
            : base(path)
        {
            _imageKey = string.Format("{0}({1},Image)", path, File.GetLastWriteTime(path).ToString());
            _thumbnailKey = string.Format("{0}({1},Thumbnail)", path, File.GetLastWriteTime(path).ToString());
            ClsThumbnailLoader.Enqueue(this);
        }

        public ClsFileItem(string path, DateTime time)
            : base(path)
        {
            _imageKey = string.Format("{0}({1},Image)", path, time.ToString());
            _thumbnailKey = string.Format("{0}({1},Thumbnail)", path, time.ToString());
            ClsThumbnailLoader.Enqueue(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }
    }
}
