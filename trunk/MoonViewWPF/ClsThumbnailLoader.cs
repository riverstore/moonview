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
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace MoonView
{
    public static class ClsThumbnailLoader
    {
        private static object _loadLock = new object();
        private static bool _loading = false;
        private static bool _cancel = false;
        private static WinMain _winMain;

        //Collections
        private static Queue<IfThumbnailItem> _queue = new Queue<IfThumbnailItem>(); 
        
        //Delegate
        private delegate void LoadThumbnailDelegate();
        private delegate void NotifyThumbnailLoadedDelegate(IfThumbnailItem item);

        #region Properties
        public static WinMain WinMain
        {
            get { return _winMain; }
            set { _winMain = value; }
        }

        public static bool IsLoading
        {
            get { return _loading; }
        }
        #endregion

        #region Public Methods
        public static void Enqueue(IfThumbnailItem item)
        {
            lock (_queue)
                _queue.Enqueue(item);
        }

        public static void Load()
        {
            LoadThumbnailDelegate updateImage = new LoadThumbnailDelegate(LoadImageQueue);
            updateImage.BeginInvoke(null, null);
        }

        /// <summary></summary>
        public static void Stop()
        {
            if (_loading)
                _cancel = true;
        }
        #endregion

        /// <summary></summary>
        private static void LoadImageQueue()
        {
            lock (_loadLock)
            {
                lock (_queue)
                {
                    _loading = true;
                    while (_queue.Count > 0)
                    {
                        if (_cancel)
                            break;
                        IfThumbnailItem item = _queue.Dequeue();

                        if (!ClsThumbnailCache.HasThumbnail(item.ThumbnailKey))
                        {
                            try
                            {
                                if (!Utility.IsSupported(item.Extension))
                                    continue; //TODO return appropriate thumbnail for non-image file

                                BitmapImage image = new BitmapImage();
                                image.BeginInit();
                                image.DecodePixelWidth = 100;
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = item.Stream;
                                image.EndInit();
                                image.StreamSource.Dispose();
                                image.Freeze();

                                ClsThumbnailCache.AddThumbnail(item.ThumbnailKey, image);
                            }
                            catch (NotSupportedException nsex)
                            {
                                Console.WriteLine(nsex.Message);
                            }
                        }

                        _winMain.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                            new NotifyThumbnailLoadedDelegate(NotifyThumbnailItem), item);
                    }
                    _queue.Clear();
                    _loading = false;
                    _cancel = false;
                }                
            }
        }

        private static void NotifyThumbnailItem(IfThumbnailItem item)
        {
            item.OnThumbnailLoaded();
            _winMain.ThumbnailCacheStatus.Content = string.Format("Thumbnail Cached {0}/{1}", ClsThumbnailCache.Cached, ClsOptions.MaxCachedThumbnail);
        }
    }
}
