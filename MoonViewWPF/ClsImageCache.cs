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
using System.Windows.Threading;

namespace MoonView
{    
    public static class ClsImageCache
    {
        public static WinMain WinMain;

        private static object _accessLock = new object();

        private static List<string> _keyList = new List<string>();
        private static Dictionary<string, BitmapImage> _imageDict = new Dictionary<string, BitmapImage>();

        private delegate void UpdateStatusDelegate();

        /// <summary>Total number of cached image</summary>
        public static int Cached
        {
            get
            {
                lock (_accessLock)
                    return _keyList.Count;
            }
        }

        public static BitmapImage GetImage(IfImageItem item)
        {
            BitmapImage image;
            lock (_accessLock)
            {
                if (_imageDict.ContainsKey(item.ImageKey))
                {
                    if (_keyList.IndexOf(item.ImageKey) != _keyList.Count - 1)
                    {
                        _keyList.Remove(item.ImageKey);
                        _keyList.Add(item.ImageKey);
                    }
                    return _imageDict[item.ImageKey];
                }
                else
                {
                    //Load image
                    if (!Utility.IsSupported(item.Extension))
                        return null;

                    image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = item.Stream;
                    image.EndInit();
                    image.StreamSource.Dispose();
                    image.Freeze();

                    AddImage(item.ImageKey, image);
                    
                }
            }

            WinMain.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                new UpdateStatusDelegate(UpdateStatus));

            return image;
        }

        /// <summary>Add image to dictionary</summary>
        public static void AddImage(string key, BitmapImage image)
        {
            //Delete the oldest thumbnail
            lock (_accessLock)
            {
                if (_keyList.Count >= ClsOptions.MaxCachedImage)
                {
                    string oldestKey = _keyList[0];
                    _keyList.RemoveAt(0);
                    _imageDict.Remove(oldestKey);
                }
                _imageDict.Add(key, image);
                _keyList.Add(key);
            }
        }

        public static bool HasImage(string key)
        {
            lock (_accessLock)
            {
                if (_imageDict.ContainsKey(key))
                    return true;
                else
                    return false;
            }
        }

        private static void UpdateStatus()
        {
            WinMain.ImageCacheStatus.Content = string.Format("Image Cached {0}/{1}", ClsImageCache.Cached, ClsOptions.MaxCachedImage);
        }
    }
}
