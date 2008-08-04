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

namespace MoonView
{
    public static class ClsThumbnailCache
    {
        public static BitmapImage _fileImage;

        private static object _accessLock = new object();

        private static List<string> _keyList = new List<string>(); //Keylist for managing the oldest to newest image
        private static Dictionary<string, BitmapImage> _thumbnailDict = new Dictionary<string, BitmapImage>(); //Hashtable for storing images

        /// <summary>Total number of cached thumbnail</summary>
        public static int Cached
        {
            get
            {
                lock (_accessLock)
                    return _keyList.Count;
            }
        }

        public static void Initialize()
        {
            //Load file image
            _fileImage = new BitmapImage();
            _fileImage.BeginInit();
            _fileImage.DecodePixelWidth = 100;
            _fileImage.CacheOption = BitmapCacheOption.OnLoad;
            _fileImage.UriSource = new Uri("pack://application:,,,/Images/file.png");
            _fileImage.EndInit();
            _fileImage.Freeze();
        }

        /// <summary>Get thumbnail</summary>
        public static BitmapImage GetThumbnail(IfThumbnailItem item)
        {
            lock (_accessLock)
            {
                if (_thumbnailDict.ContainsKey(item.ThumbnailKey))
                {
                    if (_keyList.IndexOf(item.ThumbnailKey) != _keyList.Count - 1)
                    {
                        _keyList.Remove(item.ThumbnailKey); //Put key to the end of list
                        _keyList.Add(item.ThumbnailKey);
                    }
                    return _thumbnailDict[item.ThumbnailKey];
                }
                else
                    return null;
            }
        }

        /// <summary>Add thumbnail to thumbnail dict</summary>
        public static void AddThumbnail(string key, BitmapImage thumbnail)
        {
            //Delete the oldest thumbnail
            lock (_accessLock)
            {
                if (_keyList.Count >= ClsOptions.MaxCachedThumbnail)
                {
                    string oldestKey = _keyList[0];
                    _keyList.RemoveAt(0);
                    _thumbnailDict.Remove(oldestKey);
                }
                _thumbnailDict.Add(key, thumbnail);
                _keyList.Add(key);
            }
        }

        /// <summary></summary>
        /// <param name="keyObj"></param>
        /// <returns></returns>
        public static bool HasThumbnail(string key)
        {
            lock (_accessLock)
            {
                if (_thumbnailDict.ContainsKey(key))
                    return true;
                else
                    return false;
            }
        }
    }
}
